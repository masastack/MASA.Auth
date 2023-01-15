// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.Auth.ApiGateways.Caller;
global using Masa.Auth.Contracts.Admin.Subjects.Validator;
global using Masa.Auth.Web.Admin.Rcl.Global;
global using Masa.Auth.Web.Admin.Rcl.Shared;
global using Masa.Blazor;
global using Masa.BuildingBlocks.StackSdks.Config;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;
global using Masa.Contrib.StackSdks.Config;
global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Stack.Components;
global using Masa.Stack.Components.Extensions.OpenIdConnect;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Hosting.StaticWebAssets;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.IdentityModel.Logging;
global using Microsoft.IdentityModel.Protocols.OpenIdConnect;
global using System.Diagnostics;
global using System.Security.Cryptography.X509Certificates;
