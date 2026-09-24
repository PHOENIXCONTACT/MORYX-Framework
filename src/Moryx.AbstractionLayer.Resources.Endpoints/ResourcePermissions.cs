// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Permissions used to authorize for the <see cref="ResourceManagementController"/>
/// </summary>
public static class ResourcePermissions
{
    /// <summary>
    /// Prefix used for all permissions of the controller
    /// </summary>
    private const string _prefix = "Moryx.Resources.";

    /// <summary>
    /// Permission for all actions related to viewing the resource instance tree
    /// </summary>
    public const string CanViewTree = _prefix + "CanViewTree";

    /// <summary>
    /// Permission for all actions related to viewing the instance information of a resource
    /// </summary>
    public const string CanViewDetails = _prefix + "CanViewDetails";

    /// <summary>
    /// Permission for all actions related to editing the resource graph and its members
    /// </summary>
    public const string CanEdit = _prefix + "CanEdit";

    /// <summary>
    /// Permission for all actions related to viewing the resource type tree
    /// </summary>
    public const string CanViewTypeTree = _prefix + "CanViewTypeTree";

    /// <summary>
    /// Permission for all actions related to adding one or multiple resources
    /// </summary>
    public const string CanAdd = _prefix + "CanAdd";

    /// <summary>
    /// Permission for all actions related to adding one or multiple resources
    /// </summary>
    public const string CanDelete = _prefix + "CanDelete";

    /// <summary>
    /// Permission for all actions related to invoking a method on a resource
    /// </summary>
    public const string CanInvokeMethod = _prefix + "CanInvokeMethod";
}
