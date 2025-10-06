// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Models;
using Moryx.Identity.Models;

namespace Moryx.Identity.AccessManagement.Controllers
{
    /// <summary>
    /// API controller to provide all the available actions that are otherwise provided through the MVC controllers
    /// of the AccessManagement
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MoryxUserManager _userManager;
        private readonly MoryxRoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates a new instance  of the <see cref="AuthController"/>.
        /// </summary>
        /// <param name="mapper">An AutoMapper for user handling</param>
        /// <param name="userManager">The <see cref="MoryxUserManager"/> for MORYX users</param>
        /// <param name="roleManager">The <see cref="MoryxRoleManager"/> for MORYX roles in the AccessManagement</param>
        /// <param name="permissionManager">The permission manager used by the AccessManagement</param>
        /// <param name="tokenService">A token service for handling the JWTs</param>
        /// <param name="configuration">Configuration settings mainly for the cookies' domain</param>
        public AuthController(IMapper mapper,
            MoryxUserManager userManager,
            MoryxRoleManager roleManager,
            IPermissionManager permissionManager,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionManager = permissionManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        /// <summary>
        /// Sign-up of a new MORYX user
        /// </summary>
        /// <param name="userModel">The user to be registered</param>
        [AllowAnonymous]
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(MoryxUserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(userModel.UserName);
                if (existingUser != null)
                {
                    return BadRequest(new AuthResult
                    {
                        Errors = new List<string> { "Username already in use" },
                        Success = false
                    });
                }

                var user = _mapper.Map<MoryxUserModel, MoryxUser>(userModel);
                var userCreateResult = await _userManager.CreateAsync(user, userModel.Password);

                if (userCreateResult.Succeeded)
                {
                    await _tokenService.GenerateToken(user);
                    return Ok();
                }

                return BadRequest(new AuthResult
                {
                    Errors = userCreateResult.Errors.Select(x => x.Description).ToList(),
                    Success = false
                });
            }

            return BadRequest(new AuthResult
            {
                Errors = new List<string> { "Invalid payload" },
                Success = false
            });
        }

        /// <summary>
        /// Login of a MORYX user.
        /// </summary>
        /// <param name="userLoginModel">The user to be logged in.</param>
        /// <returns>The user that logged in.</returns>
        [AllowAnonymous]
        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(UserLoginModel userLoginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(userLoginModel.UserName);
                if (user == null)
                {
                    return BadRequest(new AuthResult
                    {
                        Errors = new List<string> { "Email or password incorrect." },
                        Success = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(user, userLoginModel.Password);

                if (!isCorrect)
                {
                    return BadRequest(new AuthResult
                    {
                        Errors = new List<string> { "Email or password incorrect." },
                        Success = false
                    });
                }

                var jwtToken = await _tokenService.GenerateToken(user);
                HttpContext.Response.Cookies.SetJwtCookie(jwtToken, user);

                var userModel = _mapper.Map<MoryxUser, MoryxUserModel>(user);
                return Ok(userModel);
            }

            return BadRequest(new AuthResult
            {
                Errors = new List<string> { "Invalid payload" },
                Success = false
            });
        }

        /// <summary>
        /// Logout of a MORYX user.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("signOut")]
        public new async Task<IActionResult> SignOut()
        {
            if (ModelState.IsValid)
            {
                // We just overwrite the token
                HttpContext.Response.Cookies.RemoveJwtCookie(_configuration["CookieDomain"]);

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    return NotFound("User not found");

                var token = Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
                await _tokenService.InvalidateRefreshToken(token);

                return base.SignOut();
            }

            return BadRequest(new AuthResult
            {
                Errors = new List<string> { "Invalid payload" },
                Success = false
            });
        }

        /// <summary>
        /// Given a valid JWT token generates a new refresh token.
        /// </summary>
        /// <returns>An <see cref="AuthResult"/> with a new token to be used.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {

            TokenRequest tokenRequest = new TokenRequest()
            {
                RefreshToken = Request.Cookies[MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME],
                Token = Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME]
            };

            var result = await _tokenService.VerifyAndGenerateRefreshToken(tokenRequest);
            if (!result.Success)
                return BadRequest(result);

            HttpContext.Response.Cookies.SetJwtCookie(result);
            return Ok(JsonSerializer.Serialize<AuthResult>(result));
        }

        /// <summary>
        /// Returns the user provided in the <see cref="HttpContext"/>
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
                return NotFound("User not found");

            var userModel = _mapper.Map<MoryxUser, MoryxUserModel>(user);
            return Ok(userModel);
        }

        /// <summary>
        /// Returns a list of all users
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.Select(_ => _.UserName).ToArrayAsync();
            return Ok(users);
        }

        /// <summary>
        /// Returns a list of all permissions assigned to the user provided by the <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="filter">Optional filter to return only those permissions that start with <paramref name="filter"/>.</param>
        [HttpGet("user/permissions")]
        public async Task<ActionResult<string[]>> GetUserPermissions([FromQuery] string filter = "")
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
                return NotFound("User not found");

            filter ??= "";
            var roles = await _userManager.GetRolesAsync(user);
            var permissionClaims = await _tokenService.GetAllPermissionClaims(roles);
            var permissions = permissionClaims.Select(p => p.Value).Where(p => p.StartsWith(filter)).ToArray();

            return Ok(permissions);
        }

        /// <summary>
        /// Verifies whether the given token is valid
        /// </summary>
        /// <param name="token">The token to be verified.</param>       
        [AllowAnonymous]
        [HttpPost("verifyToken")]
        public IActionResult VerifyToken([FromBody] string token)
        {
            var isValid = _tokenService.IsTokenValid(token);
            if (isValid)
                return Accepted();

            return Forbid();
        }

        /// <summary>
        /// Returns a list of all available roles.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToArray();
            return Ok(roles);
        }

        /// <summary>
        /// Creates a new role with the given <paramref name="roleName"/>.
        /// </summary>
        /// <param name="roleName">The name of the role to be created</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name should be provided.");
            }

            var newRole = new MoryxRole(roleName);
            var roleResult = await _roleManager.CreateAsync(newRole);

            if (roleResult.Succeeded)
            {
                await _roleManager.AddClaimAsync(newRole, new Claim("permission", "order.create"));

                return Ok();
            }

            return Problem(roleResult.Errors.First().Description, null, 500);
        }

        /// <summary>
        /// Assigns a certain role to a user
        /// </summary>
        /// <param name="userName">The user name of the user that should be assigend to the role.</param>
        /// <param name="roleName">The name of the role the user should be assigned to</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("user/{userName}/role")]
        public async Task<IActionResult> AddUserToRole(string userName, [FromBody] string roleName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userName);
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }

        /// <summary>
        /// Updates the roles assigned to the given user. Removes all roles not included in the list of
        /// <paramref name="roles"/> and adds all roles not yet assigned to the user.
        /// </summary>
        /// <param name="userName">The user to be updated.</param>
        /// <param name="roles">The new list of roles assigned to the user.</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPut("user/{userName}/roles")]
        public async Task<IActionResult> UpdateUserRoles(string userName, [FromBody] string[] roles)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userName);

            if (user is null)
            {
                return NotFound("User not found.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesToRemove = currentRoles.Except(roles);
            var rolesToAdd = roles.Except(currentRoles);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (removeResult.Succeeded == false)
            {
                return Problem(removeResult.Errors.First().Description, null, 500);
            }

            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);

            if (addResult.Succeeded == false)
            {
                return Problem(addResult.Errors.First().Description, null, 500);
            }

            return Ok();
        }

        /// <summary>
        /// Returns a list of roles assigned to the given user.
        /// </summary>
        /// <param name="userName">The user for which the information is retrieved.</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("user/{userName}/roles")]
        public async Task<IActionResult> GetUserRoles(string userName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userName);

            if (user is null)
            {
                return NotFound("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        /// <summary>
        /// Add the given <paramref name="permission"/> to the specified role
        /// </summary>
        /// <param name="roleName">The name of the role to be modified.</param>
        /// <param name="permission">The permission to be added to the specified role.</param>
        /// <returns></returns>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("roles/{roleName}/permission")]
        public async Task<IActionResult> AddPermissionToRole(string roleName, [FromBody] string permission)
        {
            var result = await _permissionManager.AddToRoleAsync(roleName, new[] { permission });

            if (result.Succeeded)
                return Ok();

            return Problem(result.Errors.First().Description, null, 500);
        }

        /// <summary>
        /// Updates the permissions assigned to the given role. Removes all permissions not included in the list of
        /// <paramref name="permissions"/> and adds all permissions not yet assigned to the role.
        /// </summary>
        /// <param name="roleName">The role to be updated.</param>
        /// <param name="permissions">The new list of permissions assigned to the role.</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPut("roles/{roleName}/permissions")]
        public async Task<IActionResult> UpdateRolePermissions(string roleName, [FromBody] string[] permissions)
        {
            if (IsSuperAdmin(roleName))
            {
                return BadRequest("Admin role cannot be modified");
            }

            if (await _roleManager.RoleExistsAsync(roleName) == false)
            {
                return NotFound("Role not found.");
            }

            var result = await _permissionManager.UpdatePermissionsAsync(roleName, permissions);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }

        /// <summary>
        /// Deletes the specified role.
        /// </summary>
        /// <param name="roleName">The name of the role to be deleted.</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpDelete("roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (IsSuperAdmin(roleName))
            {
                return BadRequest("Admin role cannot be deleted");
            }

            var role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                return NotFound("Role not found");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }

        /// <summary>
        /// Returns a list of permissions available in the system. Includes all permissions that start with 
        /// the provided <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">A filter on the returned list. </param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("permissions")]
        public IActionResult Permissions([FromQuery] string filter = "")
        {
            filter ??= "";

            var permissions = _permissionManager.Permissions
                .Include(p => p.Roles).ToArray()
                .Where(p => p.Name.StartsWith(filter))
                .Select(permission => _mapper.Map<Permission, PermissionModel>(permission)).ToArray();

            return Ok(permissions);
        }

        /// <summary>
        /// Creates a new permission.
        /// </summary>
        /// <param name="permission">The new permission to be created.</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("permissions")]
        public async Task<IActionResult> CreatePermission([FromBody] string permission)
        {
            var result = await _permissionManager.CreateAsync(permission);

            if (result.Succeeded)
                return Ok();

            return Problem(result.Errors.First().Description, null, 500);
        }

        /// <summary>
        /// Deletes an existing permision with the given <paramref name="permissionId"/>.
        /// </summary>
        /// <param name="permissionId">The id of the permission to be deleted.</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpDelete("permissions/{permissionId}")]
        public async Task<IActionResult> DeletePermission(string permissionId)
        {
            var result = await _permissionManager.DeleteAsync(permissionId);

            if (result.Succeeded)
                return Ok();

            return Problem(result.Errors.First().Description, null, 500);
        }

        /// <summary>
        /// Deletes the user with the given <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName">The user name of the user to be deleted.</param>
        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpDelete("user/{userName}")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            if (userName == "admin")
            {
                return BadRequest("Admin cannot be deleted.");
            }
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
            {
                return NotFound("User not found.");
            }

            // Removes the refresh token that belongs to the user first. Otherwise we violate a constraint.
            await _tokenService.InvalidateRefreshTokens(user);
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return Ok();

            return Problem(result.Errors.First().Description, null, 500);
        }

        private static bool IsSuperAdmin(string roleName)
        {
            return roleName == Roles.SuperAdmin;
        }
    }
}

