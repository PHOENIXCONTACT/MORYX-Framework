// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Controllers
{

    [Authorize(Roles = Roles.SuperAdmin)]
    public class RolesController : Controller
    {
        private readonly MoryxRoleManager _roleManager;

        public RolesController(MoryxRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.SuccessMessage = (string)TempData["SuccessMessage"];
            ViewBag.ErrorMessage = (string)TempData["ErrorMessage"];

            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    TempData["ErrorMessage"] = $"A role '{roleName}' already exists";
                }
                else
                {
                    await _roleManager.CreateAsync(new MoryxRole(roleName.Trim()));
                    TempData["SuccessMessage"] = $"Successfully created role '{roleName}'";
                }
            }
            else
            {
                TempData["ErrorMessage"] = $"Please enter a role name";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                TempData["ErrorMessage"] = $"Could not find a role with ID '{roleId}'";
            }
            else
            {
                await _roleManager.DeleteAsync(role);
                TempData["SuccessMessage"] = $"Successfully deleted role '{role.Name}'";
            }

            return RedirectToAction("Index");
        }
    }
}
