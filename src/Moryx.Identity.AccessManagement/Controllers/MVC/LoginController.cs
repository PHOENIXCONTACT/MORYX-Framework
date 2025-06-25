#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Identity;
using Moryx.Identity.AccessManagement.Models;

namespace Moryx.Identity.AccessManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly MoryxUserManager _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPasswordResetService _pwResetService;

        public LoginController(MoryxUserManager userManager,
            ITokenService tokenService, IMapper mapper, IConfiguration configuration, IPasswordResetService passwordResetService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _configuration = configuration;
            _pwResetService = passwordResetService;
        }

        public IActionResult Index()
        {
            if (Request.QueryString.HasValue)
            {
                ViewBag.ReturnUrl = Request.Query["returnUrl"].FirstOrDefault();
                ViewBag.ErrorMessage = Request.Query["viewBagError"].FirstOrDefault();
            }
            ViewBag.ShowLoginViaMicrosoft = _configuration.GetSection("EntraId").Exists();
            return View();
        }

        public async Task<IActionResult> Login(UserLoginModel userLoginModel, [FromQuery] string returnUrl)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return Forbid("No valid license found!");
#endif

            var user = await _userManager.FindByNameAsync(userLoginModel.UserName);
            if (user is null)
                return ReturnIndexWithError(returnUrl);

            var passwordCorrect = await _userManager.CheckPasswordAsync(user, userLoginModel.Password);
            if (!passwordCorrect)
                return ReturnIndexWithError(returnUrl);

            var tokenResult = await _tokenService.GenerateToken(user);
            HttpContext.Response.Cookies.SetJwtCookie(tokenResult, user);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(Request.Query["returnUrl"]);

            if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin))
                return RedirectToAction("Index", "Users");
            return RedirectToAction("Index", "Home");
        }

        private IActionResult ReturnIndexWithError(string returnUrl)
        {
            ViewBag.ErrorMessage = "UserName or Password incorrect!";

            if (returnUrl != null)
                return RedirectToAction("Index", "Login", new { returnUrl = returnUrl, viewBagError = ViewBag.ErrorMessage });

            return RedirectToAction("Index", "Login", new { viewBagError = ViewBag.ErrorMessage });
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> RegisterExecute(MoryxUserRegisterModel userModel)
        {
            var user = _mapper.Map<MoryxUserRegisterModel, MoryxUser>(userModel);

            var userCreateResult = await _userManager.CreateAsync(user, userModel.Password);
            if (userCreateResult.Succeeded)
                return RedirectToAction("Index", "Home");

            ViewBag.ErrorMessage = userCreateResult.Errors.First().Description;
            return View("Register");
        }

        public IActionResult Logout()
        {
            var token = Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
            _tokenService.InvalidateRefreshToken(token).Wait();
            HttpContext.Response.Cookies.RemoveJwtCookie(_configuration["CookieDomain"]);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ResetView([FromQuery] string userName, [FromQuery] string resetToken, [FromQuery] string returnUrl = null)
        {
            var resetModel = new MoryxUserResetPasswordModel
            {
                ResetToken = resetToken ?? "",
                UserName = userName ?? ""
            };
            ViewBag.ReturnUrl = returnUrl;

            return View(model: resetModel);
        }

        public async Task<IActionResult> ResetPassword(MoryxUserResetPasswordModel resetModel)
        {
            var user = await _userManager.FindByNameAsync(resetModel.UserName);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found";
                return View("ResetView", resetModel);
            }

            var pwReset = await _pwResetService.GetPasswordReset(user.Id);
            if (pwReset == null || !pwReset.ResetToken.Equals(resetModel.ResetToken, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Invalid reset password token";
                return View("ResetView", resetModel);
            }
            else if (pwReset.ExpiryTime < DateTime.UtcNow)
            {
                ViewBag.ErrorMessage = "The reset password token has expired";
                return View("ResetView", resetModel);
            }

            await _userManager.RemovePasswordAsync(user);

            var addPasswordResult = await _userManager.AddPasswordAsync(user, resetModel.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                ViewBag.ErrorMessage = addPasswordResult.Errors.First().Description;
                return View("ResetView", resetModel);
            }
            await _pwResetService.RemovePasswordReset(pwReset);
            await _userManager.UpdateAsync(user);

            var returnUrl = Request?.Query["returnUrl"].FirstOrDefault();

            return RedirectToAction("Index", new { returnUrl = returnUrl });
        }
    }
}
