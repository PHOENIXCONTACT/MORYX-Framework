using Microsoft.AspNetCore.Http;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Models;
using Moryx.Identity.Models;

namespace Moryx.Identity.AccessManagement
{
    internal static class CookiesExtensions
    {
        internal static void SetJwtCookie(this IResponseCookies cookies, AuthResult authResult, MoryxUser user = null)
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

        internal static void RemoveJwtCookie(this IResponseCookies responseCookies, string domain)
        {
            var cookieOption = new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = domain,
            };

            responseCookies.Delete(MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME, cookieOption);
            responseCookies.Delete(MoryxIdentityDefaults.JWT_COOKIE_NAME, cookieOption);
            responseCookies.Delete(MoryxIdentityDefaults.USER_COOKIE_NAME, cookieOption);
        }
    }
}
