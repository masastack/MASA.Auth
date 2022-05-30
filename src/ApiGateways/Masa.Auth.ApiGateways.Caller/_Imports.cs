// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
global using Masa.Utils.Caller.Core;
global using Masa.Utils.Caller.HttpClient;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using System.Reflection;
global using System.Text.Json;