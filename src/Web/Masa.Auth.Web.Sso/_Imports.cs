// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using BlazorComponent.I18n;
global using IdentityModel;
global using IdentityServer4;
global using IdentityServer4.Events;
global using IdentityServer4.Extensions;
global using IdentityServer4.Models;
global using IdentityServer4.Services;
global using IdentityServer4.Stores;
global using IdentityServer4.Test;
global using Masa.Auth.Web.Sso.Global;
global using Masa.Auth.Web.Sso.Infrastructure;
global using Masa.Auth.Web.Sso.Pages.Account.Login;
global using Masa.Blazor;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Text;
global using System.Text.Json;
