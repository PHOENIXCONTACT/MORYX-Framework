// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Endpoints;

/// <summary>
/// Permissions used to authorize for the <see cref="MaterialManagementController"/>
/// </summary>
public static class MaterialPermissions
{
    /// <summary>
    /// Prefix used for all permissions of the controller
    /// </summary>
    private const string _prefix = "Moryx.Material.";

    public const string CanCreate = _prefix + "CanCreate";

    public const string CanRead = _prefix + "CanRead";

    public const string CanUpdate = _prefix + "CanUpdate";

    public const string CanDelete = _prefix + "CanDelete";
}
