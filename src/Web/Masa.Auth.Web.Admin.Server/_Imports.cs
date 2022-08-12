// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.Auth.ApiGateways.Caller;
global using Masa.Auth.Contracts.Admin.Subjects.Validator;
global using Masa.Auth.Service.Admin;
global using Masa.Auth.Web.Admin.Rcl;
global using Masa.Auth.Web.Admin.Rcl.Global;
global using Masa.Auth.Web.Admin.Rcl.Shared;
global using Masa.Auth.Web.Admin.Server;
global using Masa.Blazor;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.Stack.Components;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Hosting.StaticWebAssets;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.IdentityModel.Protocols.OpenIdConnect;
global using System.Diagnostics;
global using System.Security.Cryptography.X509Certificates;
global using Masa.Auth.Contracts.Admin.Infrastructure.Extensions;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
