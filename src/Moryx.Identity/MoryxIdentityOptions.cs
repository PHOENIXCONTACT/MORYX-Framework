// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authentication;

namespace Moryx.Identity;

/// <summary>
/// Options class provides information needed to control MORYX Identity Authentication handler behavior
/// </summary>
public class MoryxIdentityOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Name to be used to configure the MoryxIdentity authentication scheme
    /// </summary>
    public const string MoryxIdentity = "MoryxIdentity";

    /// <summary>
    /// Gets or sets the url at which user permissions are requested remotely
    /// </summary>
    public string BaseAddress { get; set; }

    /// <summary>
    /// Gets the url to which the request is posted
    /// </summary>
    public string RequestUri { get; } = "/api/auth/user/permissions";

    /// <summary>
    /// Uri to retrieve a new token using a refresh token
    /// </summary>
    public string RefreshTokenUri { get; } = "/api/auth/RefreshToken";
}