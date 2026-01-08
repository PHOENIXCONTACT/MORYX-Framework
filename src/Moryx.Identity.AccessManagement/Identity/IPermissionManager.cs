// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement;

/// <summary>
/// Provides the APIs for managing permissions in a persistence store.
/// </summary>
public interface IPermissionManager
{
    /// <summary>
    /// The permissions available in the store.
    /// </summary>
    IQueryable<Permission> Permissions { get; }

    /// <summary>
    /// Create a new permission in the store.
    /// </summary>
    /// <param name="permission">The name of the created permission.</param>
    /// <returns>An <see cref="IdentityResult"/> of the Create operation.</returns>
    Task<IdentityResult> CreateAsync(string permission);

    /// <summary>
    /// Deletes a permission from the store.
    /// </summary>
    /// <param name="permissionId">The ID of the permission to be deleted.</param>
    /// <returns>An <see cref="IdentityResult"/> of the Delete operation.</returns>
    Task<IdentityResult> DeleteAsync(string permissionId);

    /// <summary>
    /// Returns a list of <see cref="Permission"/>s that are assigne the the role with the specified role name.
    /// </summary>
    /// <param name="roleName">The name of the role the permissions should be retrieved for.</param>
    Task<IList<Permission>> FindForRoleAsync(string roleName);

    /// <summary>
    /// Assigns the given <paramref name="permissions"/> to the role with the specified role name.
    /// </summary>
    /// <param name="roleName">The name of the role the permissions should be assigned to.</param>
    /// <param name="permissions">The list of permissions to be added to the role.</param>
    /// <returns>An <see cref="IdentityResult"/> of the AddToRole operation.</returns>
    Task<IdentityResult> AddToRoleAsync(string roleName, IEnumerable<string> permissions);

    /// <summary>
    /// Removes the given <paramref name="permissions"/> from the role with the specified role name.
    /// </summary>
    /// <param name="roleName">The name of the role the permissions should be removed from.</param>
    /// <param name="permissions">The list of permissions to be removed from the role.</param>
    /// <returns>An <see cref="IdentityResult"/> of the RemoveFromRole operation.</returns>
    Task<IdentityResult> RemoveFromRoleAsync(string roleName, IEnumerable<Permission> permissions);

    /// <summary>
    /// Updates the set of permissions assigned to the role with the specified role name to match the 
    /// given <paramref name="permissions"/>.
    /// </summary>
    /// <param name="roleName">The name of the role the permissions should be updated for.</param>
    /// <param name="permissions">The list of permissions to be removed from the role.</param>
    /// <returns>An <see cref="IdentityResult"/> of the RemoveFromRole operation.</returns>
    Task<IdentityResult> UpdatePermissionsAsync(string roleName, IEnumerable<string> permissions);
}