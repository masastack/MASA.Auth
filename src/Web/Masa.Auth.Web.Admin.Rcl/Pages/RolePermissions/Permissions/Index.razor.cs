namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class Index
{
    StringNumber _tab = 0;
    bool _enabled = false;
    List<PermissionDto> _menuPermissions = new();
    List<PermissionDto> _apiPermissions = new();
    List<Guid> _menuActive = new List<Guid>();
    List<Guid> _apiActive = new List<Guid>();
    private List<string> _values = new List<string>
    {
        "foo", "bar"
    };
    private List<string> _items = new List<string>
    {
        "foo", "bar", "fizz", "buzz"
    };
    bool _addApiPermission, _addMenuPermission;

    private string value3 = "";

    public class Item
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public Item(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }

    List<Item> items = new()
    {
        new Item("Foo", "1"),
        new Item("Bar", "2"),
        new Item("Fizz", "3"),
        new Item("Buzz", "4"),
    };

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _menuPermissions = new List<PermissionDto>()
            {
                new PermissionDto() { Name = "菜单1", Code = "1",Id=Guid.NewGuid(),Children = new List<PermissionDto>
                    {
                        new PermissionDto() { Name ="11111",Id=Guid.NewGuid()},
                        new PermissionDto() { Name ="222233",Id=Guid.NewGuid()}
                    }
                },
                new PermissionDto() { Name = "菜单2", Code = "1",Id=Guid.NewGuid(),Children = new List<PermissionDto>
                    {
                        new PermissionDto() { Name ="44444",Id=Guid.NewGuid()},
                        new PermissionDto() { Name ="555555",Id=Guid.NewGuid()}
                    }
                }
            };
            _apiPermissions = new List<PermissionDto>()
            {
                new PermissionDto() { Name = "Api1", Code = "1",Id=Guid.NewGuid(),Children = new List<PermissionDto>
                    {
                        new PermissionDto() { Name ="11111",Id=Guid.NewGuid()},
                        new PermissionDto() { Name ="222233",Id=Guid.NewGuid()}
                    }
                },
                new PermissionDto() { Name = "Api2", Code = "1",Id=Guid.NewGuid(),Children = new List<PermissionDto>
                    {
                        new PermissionDto() { Name ="44444",Id=Guid.NewGuid()},
                        new PermissionDto() { Name ="555555",Id=Guid.NewGuid()}
                    }
                }
            };
            StateHasChanged();
        }
        return base.OnAfterRenderAsync(firstRender);
    }
}
