namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    List<StaffItemResponse> StaffItems => new List<StaffItemResponse>()
    {

    };

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationItemsResponse<StaffItemResponse>> GetStaffItemsAsync(GetStaffItemsRequest request)
    {
        var staffs = StaffItems.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationItemsResponse<StaffItemResponse>(StaffItems.Count, 1, staffs));
    }

    public async Task<StaffDetailResponse> GetStaffDetailAsync(Guid id)
    {
        return await Task.FromResult(StaffDetailResponse.Default);
    }

    public async Task AddStaffAsync(AddStaffRequest request)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateStaffAsync(UpdateStaffRequest request)
    {
        await Task.CompletedTask;
    }

    public async Task DeleteStaffAsync(Guid staffId)
    {
        StaffItems.Remove(StaffItems.First(s => s.StaffId == staffId));
        await Task.CompletedTask;
    }
}

