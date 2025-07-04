// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Moryx.Identity
{
    /// <summary>
    /// An <see cref="AuthenticationHandler{TOptions}"/> that can perform MORYX Identity based authentication.
    /// </summary>
    public class MoryxIdentityHandler : AuthenticationHandler<MoryxIdentityOptions>
    {
        private readonly JwtSecurityTokenHandler _jwtTokenHandler;
        private readonly MoryxAccessManagementClient _client;

        /// <summary>
        /// Initializes a new instance of <see cref="MoryxIdentityHandler"/>.
        /// </summary>
        /// <inheritdoc />
        public MoryxIdentityHandler(
            IOptionsMonitor<MoryxIdentityOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMemoryCache memoryCache,
            HttpClient identityClient = null)
            : base(options, logger, encoder, clock)
        {
            _jwtTokenHandler = new JwtSecurityTokenHandler();
            _client = new MoryxAccessManagementClient(options, memoryCache, logger.CreateLogger(nameof(MoryxIdentityHandler) + ":" + nameof(MoryxAccessManagementClient)), identityClient);
        }

        /// <summary>
        /// Searches the 'Authorization' header for a Cookie with the name specified in <see cref="MoryxIdentityOptions.CookieName"/>. 
        /// If the cookie is found, it is validated using the MORYX Identity Server endpoint specified in the <see cref="MoryxIdentityOptions"/>.
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var metadata = Context.GetEndpoint()?.Metadata;
            var authorizeAttributes = metadata?.OfType<AuthorizeAttribute>();
            if (authorizeAttributes?.Any() != true || metadata.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult(); // endpoint has AllowAnonymous or no Authorize attribute

            if (Context.User == null)
                return AuthenticateResult.Fail("User not found.");

            var token = Context.Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
            if (token == null)
                return AuthenticateResult.Fail("JwtToken not found");

            var refreshToken = Context.Request.Cookies[MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME];
            if (refreshToken == null)
                return AuthenticateResult.Fail("RefreshToken not found");

            var decodedJwt = _jwtTokenHandler.ReadJwtToken(token);
            if (decodedJwt.ValidTo < DateTime.UtcNow)
            {
                var refreshResult = await _client.GetRefreshedTokens(token, refreshToken);
                if (refreshResult is null || !refreshResult.Success)
                    return AuthenticateResult.Fail("Token is expired and could not be refreshed");
                token = refreshResult.Token;
                refreshToken = refreshResult.RefreshToken;
                Context.Response.Cookies.Append(MoryxIdentityDefaults.JWT_COOKIE_NAME, token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = refreshResult.Domain,
                });
                Context.Response.Cookies.Append(MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME, refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = refreshResult.Domain,
                });
            }

            // if multiple policies are required set filter to empty string 
            string requiredPolicy = "";
            var distinctPolicies = authorizeAttributes.Select(a => a.Policy).Where(s => !string.IsNullOrEmpty(s)).Distinct();
            if (distinctPolicies.Count() == 1)
                requiredPolicy = distinctPolicies.First();

            var permissions = await _client.GetPermissions(token, refreshToken, requiredPolicy);
            if (permissions == null)
                return AuthenticateResult.Fail("Retrieving Permissions failed");

            var appIdentity = new ClaimsIdentity(MoryxIdentityOptions.MoryxIdentity);
            foreach (var perm in permissions)
                appIdentity.AddClaim(new Claim("Permission", perm));
            var ticket = new AuthenticationTicket(
                        new ClaimsPrincipal(appIdentity), this.Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}

