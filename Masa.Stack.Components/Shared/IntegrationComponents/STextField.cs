namespace Masa.Stack.Components;

public class STextField<TValue> : MTextField<TValue>
{
    [Inject]
    public I18n I18n { get; set; } = default!;

    [Parameter]
    public bool Small { get; set; }

    [Parameter]
    public bool Medium { get; set; }

    [Parameter]
    public bool Large { get; set; }

    [Parameter]
    public string? Tooltip { get; set; }

    [Parameter]
    public Action<DefaultTextfieldAction>? Action { get; set; }

    [Parameter]
    public bool AutoLabel { get; set; } = true;

    private DefaultTextfieldAction InternalAction { get; set; } = new();

    private RenderFragment? _requiredLabelContent;
    private RenderFragment? _tooltipContent;
    private RenderFragment? _actionContent;
    private string? _fieldName;
    
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Dense = true;
        HideDetails = "auto";
        Outlined = true;

        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // TODO: refactor the following code about css
        Class ??= "";
        if (Large is false && Small is false) Medium = true;
        if (Dense is true)
        {
            if (Large)
            {
                Height = 56;
                if (Class.Contains("m-input--dense-56") is false)
                {
                    Class += " m-input--dense-56";
                }
            }
            else if (Medium)
            {
                Height = 48;
                if (Class.Contains("m-input--dense-48") is false)
                {
                    Class += " m-input--dense-48";
                }
            }
            else if (Small)
            {
                Height = 40;
                if (Class.Contains("m-input--dense-40") is false)
                {
                    Class += " m-input--dense-40";
                }
            }
        }

        if (Action is not null)
        {
            Action.Invoke(InternalAction);
        
            _actionContent ??= builder =>
            {
                builder.OpenComponent<MDivider>(0);
                builder.AddAttribute(1, "Vertical", true);
                builder.CloseComponent();
        
                builder.OpenComponent<SAutoLoadingButton>(2);
                builder.AddAttribute(3, "Text", InternalAction.Text);
                builder.AddAttribute(4, "Disabled", InternalAction.Disabled);
                builder.AddAttribute(5, "Color", InternalAction.Color);
                builder.AddAttribute(6, "Style", "border:none;border-radius: 0 8px 8px 0 !important;height:100%;");
                builder.AddAttribute(7, "DisableLoading", InternalAction.DisableLoding);
                builder.AddAttribute(8, "OnClick", EventCallback.Factory.Create<MouseEventArgs>(this, InternalAction.OnClick));
                builder.AddAttribute(9, "ChildContent", (RenderFragment)(cb => cb.AddContent(9, InternalAction.Content)));
                builder.CloseComponent();
            };
        
            AppendContent = _actionContent;
        }

        if (!string.IsNullOrWhiteSpace(Tooltip) && AppendOuterContent == default)
        {
            AppendOuterContent = _tooltipContent ??= RenderFragments.GenHelpIcon(Tooltip);
        }

        if (Required && LabelContent == default)
        {
            LabelContent = _requiredLabelContent ??= RenderFragments.GenRequiredLabel(Label);
        }

        if (string.IsNullOrEmpty(Label) && AutoLabel && ValueExpression is not null)
        {
            var fieldName = _fieldName ??= ValueExpression.GetFieldName();
            Label = I18n.T(fieldName);
        }
    }
}

public class DefaultTextfieldAction
{
    public string Content { get; set; }

    public string Color { get; set; } = "primary";

    public bool Disabled { get; set; }

    public bool DisableLoding { get; set; }

    public bool Text { get; set; }

    public Func<MouseEventArgs, Task> OnClick { get; set; }
}
