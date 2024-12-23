// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

/// <summary>
/// OpenId Connect subject types.
/// </summary>
public enum SubjectTypes
{
    /// <summary>
    /// global - use the native subject id
    /// </summary>
    Global = 0,

    /// <summary>
    /// ppid - scope the subject id to the client
    /// </summary>
    Ppid = 1
}

/// <summary>
/// Content Security Policy Level
/// </summary>
public enum CspLevel
{
    /// <summary>
    /// Level 1
    /// </summary>
    One = 0,

    /// <summary>
    /// Level 2
    /// </summary>
    Two = 1
}
