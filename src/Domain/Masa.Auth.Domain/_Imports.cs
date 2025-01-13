// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.Auth.Contracts.Admin;
global using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
global using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Contracts.Admin.Infrastructure.Extensions;
global using Masa.Auth.Contracts.Admin.Infrastructure.Utils;
global using Masa.Auth.Contracts.Admin.Permissions;
global using Masa.Auth.Contracts.Admin.Sso;
global using Masa.Auth.Contracts.Admin.Subjects;
global using Masa.Auth.Contracts.Admin.Webhooks;
global using Masa.Auth.Domain.GlobalNavs.Aggregates;
global using Masa.Auth.Domain.Logs.Aggregates;
global using Masa.Auth.Domain.Organizations.Aggregates;
global using Masa.Auth.Domain.Permissions.Aggregates;
global using Masa.Auth.Domain.Sso.Aggregates;
global using Masa.Auth.Domain.Subjects.Aggregates;
global using Masa.Auth.Domain.Webhooks.Aggregates;
global using Masa.BuildingBlocks.Data.Contracts;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Ddd.Domain.Values;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.Utils.Security.Cryptography;
global using Mapster;
global using Microsoft.AspNetCore.Identity;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq.Expressions;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Masa.Auth.Contracts.Subjects;