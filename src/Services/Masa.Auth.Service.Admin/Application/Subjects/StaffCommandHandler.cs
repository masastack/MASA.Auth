// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class StaffCommandHandler
{
    readonly IStaffRepository _staffRepository;
    readonly StaffDomainService _staffDomainService;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly ILogger<CommandHandler> _logger;
    readonly IEventBus _eventBus;
    readonly PhoneHelper _phoneHelper;

    public StaffCommandHandler(
        IStaffRepository staffRepository,
        StaffDomainService staffDomainService,
        IDistributedCacheClient distributedCacheClient,
        ILogger<CommandHandler> logger,
        IEventBus eventBus,
        PhoneHelper phoneHelper)
    {
        _staffRepository = staffRepository;
        _staffDomainService = staffDomainService;
        _distributedCacheClient = distributedCacheClient;
        _logger = logger;
        _eventBus = eventBus;
        _phoneHelper = phoneHelper;
    }

    [EventHandler(1)]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        await _staffDomainService.AddAsync(command.Staff);
    }

    [EventHandler(1)]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        command.Result = await _staffDomainService.UpdateAsync(command.Staff);
    }

    [EventHandler(1)]
    public async Task ChangeStaffCurrentTeamAsync(UpdateStaffCurrentTeamCommand updateStaffCurrentTeamCommand)
    {
        var staff = await _staffRepository.FindAsync(s => s.UserId == updateStaffCurrentTeamCommand.UserId);
        if (staff == null)
        {
            _logger.LogError($"Can`t find staff by UserId = {updateStaffCurrentTeamCommand.UserId}");
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);
        }
        staff.SetCurrentTeam(updateStaffCurrentTeamCommand.TeamId);
        await _staffRepository.UpdateAsync(staff);
        updateStaffCurrentTeamCommand.Result = staff;
    }

    [EventHandler(1)]
    public async Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoCommand command)
    {
        var staffModel = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == command.Staff.UserId);
        if (staff is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);

        staff.UpdateBasicInfo(staffModel.Name, staffModel.DisplayName, staffModel.Gender, staffModel.PhoneNumber, staffModel.Email);
        await _staffRepository.UpdateAsync(staff);
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task UpdateStaffAvatarAsync(UpdateStaffAvatarCommand command)
    {
        var staffDto = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == staffDto.UserId);
        if (staff is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);

        staff.UpdateAvatar(staffDto.Avatar);
        await _staffRepository.UpdateAsync(staff);
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        command.Result = await _staffDomainService.RemoveAsync(command.Staff.Id);
    }

    [EventHandler(1)]
    public async Task SyncStaffAsync(SyncStaffCommand command)
    {
        var syncResults = new SyncStaffResultsDto();
        command.Result = syncResults;
        var syncStaffs = command.Staffs;
        //validation
        var validator = new SyncStaffValidator(new PhoneNumberValidator(_phoneHelper));
        for (var i = 0; i < syncStaffs.Count; i++)
        {
            var staff = syncStaffs[i];
            var result = validator.Validate(staff);
            if (result.IsValid is false)
            {
                syncResults[i] = new()
                {
                    JobNumber = staff.JobNumber,
                    Errors = result.Errors.GroupBy(error => error.PropertyName).Select(e => e.First().ErrorMessage).ToList()
                };
            }
        }
        //check duplicate
        CheckDuplicate(Staff => Staff.PhoneNumber);
        CheckDuplicate(Staff => Staff.JobNumber);
        CheckDuplicate(Staff => Staff.Email);
        CheckDuplicate(Staff => Staff.IdCard);
        if (syncResults.IsValid) return;

        var defaultPasswordQuery = new StaffDefaultPasswordQuery();
        await _eventBus.PublishAsync(defaultPasswordQuery);
        string defaultPassword = defaultPasswordQuery.Result.DefaultPassword.WhenNullOrEmptyReplace(DefaultUserAttributes.Password);
        //sync user
        for (var i = 0; i < syncStaffs.Count; i++)
        {
            var syncStaff = syncStaffs[i];
            try
            {
                var (existStaff, _) = await _staffDomainService.VerifyRepeatAsync(syncStaff.JobNumber, syncStaff.PhoneNumber, syncStaff.Email, syncStaff.IdCard);
                if (existStaff is not null)
                {
                    var checkResult = await CheckSyncDataAsync(existStaff, syncStaff);
                    if (!checkResult.State)
                    {
                        if (checkResult.StaffResult != null)
                        {
                            syncResults[i] = checkResult.StaffResult;
                        }
                        continue;
                    }
                    await _staffDomainService.UpdateAsync(new UpdateStaffDto
                    {
                        Id = existStaff.Id,
                        Name = syncStaff.Name,
                        DisplayName = syncStaff.DisplayName,
                        Email = syncStaff.Email,
                        IdCard = syncStaff.IdCard,
                        Gender = syncStaff.Gender,
                        StaffType = syncStaff.StaffType,
                        Position = syncStaff.Position,
                        PhoneNumber = syncStaff.PhoneNumber
                    });
                }
                else
                {
                    var addStaffDto = new AddStaffDto
                    {
                        Name = syncStaff.Name,
                        DisplayName = syncStaff.DisplayName,
                        Enabled = true,
                        Email = syncStaff.Email,
                        Password = defaultPassword,
                        PhoneNumber = syncStaff.PhoneNumber,
                        JobNumber = syncStaff.JobNumber,
                        IdCard = syncStaff.IdCard,
                        Position = syncStaff.Position,
                        Gender = syncStaff.Gender,
                        StaffType = syncStaff.StaffType,
                    };
                    await _staffDomainService.AddAsync(addStaffDto);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex is UserFriendlyException ? ex.Message : "Unknown exception, please contact the administrator";
                syncResults[i] = new()
                {
                    JobNumber = syncStaff.JobNumber,
                    Errors = new() { errorMsg }
                };
            }
        }

        void CheckDuplicate(Expression<Func<SyncStaffDto, string?>> selector)
        {
            var func = selector.Compile();
            if (syncStaffs.Where(staff => string.IsNullOrEmpty(func(staff)) is false).IsDuplicate(func, out List<SyncStaffDto>? duplicates))
            {
                foreach (var duplicate in duplicates)
                {
                    var index = syncStaffs.IndexOf(duplicate);
                    var staff = syncStaffs[index];
                    syncResults[index] = new()
                    {
                        JobNumber = staff.JobNumber,
                        Errors = new() { $"{(selector.Body as MemberExpression)!.Member.Name}:{func(staff)} - duplicate" }
                    };
                }
            }
        }

        SyncStaffResultsDto.SyncStaffResult Error(string jobNumber, params string[] errorMessages) =>
            new SyncStaffResultsDto.SyncStaffResult()
            {
                JobNumber = jobNumber,
                Errors = errorMessages.ToList()
            };

        async Task<(bool State, SyncStaffResultsDto.SyncStaffResult? StaffResult)> CheckSyncDataAsync(Staff existStaff,
            SyncStaffDto syncStaff)
        {
            // Do not flush to db if the sync staff info is same with exist staff. 
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber &&
                existStaff.JobNumber == syncStaff.JobNumber &&
                existStaff.DisplayName == syncStaff.DisplayName.WhenNullOrEmptyReplace("") &&
                existStaff.Name == syncStaff.Name.WhenNullOrEmptyReplace("") &&
                existStaff.Gender == syncStaff.Gender &&
                existStaff.StaffType == syncStaff.StaffType &&
                existStaff.Position != null &&
                existStaff.Position.Name == syncStaff.Position.WhenNullOrEmptyReplace("") &&
                existStaff.Email == syncStaff.Email.WhenNullOrEmptyReplace("") &&
                existStaff.IdCard == syncStaff.IdCard.WhenNullOrEmptyReplace(""))
            {
                return (false, default);
            }

            // When phone number is not equals and job number is not equals, the email and id card cannot be equal! 
            if (existStaff.PhoneNumber != syncStaff.PhoneNumber && existStaff.JobNumber != syncStaff.JobNumber)
            {
                if (!string.IsNullOrWhiteSpace(syncStaff.Email) && existStaff.Email == syncStaff.Email)
                {
                    return (false, Error(syncStaff.JobNumber,
                        $"The employee whose email is {syncStaff.Email} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                }

                if (!string.IsNullOrWhiteSpace(syncStaff.IdCard) && existStaff.IdCard == syncStaff.IdCard)
                {
                    return (false, Error(syncStaff.JobNumber,
                        $"The employee whose id card is {syncStaff.IdCard} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                }
            }

            // When job number is equals, the phone number must be equal
            if (existStaff.JobNumber == syncStaff.JobNumber && existStaff.PhoneNumber != syncStaff.PhoneNumber)
            {
                return (false, Error(syncStaff.JobNumber,
                    $"The employee whose job number is {syncStaff.JobNumber}, the corresponding mobile phone number is {existStaff.PhoneNumber}, which does not match the mobile phone number {syncStaff.PhoneNumber}"));
            }

            // When phone number is equals, the job number must be equal
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber && existStaff.JobNumber != syncStaff.JobNumber)
            {
                return (false, Error(syncStaff.JobNumber,
                    $"The employee whose mobile phone number is {syncStaff.PhoneNumber} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
            }

            // When job number is equal and phone number is equal, the staff whose is email or id card cannot be equal for other staff who is in database.
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber && existStaff.JobNumber == syncStaff.JobNumber)
            {
                if (!string.IsNullOrWhiteSpace(syncStaff.IdCard))
                {
                    var (existIdCardStaff, _) = await _staffDomainService.VerifyRepeatAsync(default, default, default, syncStaff.IdCard);
                    if (existIdCardStaff != null && existIdCardStaff.JobNumber != syncStaff.JobNumber)
                    {
                        return (false, Error(syncStaff.JobNumber,
                            $"The employee whose id card number is {syncStaff.IdCard} has a corresponding job number of {existIdCardStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                    }
                }

                if (!string.IsNullOrWhiteSpace(syncStaff.Email))
                {
                    var (existEmailStaff, _) = await _staffDomainService.VerifyRepeatAsync(default, default, syncStaff.Email, default);
                    if (existEmailStaff != null && existEmailStaff.JobNumber != syncStaff.JobNumber)
                    {
                        return (false, Error(syncStaff.JobNumber,
                            $"The employee whose email is {syncStaff.Email} has a corresponding job number of {existEmailStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                    }
                }
            }

            return (true, default);
        }
    }

    [EventHandler(1)]
    public async Task UpdateStaffDefaultPasswordAsync(UpdateStaffDefaultPasswordCommand command)
    {
        await _distributedCacheClient.SetAsync(CacheKey.STAFF_DEFAULT_PASSWORD, command.DefaultPassword);
    }

}
