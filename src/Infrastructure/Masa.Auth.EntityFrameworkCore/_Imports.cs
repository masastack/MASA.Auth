﻿global using Microsoft.EntityFrameworkCore;
global using System.Reflection;
global using Masa.Auth.Domain.GlobalNavs.Aggregates;
global using Masa.Auth.Domain.Logs.Aggregates;
global using Masa.Auth.Domain.Organizations.Aggregates;
global using Masa.Auth.Domain.Permissions.Aggregates;
global using Masa.Auth.Domain.Sso.Aggregates;
global using Masa.Auth.Domain.Subjects.Aggregates;
global using Masa.Auth.Domain.Webhooks.Aggregates;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;
global using Masa.Contrib.Authentication.OpenIdConnect.EFCore.Repositories;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.Extensions.Configuration;
global using Masa.Auth.Domain.Sso.Repositories;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Auth.Contracts.Admin;
global using Masa.Auth.Domain.Organizations.Repositories;
global using System.Linq.Expressions;
global using Masa.Auth.Domain.GlobalNavs.Repositories;
global using Masa.Auth.Domain.Permissions.Repositories;
global using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
global using Masa.Auth.Domain.Subjects.Repositories;
global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Microsoft.Extensions.DependencyInjection;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Domain.Logs.Repositories;
global using Masa.Auth.Service.Admin.Infrastructure;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Microsoft.Extensions.Logging;
global using Masa.Auth.Domain.Webhooks.Repositories;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
global using System.Text.Json;
global using Masa.Auth.Domain.DynamicRoles.Aggregates;
global using Masa.Auth.Domain.DynamicRoles.Repositories;