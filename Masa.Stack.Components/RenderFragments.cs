namespace Masa.Stack.Components;

public static class RenderFragments
{
    public static RenderFragment UserSelectItem(
        string? avatar,
        string title1,
        string? title2,
        string? subtitle1,
        string? subtitle2)
    {
        ArgumentNullException.ThrowIfNull(title1);

        return builder =>
        {
            if (!string.IsNullOrWhiteSpace(avatar))
            {
                builder.OpenComponent<MListItemAvatar>(0);
                builder.AddAttribute(1, nameof(MAvatar.Size), (StringNumber)40);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(sb =>
                {
                    sb.OpenComponent<MImage>(0);
                    sb.AddAttribute(1, "Src", avatar);
                    sb.CloseComponent();
                }));
                builder.CloseComponent();
            }

            builder.OpenComponent<MListItemContent>(3);
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(sb =>
            {
                sb.OpenComponent<MListItemTitle>(0);
                sb.AddAttribute(1, "Class", "");
                sb.AddAttribute(2, "ChildContent", (RenderFragment)(title =>
                {
                    title.OpenElement(0, "span");
                    title.AddAttribute(1, "Class", "subtitle2 emphasis2--text");
                    title.AddContent(2, title1);
                    title.CloseElement();

                    if (!string.IsNullOrWhiteSpace(title2))
                    {
                        title.OpenComponent<MDivider>(2);
                        title.AddAttribute(3, nameof(MDivider.Vertical), true);
                        title.AddAttribute(4, nameof(MDivider.Style), "height: 12px;");
                        title.AddAttribute(5, nameof(MDivider.Class), "mx-2 my-auto");
                        title.CloseComponent();

                        title.OpenElement(6, "span");
                        title.AddAttribute(7, "Class", "regular2--text");
                        title.AddContent(8, title2);
                        title.CloseElement();
                    }
                }));
                sb.CloseComponent();

                if (!string.IsNullOrWhiteSpace(subtitle1))
                {
                    sb.OpenComponent<MListItemSubtitle>(3);
                    sb.AddAttribute(4, "Class", " regular2--text");
                    sb.AddAttribute(5, "Style", "font-weight: normal");
                    sb.AddAttribute(6, "ChildContent", (RenderFragment)(subtitle =>
                    {
                        subtitle.OpenElement(0, "span");
                        subtitle.AddContent(1, subtitle1);
                        subtitle.CloseElement();

                        if (!string.IsNullOrWhiteSpace(subtitle2))
                        {
                            subtitle.OpenComponent<MDivider>(2);
                            subtitle.AddAttribute(3, nameof(MDivider.Vertical), true);
                            subtitle.AddAttribute(4, nameof(MDivider.Style), "height: 12px;");
                            subtitle.AddAttribute(5, nameof(MDivider.Class), "mx-2 my-auto");
                            subtitle.CloseComponent();

                            subtitle.OpenElement(6, "span");
                            subtitle.AddContent(7, subtitle2);
                            subtitle.CloseElement();
                        }
                    }));
                    sb.CloseComponent();
                }
            }));
            builder.CloseComponent();
        };
    }

    public static RenderFragment GenRequiredLabel(string? label)
    {
        return builder =>
        {
            builder.OpenElement(0, "label");
            builder.AddAttribute(1, "class", "red--text mr-1");
            builder.AddContent(2, "*");
            builder.CloseElement();
            builder.AddContent(3, label);
        };
    }

    public static RenderFragment GenHelpIcon(string tooltip)
    {
        return builder =>
        {
            builder.OpenComponent<SIcon>(0);
            builder.AddAttribute(1, "Tooltip", tooltip);
            builder.AddAttribute(2, "ChildContent",
                (RenderFragment)(cb => cb.AddContent(3, "mdi-help-circle-outline")));
            builder.CloseComponent();
        };
    }
}