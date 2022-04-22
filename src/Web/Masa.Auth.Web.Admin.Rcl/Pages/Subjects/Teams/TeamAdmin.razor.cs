using Mapster;
using StackApp = Masa.Stack.Components.Models.App;
namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamAdmin
{
    [Parameter]
    public TeamPersonnelDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamPersonnelDto> ValueChanged { get; set; }

    bool _staffLoading, _roleLoading;
    List<StaffSelectDto> _staffs = new List<StaffSelectDto>();
    List<StaffSelectDto> _roles = new List<StaffSelectDto>();
    StaffService StaffService => AuthCaller.StaffService;
    ProjectService ProjectService => AuthCaller.ProjectService;
    List<Category> Categories = new();
    List<FavoriteNav> FavoriteNavs = new();
    List<CategoryAppNav> categoryAppNavs = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Categories = await FetchCategories();
            StateHasChanged();
        }
    }

    private async Task<List<Category>> FetchCategories()
    {
        var apps = (await ProjectService.GetListAsync(true)).SelectMany(p => p.Apps).ToList();
        var categories = apps.GroupBy(a => a.Tag).Select(ag => new Category
        {
            Code = ag.Key,
            Name = ag.Key,
            Apps = ag.Select(a => a.Adapt<StackApp>()).ToList()
        }).ToList();
        return categories;
    }

    private void Test()
    {
        var d = Categories;
        var dd = FavoriteNavs;
        var ddd = categoryAppNavs;
    }

    public void RemoveAdmin(Guid staffId)
    {
        var index = Value.Staffs.IndexOf(staffId);
        if (index >= 0)
        {
            Value.Staffs.RemoveAt(index);
        }
    }

    private async Task QuerySelectionStaff(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return;
        }

        _staffLoading = true;
        _staffs = await StaffService.GetSelectAsync(search);
        _staffLoading = false;
    }

    private async Task QuerySelectionRole(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return;
        }

        _roleLoading = true;
        _roles = await StaffService.GetSelectAsync(search);
        _roleLoading = false;
    }
}
