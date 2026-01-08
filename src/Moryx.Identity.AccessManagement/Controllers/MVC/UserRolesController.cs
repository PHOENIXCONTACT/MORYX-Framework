// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Models;

namespace Moryx.Identity.AccessManagement.Controllers;

[Authorize(Roles = Roles.SuperAdmin)]
public class UserRolesController : Controller
{
    private readonly SignInManager<MoryxUser> _signInManager;
    private readonly MoryxUserManager _userManager;
    private readonly MoryxRoleManager _roleManager;

    public UserRolesController(MoryxUserManager userManager, SignInManager<MoryxUser> signInManager, MoryxRoleManager roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string userId)
    {
        var viewModel = new List<UserRolesViewModel>();
        var user = await _userManager.FindByIdAsync(userId);
        foreach (var role in _roleManager.Roles.ToList())
        {
            var userRolesViewModel = new UserRolesViewModel
            {
                RoleName = role.Name
            };
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }
            viewModel.Add(userRolesViewModel);
        }
        var model = new ManageUserRolesViewModel
        {
            UserId = userId,
            UserName = user.UserName,
            UserRoles = viewModel
        };

        return View(model);
    }

    public async Task<IActionResult> Update(string id, ManageUserRolesViewModel model)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, model.UserRoles.Where(x => x.Selected).Select(y => y.RoleName));

            var currentUser = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(currentUser);

            TempData["SuccessMessage"] = "Role has been updated successfully!";
        }
        catch
        {
            TempData["ErrorMessage"] = "Updating the role failed with an unexpected error!";
        }

        return RedirectToAction("Index", new { userId = id });
    }
}