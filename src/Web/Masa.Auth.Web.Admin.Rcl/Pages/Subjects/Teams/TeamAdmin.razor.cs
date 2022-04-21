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
    List<Category> Categories = new();
    List<FavoriteNav> FavoriteNavs = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Categories = await FetchCategories();

            StateHasChanged();
        }
    }

    private Task<List<Category>> FetchCategories()
    {
        var categories = new List<Category>()
        {
            new Category("basic-ability", "Basic Ability 基础能力", new List<StackApp>()
            {
                new StackApp("auth", "Auth", new List<Nav>()
                {
                    new Nav("user", "Users", "/users", 1),
                    new Nav("role-permission", "Role and Permission", "mdi-users", 1, new List<Nav>()
                    {
                        new Nav("role", "Roles", "/roles", 2),
                        new Nav("permission", "Permissions", "/permissions", 2),
                    })
                }),
                new StackApp("pm", "Project Management 项目管理", new List<Nav>()
                {
                    new Nav("all", "All views", "/all-view", 1),
                    new Nav("groups", "Groups", "mdi-users", 1, new List<Nav>()
                    {
                        new Nav("group-1", "Group Name 1", "/group/1", 2),
                        new Nav("group-2", "Group Name 2", "/group/2", 2),
                        new Nav("group-3", "Group Name 3", "/group/3", 2),
                        new Nav("group-4", "Group Name 4", "/group/4", 2),
                        new Nav("group-5", "Group Name 5", "/group/5", 2),
                    })
                }),
                new StackApp("auth2", "Auth", new List<Nav>()
                {
                    new Nav("user", "Users", "/users", 1),
                    new Nav("role-permission", "Role and Permission", "mdi-users", 1, new List<Nav>()
                    {
                        new Nav("role", "Roles", "/roles", 2),
                        new Nav("permission", "Permissions", "/permissions", 2),
                    })
                }),
                new StackApp("pm3", "Project Management 项目管理", new List<Nav>()
                {
                    new Nav("all", "All views", "/all-view", 1),
                    new Nav("groups", "Groups", "mdi-users", 1, new List<Nav>()
                    {
                        new Nav("group-1", "Group Name 1", "/group/1", 2),
                        new Nav("group-2", "Group Name 2", "/group/2", 2),
                        new Nav("group-3", "Group Name 3", "/group/3", 2),
                    })
                }),
            }),
            new Category("basic-ability2", "Basic Ability 基础能力", new List<StackApp>()
            {
                new StackApp("auth", "Auth", new List<Nav>()
                {
                    new Nav("user", "Users", "/users", 1),
                    new Nav("role-permission", "Role and Permission", "mdi-users", 1, new List<Nav>()
                    {
                        new Nav("role", "Roles", "/roles", 2),
                        new Nav("permission", "Permissions", "/permissions", 2),
                        new Nav("bing", "Bing", "https://www.bing.com", 2),
                    })
                }),
                new StackApp("pm", "Project Management 项目管理", new List<Nav>()
                {
                    new Nav("all", "All views", "/all-view", 1),
                    new Nav("groups", "Groups", "mdi-users", 1, new List<Nav>()
                    {
                        new Nav("group-1", "Group Name 1", "/group/1", 2),
                        new Nav("group-2", "Group Name 2", "/group/2", 2),
                        new Nav("group-3", "Group Name 3", "/group/3", 2),
                        new Nav("group-4", "Group Name 4", "/group/4", 2),
                        new Nav("group-5", "Group Name 5", "/group/5", 2),
                        new Nav("group-6", "Group Name 6", "/group/6", 2),
                        new Nav("group-7", "Group Name 7", "/group/7", 2),
                        new Nav("group-8", "Group Name 8", "/group/8", 2),
                        new Nav("group-9", "Group Name 9", "/group/9", 2),
                    })
                }),
            }),
        };

        return Task.FromResult(categories);
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
