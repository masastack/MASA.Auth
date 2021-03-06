// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using BlazorComponent.I18n;
global using HealthChecks.UI.Client;
global using IdentityModel;
global using IdentityServer4;
global using IdentityServer4.EntityFramework.Services;
global using IdentityServer4.Events;
global using IdentityServer4.Extensions;
global using IdentityServer4.Models;
global using IdentityServer4.Services;
global using IdentityServer4.Stores;
global using IdentityServer4.Test;
global using IdentityServer4.Validation;
global using Mapster;
global using Masa.Auth.Web.Sso;
global using Masa.Auth.Web.Sso.Global;
global using Masa.Auth.Web.Sso.Global.Config;
global using Masa.Auth.Web.Sso.Infrastructure;
global using Masa.Auth.Web.Sso.Infrastructure.Services;
global using Masa.Auth.Web.Sso.Infrastructure.Stores;
global using Masa.Auth.Web.Sso.Infrastructure.Validation;
global using Masa.Auth.Web.Sso.Pages.Account.Login;
global using Masa.Blazor;
global using Masa.BuildingBlocks.BasicAbility.Auth;
global using Masa.BuildingBlocks.BasicAbility.Pm.Model;
global using Masa.BuildingBlocks.Identity.IdentityModel;
global using Masa.Contrib.Authentication.Oidc.Cache.Storage;
global using Masa.Utils.Caching.Redis.Models;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Server;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.JSInterop;
global using System.Collections.Concurrent;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.Security.Claims;
global using System.Security.Cryptography.X509Certificates;
global using System.Text;
global using System.Text.Json;
