﻿using Masa.Auth.ApiGateways.Caller.Request.Organizations;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization.ViewModels;

public class CreateDepartmentModel : AddDepartmentRequest
{
    public string ParentName { get; set; } = "";
}

