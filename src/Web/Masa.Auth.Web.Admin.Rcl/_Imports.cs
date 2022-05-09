// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using BlazorComponent;
global using BlazorComponent.I18n;
global using Mapster;
global using Masa.Auth.ApiGateways.Caller.Services.Organizations;
global using Masa.Auth.ApiGateways.Caller.Services.Permissions;
global using Masa.Auth.ApiGateways.Caller.Services.Projects;
global using Masa.Auth.ApiGateways.Caller.Services.Sso;
global using Masa.Auth.ApiGateways.Caller.Services.Subjects;
global using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Contracts.Admin.Organizations;
global using Masa.Auth.Contracts.Admin.Permissions;
global using Masa.Auth.Contracts.Admin.Projects;
global using Masa.Auth.Contracts.Admin.Sso;
global using Masa.Auth.Contracts.Admin.Subjects;
global using Masa.Auth.Web.Admin.Rcl.Data.Shared.Favorite;
global using Masa.Auth.Web.Admin.Rcl.Global.Config;
global using Masa.Auth.Web.Admin.Rcl.Global.Nav.Model;
global using Masa.Auth.Web.Admin.Rcl.Pages.Component;
global using Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions.ViewModels;
global using Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin.Model;
global using Masa.Blazor;
global using Masa.Stack.Components.Models;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using System.Net.Http.Json;
global using System.Reflection;
global using System.Text.Json;
