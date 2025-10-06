// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moryx.Identity.AccessManagement.Models;

namespace Moryx.Identity.AccessManagement.Controllers
{

    [Authorize(Roles = Roles.SuperAdmin)]
    public class PermissionsController : Controller
    {
        private readonly MoryxRoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;

        public PermissionsController(MoryxRoleManager roleManager, IPermissionManager permissionManager)
        {
            _roleManager = roleManager;
            _permissionManager = permissionManager;
        }

        public async Task<ActionResult> Index()
        {
            if (Request.QueryString.HasValue)
            {
                ViewBag.ErrorMessage = Request.Query["viewBagError"].FirstOrDefault();
            }

            var permissions = await _permissionManager.Permissions.Include(p => p.Roles).ToListAsync();
            var model = permissions.Select(p => new PermissionsViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Roles = p.Roles.Select(r => r.Name).ToArray()
            });

            return View(model);
        }

        public async Task<IActionResult> AddPermission(string permission)
        {
            var creationResult = await _permissionManager.CreateAsync(permission);

            if (!creationResult.Succeeded)
                ViewBag.ErrorMessage = creationResult.Errors.First().Description;

            return RedirectToAction("Index", routeValues: new { viewBagError = ViewBag.ErrorMessage });
        }

        public async Task<IActionResult> Delete(string permissionId)
        {
            var deletionResult = await _permissionManager.DeleteAsync(permissionId);

            if (!deletionResult.Succeeded)
                return BadRequest(deletionResult);

            return RedirectToAction("Index");
        }
    }
}

