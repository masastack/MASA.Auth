// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultUploadImage : UploadImage
{
    [Inject]
    public AuthCaller? AuthCaller { get; set; }

    public OssService OssService => AuthCaller!.OssService;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Avatar = true;
        Size = 120;
    }

    public override async Task UploadAsync()
    {
        var paramter = await OssService.GetSecurityTokenAsync();
        OnInputFileUpload = FileUploadCallBack.CreateCallback("UploadImage", paramter);
        await base.UploadAsync();
    }
}

