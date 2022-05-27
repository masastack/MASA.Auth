// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultUploadImage : UploadImage
{
    private OssService OssService => AuthCaller.OssService;

    protected override void OnInitialized()
    {
        base.OnInitialized();        
    }

    public override async Task UploadAsync()
    {
        var paramter = await OssService.GetAccessTokenAsync();
        OnInputFileUpload = FileUploadCallBack.CreateCallback("UploadImage", paramter);
        await base.UploadAsync();
    }
}

