namespace Masa.Stack.Components.Logins;

public class SDefaultSheet : MSheet
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Height ??= 684;
        MaxWidth ??= 512;

        Class ??= string.Empty;
        if (!Class.Contains(" rounded-5"))
        {
            Class += " rounded-5";
        }
    }
}
