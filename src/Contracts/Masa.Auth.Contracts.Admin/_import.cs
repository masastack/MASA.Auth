// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using FluentValidation;
global using Magicodes.ExporterAndImporter.Core;
global using Masa.Auth.Contracts.Admin.Infrastructure.Converters;
global using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Auth.Contracts.Admin.Infrastructure.Enums;
global using Masa.Auth.Contracts.Admin.Infrastructure.Utils;
global using Masa.Auth.Contracts.Admin.Subjects;
global using Masa.BuildingBlocks.Authentication.Oidc.Domain.Enums;
global using Masa.BuildingBlocks.Authentication.Oidc.Models.Enums;
global using Masa.BuildingBlocks.SearchEngine.AutoComplete;
global using Microsoft.AspNetCore.Http;
global using SixLabors.Fonts;
global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Drawing.Processing;
global using SixLabors.ImageSharp.PixelFormats;
global using SixLabors.ImageSharp.Processing;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Enum;

