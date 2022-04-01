namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class Index
{
    private List<string> _values = new List<string>
    {
        "foo", "bar"
    };
    private List<string> _items = new List<string>
    {
        "foo", "bar", "fizz", "buzz"
    };

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
}
