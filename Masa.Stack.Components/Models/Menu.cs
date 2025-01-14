namespace Masa.Stack.Components.Models;

public class Menu
{
    string? _fullTitle;

    public string Code { get; set; }

    public string Title { get; set; }

    public string FullTitle
    {
        get
        {
            if (_fullTitle is null)
            {
                _fullTitle = Parent is null ? Title : $"{Parent.FullTitle} {Title}";
            }
            return _fullTitle;
        }
    }

    public string? Icon { get; set; }

    public string? InheritedIcon
    {
        get
        {
            if (string.IsNullOrEmpty(Icon))
            {
                return Parent is null ? Icon : Parent.InheritedIcon;
            }
            return Icon;
        }
    }

    public string? Url { get; set; }

    public Menu? Parent { get; set; }

    public Menu(string code, string title, string? icon, string? url, Menu? parent)
    {
        Code = code;
        Title = title;
        Icon = icon;
        Url = url;
        Parent = parent;
    }
}
