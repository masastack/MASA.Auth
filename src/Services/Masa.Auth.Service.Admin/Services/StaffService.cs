// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class StaffService : RestServiceBase
{
    public StaffService(IServiceCollection services) : base(services, "api/staff")
    {
        MapPost(SyncAsync);
        MapPost(SelectByIdsAsync, "SelectByIds");
    }

    private async Task<PaginationDto<StaffDto>> GetListAsync(IEventBus eventBus, GetStaffsDto staff)
    {
        var query = new StaffsQuery(staff.Page, staff.PageSize, staff.Search, staff.Enabled, staff.DepartmentId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<StaffModel>> GetListByDepartmentAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffsByDepartmentQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result.Select(staff => ConvertToModel(staff)).ToList();
    }

    private async Task<List<StaffModel>> GetListByTeamAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffsByTeamQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result.Select(staff => ConvertToModel(staff)).ToList();
    }

    private async Task<List<StaffModel>> GetListByRoleAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffsByRoleQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result.Select(staff => ConvertToModel(staff)).ToList();
    }

    private async Task<int> GetTotalByDepartmentAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffTotalByDepartmentQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<int> GetTotalByTeamAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffTotalByTeamQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<int> GetTotalByRoleAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffTotalByRoleQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private StaffModel ConvertToModel(StaffDto staff)
    {
        return new StaffModel()
        {
            Id = staff.Id,
            Department = staff.Department,
            JobNumber = staff.JobNumber,
            Position = staff.Position,
            StaffType = Enum.Parse<StaffTypes>(staff.StaffType.ToString()),
            UserId = staff.UserId,
            Name = staff.Name,
            DisplayName = staff.DisplayName,
            IdCard = staff.IdCard,
            CompanyName = staff.CompanyName,
            PhoneNumber = staff.PhoneNumber,
            Email = staff.Email,
            Gender = staff.Gender,
            Avatar = staff.Avatar,
            Address = new AddressValueModel
            {
                Address = staff.Address.Address
            }
        };
    }

    private async Task<StaffDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<StaffDetailModel?> GetDetailByUserIdAsync(IEventBus eventBus, [FromQuery] Guid userId)
    {
        var query = new StaffDetailByUserIdQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<StaffSelectDto>> GetSelectAsync(IEventBus eventBus, [FromQuery] string? name)
    {
        var query = new StaffSelectQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<StaffSelectDto>> SelectByIdsAsync(IEventBus eventBus, [FromBody] List<Guid> Ids)
    {
        var query = new StaffSelectByIdQuery(Ids);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddAsync(IEventBus eventBus,
        [FromBody] AddStaffDto staff)
    {
        await eventBus.PublishAsync(new AddStaffCommand(staff));
    }

    private async Task UpdateAsync(IEventBus eventBus,
        [FromBody] UpdateStaffDto staff)
    {
        await eventBus.PublishAsync(new UpdateStaffCommand(staff));
    }

    private async Task UpdateBasicInfoAsync(IEventBus eventBus,
        [FromBody] UpdateStaffBasicInfoModel staff)
    {
        await eventBus.PublishAsync(new UpdateStaffBasicInfoCommand(staff));
    }

    private async Task UpdateAvatarAsync(IEventBus eventBus,
        [FromBody] UpdateStaffAvatarModel staff)
    {
        await eventBus.PublishAsync(new UpdateStaffAvatarCommand(staff));
    }

    private async Task RemoveAsync(IEventBus eventBus,
        [FromBody] RemoveStaffDto staff)
    {
        var deleteCommand = new RemoveStaffCommand(staff);
        await eventBus.PublishAsync(deleteCommand);
    }

    //private async Task<SyncStaffResultsDto> SyncAsync(IEventBus eventBus, HttpRequest request)
    //{
    //    if (request.HasFormContentType is false) throw new Exception("Only supported formContent");
    //    var form = await request.ReadFormAsync();
    //    if (form.Files.Count <= 0) throw new UserFriendlyException("File not found");
    //    var file = form.Files.First();
    //    ICsvImporter importer = new CsvImporter();
    //    using var stream = file.OpenReadStream();
    //    var import = await importer.Import<SyncStaffDto>(stream);
    //    if (import.HasError) throw new UserFriendlyException("Read file data failed");
    //    var syncCommand = new SyncStaffCommand(import.Data.ToList());
    //    await eventBus.PublishAsync(syncCommand);
    //    return syncCommand.Result;
    //}

    private async Task<SyncStaffResultsDto> SyncAsync(IEventBus eventBus, UploadFileDto file)
    {
        ICsvImporter importer = new CsvImporter();
        using var stream = new MemoryStream(file.FileContent);
        var import = await importer.Import<SyncStaffDto>(stream);
        if (import.HasError) throw new UserFriendlyException("Read file data failed");
        var syncCommand = new SyncStaffCommand(import.Data.ToList());
        await eventBus.PublishAsync(syncCommand);
        return syncCommand.Result;
    }

    private async Task UpdateCurrentTeamAsync(IEventBus eventBus, [FromBody] UpdateCurrentTeamDto updateCurrentTeamDto)
    {
        //todo UpdateCurrentTeamModel
        var updateCurrentTeamCommand = new UpdateStaffCurrentTeamCommand(updateCurrentTeamDto.UserId, updateCurrentTeamDto.TeamId);
        await eventBus.PublishAsync(updateCurrentTeamCommand);
    }
}
