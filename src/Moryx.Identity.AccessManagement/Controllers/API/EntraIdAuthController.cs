// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.Models;

namespace Moryx.Identity.AccessManagement.Controllers
{
    /// <summary>
    /// API controller to provide actions regarding the single sign-on via Entra ID (Azure AD) in addition to the authentication controller.
    /// </summary>
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [Route("api/auth")]
    [ApiController]
    public class EntraIdAuthController : ControllerBase
    {
        private readonly MoryxUserManager _userManager;
        private readonly ITokenService _tokenService;
        private readonly IDownstreamApi _downstreamWebApi;

        /// <summary>
        /// Creates a new instance  of the <see cref="EntraIdAuthController"/>.
        /// </summary>
        /// <param name="userManager">The <see cref="MoryxUserManager"/> for MORYX users</param>
        /// <param name="tokenService">A token service for handling the JWTs</param>
        /// <param name="downstreamWebApi">The downstream API to call for the user data.</param>
        public EntraIdAuthController(
            MoryxUserManager userManager,
            ITokenService tokenService,
            IDownstreamApi downstreamWebApi)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _downstreamWebApi = downstreamWebApi;
        }

        /// <summary>
        /// Signs in via Microsoft Entra ID.
        /// The actual login via Entra ID is done through the middleware.
        /// This endpoint then provides its own session, just like the login via username and password.
        /// From that point on, the AccessManagement takes care of the accesses
        /// </summary>
        /// <returns>Redirects to the provided url.</returns>
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        [HttpGet("signIn/microsoft")]
        public async Task<IActionResult> SignInViaMicrosoft([FromQuery] string redirectUrl)
        {
            // try to get the username
            if (User.Identity?.Name == null || User.Identity.Name.Contains('@') == false)
            {
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Failed to identify the user." },
                    Success = false
                });
            }

            var userName = User.Identity.Name.Split('@')[0];

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new MoryxUser
                {
                    UserName = userName
                };
                var userCreateResult = await _userManager.CreateAsync(user);
                if (userCreateResult.Succeeded == false)
                {
                    return StatusCode(500, new AuthResult
                    {
                        Errors = new List<string> { "Failed to create the user." },
                        Success = false
                    });
                }
            }

            try
            {
                await UpdateUserByEntraIdInformationAsync(user);
            }
            catch
            {
                return StatusCode(500, new AuthResult
                {
                    Errors = new List<string> { "Failed to access the remote user data." },
                    Success = false
                });
            }

            await _userManager.UpdateAsync(user);

            var jwtToken = await _tokenService.GenerateToken(user);
            HttpContext.Response.Cookies.SetJwtCookie(jwtToken, user);

            return Redirect(redirectUrl);
        }

        private async Task UpdateUserByEntraIdInformationAsync(MoryxUser user)
        {

            // get user information from microsoft graph API
            using var response = await _downstreamWebApi.CallApiForUserAsync("DownstreamApi");
            response.EnsureSuccessStatusCode();

            var rawUserInfo = await response.Content.ReadFromJsonAsync<JsonDocument>();
            if (rawUserInfo.RootElement.TryGetProperty("givenName", out JsonElement givenName))
            {
                user.Firstname = givenName.GetString();
            }

            if (rawUserInfo.RootElement.TryGetProperty("surname", out JsonElement surname))
            {
                user.LastName = surname.GetString();
            }

            if (rawUserInfo.RootElement.TryGetProperty("mail", out JsonElement mail))
            {
                user.Email = mail.GetString();
                user.EmailConfirmed = true;
            }
            // there are also properties like businessPhones and officeLocation, but there is currently no reason to save them 
        }
    }
}

