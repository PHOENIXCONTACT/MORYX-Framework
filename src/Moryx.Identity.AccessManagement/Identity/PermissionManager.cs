// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement
{
    /// <inheritdoc/>
    public class PermissionManager : IPermissionManager
    {
        private readonly MoryxRoleManager _roleManager;
        private readonly MoryxIdentitiesDbContext _identitiesDbContext;

        /// <summary>
        /// Creates a new permission manager
        /// </summary>
        /// <param name="roleManager">Provides available roles in the MORYX AccessManagement</param>
        /// <param name="applicationDbContext">The database context of the MORYX AccessManagement</param>
        public PermissionManager(MoryxRoleManager roleManager, MoryxIdentitiesDbContext applicationDbContext)
        {
            _roleManager = roleManager;
            _identitiesDbContext = applicationDbContext;
        }

        /// <inheritdoc/>
        public IQueryable<Permission> Permissions => _identitiesDbContext.Permissions;

        /// <inheritdoc/>
        public async Task<IdentityResult> CreateAsync(string permission)
        {
            if (string.IsNullOrEmpty(permission))
                return IdentityResult.Failed(new IdentityError { Description = "Permission cannot be empty" });

            var friendlyName = FriendlyName(permission);

            var existing = await _identitiesDbContext.Permissions.FirstOrDefaultAsync(p => p.Name == friendlyName);
            if (existing != null)
                return IdentityResult.Failed(new IdentityError { Description = "Permission already exists" });

            var permissionEntity = new Permission { Name = friendlyName };

            _identitiesDbContext.Permissions.Add(permissionEntity);
            await _identitiesDbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public async Task<IdentityResult> DeleteAsync(string permissionId)
        {
            var existing = await _identitiesDbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (existing == null)
                return IdentityResult.Failed(new IdentityError { Description = "Permission does not exists" });

            _identitiesDbContext.Permissions.Remove(existing);

            await _identitiesDbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public Task<IList<Permission>> FindForRole(string roleName)
        {
            if (roleName == Roles.SuperAdmin)
                return GetSuperAdminPermissions();

            var role = _roleManager.Roles
                .Include(r => r.Permissions)
                .FirstOrDefault(r => r.Name == roleName);

            var permissions = role != null
                ? role.Permissions.ToArray()
                : [];

            return Task.FromResult<IList<Permission>>(permissions);
        }

        /// <inheritdoc/>
        public async Task<IdentityResult> RemoveFromRoleAsync(string roleName, IEnumerable<Permission> permissions)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Role was not found" });

            foreach (var permissionToRemove in permissions)
            {
                var permissionEntry = role.Permissions.FirstOrDefault(p => p.Id == permissionToRemove.Id);
                if (permissionEntry != null)
                    role.Permissions.Remove(permissionEntry);
            }

            await _identitiesDbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public async Task<IdentityResult> UpdatePermissionsAsync(string roleName, IEnumerable<string> permissions)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role was not found" });
            }

            role.Permissions.Clear();

            var permissionEntities = _identitiesDbContext.Permissions.Where(permissionEntity =>
                permissions.Contains(permissionEntity.Name));

            foreach (var permissionEntity in permissionEntities)
            {
                role.Permissions.Add(permissionEntity);
            }

            _identitiesDbContext.Roles.Update(role);

            await _identitiesDbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        /// <inheritdoc/>
        public async Task<IdentityResult> AddToRoleAsync(string roleName, IEnumerable<string> permissions)
        {
            var role = await _identitiesDbContext.Roles.Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Role was not found" });

            var permissionEntities = _identitiesDbContext.Permissions.Where(permissionEntity =>
                permissions.Contains(permissionEntity.Name));

            foreach (var permissionEntity in permissionEntities)
            {
                role.Permissions.Add(permissionEntity);
            }

            _identitiesDbContext.Roles.Update(role);

            await _identitiesDbContext.SaveChangesAsync();

            return IdentityResult.Success;
        }

        private async Task<IList<Permission>> GetSuperAdminPermissions()
        {
            var allPermissions = await _identitiesDbContext.Permissions.ToArrayAsync();
            return allPermissions;
        }

        private static string FriendlyName(string s)
        {
            var map = new Dictionary<char, string> {
                { ' ', "." },
                { 'ä', "ae" },
                { 'ö', "oe" },
                { 'ü', "ue" },
                { 'Ä', "Ae" },
                { 'Ö', "Oe" },
                { 'Ü', "Ue" },
                { 'ß', "ss" }
            };

            var res = s.Aggregate(new StringBuilder(),
                (sb, c) => map.TryGetValue(c, out var r) ? sb.Append(r) : sb.Append(c)
            ).ToString();

            return res;
        }
    }
}
