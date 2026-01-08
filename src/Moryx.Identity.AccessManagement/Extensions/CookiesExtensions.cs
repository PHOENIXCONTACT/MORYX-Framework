// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.Models;

namespace Moryx.Identity.AccessManagement;

internal static class CookiesExtensions
{
    extension(IResponseCookies cookies)
    {
        internal void SetJwtCookie(AuthResult authResult, MoryxUser user = null)
        {
            cookies.Append(MoryxIdentityDefaults.JWT_COOKIE_NAME, authResult.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = authResult.Domain,
            });

            cookies.Append(MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME, authResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = authResult.Domain,
            });

            if (user != null)
                cookies.Append(MoryxIdentityDefaults.USER_COOKIE_NAME, user.UserName, new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Domain = authResult.Domain,
                });
        }

        internal void RemoveJwtCookie(string domain)
        {
            var cookieOption = new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = domain,
            };

            cookies.Delete(MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME, cookieOption);
            cookies.Delete(MoryxIdentityDefaults.JWT_COOKIE_NAME, cookieOption);
            cookies.Delete(MoryxIdentityDefaults.USER_COOKIE_NAME, cookieOption);
        }
    }
}