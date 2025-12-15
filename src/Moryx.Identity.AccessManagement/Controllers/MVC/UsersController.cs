// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Identity.AccessManagement.Models;

namespace Moryx.Identity.AccessManagement.Controllers
{
    [Authorize(Roles = Roles.SuperAdmin)]
    public class UsersController : Controller
    {
        private readonly MoryxUserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPasswordResetService _pwResetService;

        public UsersController(MoryxUserManager userManager, IConfiguration configuration, IPasswordResetService passwordResetService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _pwResetService = passwordResetService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userModel = ModelConverter.GetUserUpdateModelFromUser(user);
            var pwReset = await _pwResetService.GetPasswordResetAsync(userId);
            if (pwReset != null)
                userModel.PasswordResetToken = pwReset.ResetToken;
            return View(userModel);
        }

        public async Task<IActionResult> GenerateResetToken(MoryxUserUpdateModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound(model.UserName);

            var oldPwReset = await _pwResetService.GetPasswordResetAsync(user.Id);
            if (oldPwReset != null)
                await _pwResetService.RemovePasswordResetAsync(oldPwReset);

            await _pwResetService.GeneratePasswordResetAsync(user.Id);
            return RedirectToAction("Edit", new { userId = user.Id });

        }
        public async Task<IActionResult> Update(MoryxUserUpdateModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound(model.UserName);

            user.Email = model.Email;
            user.Firstname = model.FirstName;
            user.LastName = model.LastName;

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(userId);

            if (User.Identity.Name != user.UserName)
            {
                var result = await _userManager.DeleteAsync(user);
            }
            else
            {
                return BadRequest("You can not delete yourself.");
            }

            return RedirectToAction("Index");
        }
    }
}
