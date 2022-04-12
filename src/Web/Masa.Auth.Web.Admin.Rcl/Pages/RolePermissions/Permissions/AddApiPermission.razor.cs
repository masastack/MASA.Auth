namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class AddApiPermission
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public EventCallback<ApiPermissionDetailDto> OnSubmit { get; set; }

    [EditorRequired]
    [Parameter]
    public List<SelectItemDto<PermissionTypes>> PermissionTypes { get; set; } = new();

    ApiPermissionDetailDto _apiPermissionDetailDto = new();

    private async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(_apiPermissionDetailDto);
        }
    }

    private List<string> _values = new List<string>
    {
        "foo", "bar"
    };
    private List<string> _items = new List<string>
    {
        "foo", "bar", "fizz", "buzz"
    };

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
}
