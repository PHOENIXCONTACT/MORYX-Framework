// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Products.Endpoints;

/// <summary>
/// Permissions used to authorize for the <see cref="ProductManagementController"/>
/// </summary>
public static class ProductPermissions
{
    /// <summary>
    /// Prefix used for all permissions of the controller
    /// </summary>
    private const string _prefix = "Moryx.Products.";

    /// <summary>
    /// Permission for all actions related to viewing one or multiple product types
    /// </summary>
    public const string CanViewTypes = _prefix + "CanViewTypes";

    /// <summary>
    /// Permission for all actions related to creating and editing recipes
    /// </summary>
    public const string CanCreateAndEditRecipes = _prefix + "CanCreateAndEditRecipes";

    /// <summary>
    /// Permission for all actions related to editing one or multiple product types
    /// </summary>
    public const string CanEditType = _prefix + "CanEditType";

    /// <summary>
    /// Permission for all actions related to duplicating one or multiple product types
    /// </summary>
    public const string CanDuplicateType = _prefix + "CanDuplicateType";

    /// <summary>
    /// Permission for all actions related to see and execute a product importer
    /// </summary>
    public const string CanImport = _prefix + "CanImport";

    /// <summary>
    /// Permission for all actions related to deleting one or multiple product types
    /// </summary>
    public const string CanDeleteType = _prefix + "CanDeleteType";

    /// <summary>
    /// Permission for all actions related to viewing one or multiple product instances
    /// </summary>
    public const string CanViewInstances = _prefix + "CanViewInstances";

    /// <summary>
    /// Permission for all actions related to creating one or multiple product instances
    /// </summary>
    public const string CanCreateInstances = _prefix + "CanCreateInstances";
}