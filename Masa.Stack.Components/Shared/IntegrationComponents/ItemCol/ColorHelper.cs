namespace Masa.Stack.Components;

public static class ColorHelper
{
    private readonly static string[] Colors =
    {
        "amber",
        "blue",
        "blue-grey",
        "brown",
        "cyan",
        "deep-orange",
        "deep-purple",
        "green",
        "grey",
        "indigo",
        "light-blue",
        "light-green",
        "lime",
        "orange",
        "pink",
        "purple",
        "red",
        "teal",
        "yellow",
    };

    public static string GetColor(int index)
    {
        if (Colors.Length > index)
        {
            return Colors[index];
        }

        return string.Empty;
    }
}
