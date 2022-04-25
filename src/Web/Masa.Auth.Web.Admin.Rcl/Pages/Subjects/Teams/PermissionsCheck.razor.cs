using StackApp = Masa.Stack.Components.Models.App;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class PermissionsCheck
{
    [Parameter]
    public List<Guid> RoleIds { get; set; } = new();

    [Parameter]
    public Dictionary<Guid, bool> Value { get; set; } = new();

    [Parameter]
    public EventCallback<Dictionary<Guid, bool>> ValueChanged { get; set; }

    List<Guid> _prevRoleIds = new();
    Dictionary<Guid, bool> _prevValue = new();
    List<Category> _categories = new();
    List<CategoryAppNav> _initValue = new();

    ProjectService ProjectService => AuthCaller.ProjectService;

    RoleService RoleService => AuthCaller.RoleService;

    private void ValueChangedHandler(List<CategoryAppNav> _checkedItems)
    {
        if (_checkedItems != null)
        {
            var navKeys = _checkedItems.Select(i => i.Nav).ToList();
            foreach (var keyValue in Value)
            {
                if (!navKeys.Contains(keyValue.Key.ToString()))
                {
                    Value[keyValue.Key] = false;
                }
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_prevRoleIds.SequenceEqual(RoleIds))
        {
            _prevRoleIds = RoleIds;
            await LoadRolePermissions();
        }
        if (!_prevValue.SequenceEqual(Value))
        {
            _prevValue = Value;
        }
    }

    private async Task LoadData()
    {
        var apps = (await ProjectService.GetListAsync(true)).SelectMany(p => p.Apps).ToList();
        var Categories = apps.GroupBy(a => a.Tag).Select(ag => new Category
        {
            Code = ag.Key,
            Name = ag.Key,
            Apps = ag.Select(a => a.Adapt<StackApp>()).ToList()
        }).ToList();

        var allKey = Categories.SelectMany(c => c.Apps).SelectMany(a => a.Navs).Select(a => Guid.Parse(a.Code));
        foreach (var key in allKey)
        {
            if (!Value.ContainsKey(key))
            {
                Value[key] = false;
            }
        }
    }

    private async Task LoadRolePermissions()
    {
        var rolePermissions = await RoleService.GetPermissionsByRoleAsync(RoleIds);
        _initValue = rolePermissions.Select(p => new CategoryAppNav("", "", p.ToString())).ToList();
    }
}
