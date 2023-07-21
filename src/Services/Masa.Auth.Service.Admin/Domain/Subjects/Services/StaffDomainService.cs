// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class StaffDomainService : DomainService
{
    readonly UserDomainService _userDomainService;
    readonly IStaffRepository _staffRepository;

    public StaffDomainService(UserDomainService userDomainService, IStaffRepository staffRepository)
    {
        _userDomainService = userDomainService;
        _staffRepository = staffRepository;
    }

    public async Task<Staff> RemoveAsync(Guid Id)
    {
        var staff = await _staffRepository.FindAsync(Id);
        if (staff == null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_FOUND);
        }
        await _staffRepository.RemoveAsync(staff);
        return staff;
    }

    public async Task<Staff> AddAsync(AddStaffDto staffDto)
    {
        await VerifyRepeatAsync(staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard, default);

        var staff = new Staff(
                staffDto.Name,
                staffDto.DisplayName,
                staffDto.Avatar,
                staffDto.IdCard,
                staffDto.CompanyName,
                staffDto.Gender,
                staffDto.PhoneNumber,
                staffDto.Email,
                staffDto.JobNumber,
                await GetPositonId(staffDto.Position),
                staffDto.StaffType,
                staffDto.Enabled,
                staffDto.Address
            );
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        await _userDomainService.AddAsync(new User(staffDto.Name, staffDto.DisplayName, staffDto.Avatar, staffDto.Account,
            staffDto.Password, staffDto.CompanyName, staffDto.Email, staffDto.PhoneNumber, staff));
        return staff;
    }

    /// <summary>
    /// Update Staff info,but not update relation user info
    /// </summary>
    /// <param name="staffDto"></param>
    /// <returns></returns>
    public async Task<Staff> UpdateAsync(UpdateStaffDto staffDto)
    {
        var (_, exception) = await VerifyRepeatAsync(staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard, staffDto.Id);
        if (exception != null)
        {
            throw exception;
        }
        var staff = await _staffRepository.GetDetailByIdAsync(staffDto.Id);
        staff.Update(
            await GetPositonId(staffDto.Position), staffDto.StaffType, staffDto.Enabled, staffDto.Name,
            staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName,
            staffDto.PhoneNumber, staffDto.Email, staffDto.Address, staffDto.Gender);
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        staff.SetTeamStaff(staffDto.Teams);
        await _staffRepository.UpdateAsync(staff);
        return staff;
    }

    async Task<Guid?> GetPositonId(string? position)
    {
        Guid? positionId = default(Guid?);
        if (!position.IsNullOrEmpty())
        {
            var positionCommand = new UpsertPositionCommand(position);
            await EventBus.PublishAsync(positionCommand);
            positionId = positionCommand.Result;
        }
        return positionId;
    }

    public async Task<(Staff?, UserFriendlyException?)> VerifyRepeatAsync(string? jobNumber, string? phoneNumber, string? email, string? idCard, Guid? curStaffId = default)
    {
        Expression<Func<Staff, bool>> condition = staff => false;
        condition = condition.Or(!string.IsNullOrEmpty(jobNumber), staff => staff.JobNumber == jobNumber);
        condition = condition.Or(!string.IsNullOrEmpty(phoneNumber), staff => staff.PhoneNumber == phoneNumber);
        condition = condition.Or(!string.IsNullOrEmpty(email), staff => staff.Email == email);
        condition = condition.Or(!string.IsNullOrEmpty(idCard), staff => staff.IdCard == idCard);
        condition = condition.And(curStaffId is not null, staff => staff.Id != curStaffId);

        var existStaff = await _staffRepository.FindAsync(condition);
        UserFriendlyException? exception = null;
        if (existStaff is not null)
        {
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == existStaff.PhoneNumber)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(email) is false && email == existStaff.Email)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_EMAIL_EXIST, email);
            if (string.IsNullOrEmpty(idCard) is false && idCard == existStaff.IdCard)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_ID_CARD_EXIST, idCard);
            if (string.IsNullOrEmpty(jobNumber) is false && jobNumber == existStaff.JobNumber)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_JOB_NUMBER_EXIST, jobNumber);
        }
        return (existStaff, exception);
    }
}
