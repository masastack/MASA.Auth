// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.Auth.Contracts.Admin;
global using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
global using Masa.Auth.Domain.DynamicRoles.Aggregates;
global using Masa.Auth.Domain.DynamicRoles.Repositories;
global using Masa.Auth.Domain.GlobalNavs.Aggregates;
global using Masa.Auth.Domain.GlobalNavs.Repositories;
global using Masa.Auth.Domain.Logs.Aggregates;
global using Masa.Auth.Domain.Logs.Repositories;
global using Masa.Auth.Domain.Organizations.Aggregates;
global using Masa.Auth.Domain.Organizations.Repositories;
global using Masa.Auth.Domain.Permissions.Aggregates;
global using Masa.Auth.Domain.Permissions.Repositories;
global using Masa.Auth.Domain.Sso.Aggregates;
global using Masa.Auth.Domain.Sso.Repositories;
global using Masa.Auth.Domain.Subjects.Aggregates;
global using Masa.Auth.Domain.Subjects.Repositories;
global using Masa.Auth.Domain.Webhooks.Aggregates;
global using Masa.Auth.Domain.Webhooks.Repositories;
global using Masa.Auth.EntityFrameworkCore.PostgreSql.Converters;
global using Masa.Auth.Service.Admin.Infrastructure;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Repositories;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text.Json;
