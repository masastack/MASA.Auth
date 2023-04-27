// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using AspNet.Security.OAuth.Apple;
global using AspNet.Security.OAuth.GitHub;
global using AspNet.Security.OAuth.Weixin;
global using Masa.Auth.Security.OAuth.Providers;
global using Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;
global using Masa.Auth.Security.OAuth.Providers.Middlewares;
global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication.OAuth;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
