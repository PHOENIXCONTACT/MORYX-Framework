#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Identity;
using Moryx.Identity.AccessManagement.Models;

namespace Moryx.Identity.AccessManagement.Controllers
{

    [Authorize(Roles = Roles.SuperAdmin)]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly MoryxUserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPasswordResetService _pwResetService;

        public UsersController(IMapper mapper, MoryxUserManager userManager, IConfiguration configuration, IPasswordResetService passwordResetService)
        {
            _mapper = mapper;
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
            var userModel = _mapper.Map<MoryxUserUpdateModel>(user);
            var pwReset = await _pwResetService.GetPasswordReset(userId);
            if (pwReset != null)
                userModel.PasswordResetToken = pwReset.ResetToken;
            return View(userModel);
        }

        public async Task<IActionResult> GenerateResetToken(MoryxUserUpdateModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound(model.UserName);

            var oldPwReset = await _pwResetService.GetPasswordReset(user.Id);
            if (oldPwReset != null)
                await _pwResetService.RemovePasswordReset(oldPwReset);

            await _pwResetService.GeneratePasswordReset(user.Id);
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