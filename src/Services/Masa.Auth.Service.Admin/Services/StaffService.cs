// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class StaffService : RestServiceBase
{
    public StaffService(IServiceCollection services) : base(services, "api/staff")
    {
        MapPost(SyncAsync);
    }

    private async Task<PaginationDto<StaffDto>> GetListAsync(IEventBus eventBus, GetStaffsDto staff)
    {
        var query = new StaffsQuery(staff.Page, staff.PageSize, staff.Search, staff.Enabled, staff.DepartmentId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<StaffDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<StaffSelectDto>> GetSelectAsync(IEventBus eventBus, [FromQuery] string name)
    {
        var query = new StaffSelectQuery(name);
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

    private async Task RemoveAsync(IEventBus eventBus,
        [FromBody] RemoveStaffDto staff)
    {
        var deleteCommand = new RemoveStaffCommand(staff);
        await eventBus.PublishAsync(deleteCommand);
    }

    private async Task<SyncStaffResultsDto> SyncAsync(IEventBus eventBus, HttpRequest request)
    {
        if (request.HasFormContentType is false) throw new Exception("Only supported formContent");
        var form = await request.ReadFormAsync();
        if (form.Files.Count <= 0) throw new UserFriendlyException("File not found");
        var file = form.Files.First();
        ICsvImporter importer = new CsvImporter();
        using var stream = file.OpenReadStream();
        var import = await importer.Import<SyncStaffDto>(stream);
        if (import.HasError) throw new UserFriendlyException("Read file data failed");
        var syncCommand = new SyncStaffCommand(import.Data.ToList());
        await eventBus.PublishAsync(syncCommand);
        return syncCommand.Result;
    }
}
