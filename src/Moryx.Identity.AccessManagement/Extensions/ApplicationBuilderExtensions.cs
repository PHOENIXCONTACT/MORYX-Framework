using Microsoft.AspNetCore.Builder;

namespace Moryx.Identity.AccessManagement
{
    /// <summary>
    /// Extension methods to use the MORYX AccessManagement with the Asp.Net Core application builder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a custom middleware to copy the Jwt token from the moryx identity cookie to the authorization header.
        /// Also enables the CORS policy used to allow application requests to the AccessManagement API.
        /// </summary>
        /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder to add the middleware to.</param>
        /// <param name="corsPolicy">The name of the CORS policy used by the MORYX AccessManagement</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseMoryxAccessManagement(this IApplicationBuilder app, string corsPolicy)
        {
            // Copy JWT from cookie into the autorization header if that does not exist
            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
                if (!string.IsNullOrEmpty(token) && !context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }
                await next();
            });

            return app.UseCors(corsPolicy);
        }
    }
}
