// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultDataTable<TItem> : MDataTable<TItem>
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        HideDefaultFooter = true;
        Class += "default table-border-none";
    }
}

