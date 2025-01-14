// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SUrlTextField : IDisposable
{
    public string? _protocol;

    public string Protocol
    {
        get => _protocol ??= "Https";
        set => _protocol = value;
    }

    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    [Parameter]
    public string Value
    {
        get => $"{Protocol}://{TextValue}";
        set
        {
            if (string.IsNullOrEmpty(Value) is false)
            {
                if (value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    _protocol = "https";
                    TextValue = value.TrimStart("https://".ToCharArray());
                }
                else if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    _protocol = "http";
                    TextValue = value.TrimStart("http://".ToCharArray());
                }
                else
                {
                    _protocol = "https";
                    TextValue = "";
                }
            }
            else
            {
                _protocol = "https";
                TextValue = "";
            }
        }
    }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<string>>? ValueExpression { get; set; }

    [Parameter]
    public string Label { get; set; } = "";

    [Parameter]
    public bool Required { get; set; }

    public string TextValue { get; set; } = "";

    protected FieldIdentifier ValueIdentifier { get; set; }

    public async Task ValueTextUpdateAsync(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            value = $"{Protocol}://{value.TrimStart("https://".ToArray()).TrimStart("http://".ToArray())}";
        }
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else
        {
            Value = value;
        }
    }

    public async Task ProtocolUpdateAsync(string protocol)
    {
        if (!string.IsNullOrWhiteSpace(TextValue))
        {
            var value = $"{protocol}://{TextValue}";
            if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
            else Value = value;
        }
    }

    public List<string> Items { get; set; } = new() { "http", "https" };

    public List<string> ErrorMessage { get; set; } = new();

    protected override void OnInitialized()
    {
        if (EditContext is not null && ValueExpression is not null)
        {
            ValueIdentifier = FieldIdentifier.Create(ValueExpression);
            EditContext.OnValidationStateChanged += HandleOnValidationStateChanged;
        }
    }

    protected virtual void HandleOnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        ErrorMessage = EditContext!.GetValidationMessages(ValueIdentifier).ToList();
        base.InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (EditContext != null)
        {
            EditContext.OnValidationStateChanged -= HandleOnValidationStateChanged;
        }
    }
}
