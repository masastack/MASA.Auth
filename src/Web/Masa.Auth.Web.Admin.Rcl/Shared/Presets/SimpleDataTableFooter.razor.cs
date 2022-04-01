﻿namespace Masa.Auth.Web.Admin.Rcl.Shared.Presets;

public partial class SimpleDataTableFooter
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public bool TotalShow { get; set; }

    [Parameter]
    public List<int> PageSizes { get; set; } = new List<int> { 10, 20, 30 };

    [EditorRequired]
    [Parameter]
    public int Page { get; set; } = 1;

    [EditorRequired]
    [Parameter]
    public int PageSize { get; set; } = 10;

    [Parameter]
    public string PageSizeText { get; set; } = "行/页";

    [EditorRequired]
    [Parameter]
    public long Total { get; set; }

    [Parameter]
    public string TotalStringFormat { get; set; } = "总计：{0}";

    [Parameter]
    public string PreIcon { get; set; } = "mdi-chevron-left";

    [Parameter]
    public string NexIcon { get; set; } = "mdi-chevron-right";

    [Parameter]
    public EventCallback<int> OnSelectedPageSize { get; set; }

    [Parameter]
    public EventCallback OnNext { get; set; }

    [Parameter]
    public EventCallback OnPrevious { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }
}
