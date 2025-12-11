// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moryx.Identity.AccessManagement.Models;

namespace Moryx.Identity.AccessManagement.Controllers
{

    [Authorize(Roles = Roles.SuperAdmin)]
    public class RolePermissionsController : Controller
    {
        private readonly MoryxRoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;

        public RolePermissionsController(MoryxRoleManager roleManager, IPermissionManager permissionManager)
        {
            _roleManager = roleManager;
            _permissionManager = permissionManager;
        }

        public async Task<ActionResult> Index(string roleId)
        {
            var model = new RolePermissionsViewModel();
            var allPermissions = _permissionManager.Permissions.Select(p => p.Name).ToList();

            var role = await _roleManager.FindByIdAsync(roleId);
            model.RoleId = roleId;
            model.RoleName = role.Name;

            var rolePermissions = await _permissionManager.FindForRoleAsync(role.Name);

            var rolePermissionValues = rolePermissions.Select(a => a.Name).ToList();
            var authorizedPermissions = allPermissions.Intersect(rolePermissionValues).ToList();

            model.Permissions = new List<PermissionViewModel>(allPermissions.Count);
            foreach (var permission in allPermissions)
            {
                var permissionViewModel = new PermissionViewModel { Value = permission };

                if (authorizedPermissions.Any(authorized => authorized == permission))
                    permissionViewModel.Selected = true;

                model.Permissions.Add(permissionViewModel);
            }

            return View(model);
        }

        public async Task<IActionResult> Update(RolePermissionsViewModel model)
        {
            try
            {
                var permissions = await _permissionManager.FindForRoleAsync(model.RoleName);
                await _permissionManager.RemoveFromRoleAsync(model.RoleName, permissions);

                var selectedPermissions = model.Permissions.Where(a => a.Selected).Select(p => p.Value).ToList();

                await _permissionManager.AddToRoleAsync(model.RoleName, selectedPermissions);

                TempData["SuccessMessage"] = "Permissions has been updated successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Permission update was failed with an unexpected error!";
            }

            return RedirectToAction("Index", new { roleId = model.RoleId });
        }
    }
}

