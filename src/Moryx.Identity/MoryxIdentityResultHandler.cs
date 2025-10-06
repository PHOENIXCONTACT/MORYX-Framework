// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Moryx.Identity
{
    /// <summary>
    /// An <see cref="IAuthorizationMiddlewareResultHandler"/> that can perform MORYX Identity based handling of authorization responses.
    /// </summary>
    public class MoryxIdentityResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly IAuthorizationMiddlewareResultHandler _handler;

        /// <summary>
        /// Initializes a new instance of <see cref="MoryxIdentityResultHandler"/>.
        /// </summary>
        /// <inheritdoc />
        public MoryxIdentityResultHandler()
        {
            _handler = new AuthorizationMiddlewareResultHandler();
        }

        /// <summary>
        /// Writes the collection of missing permissions in the HTTP respone in case of a 403 error.
        /// In all other cases the authorization result is processed by the <see cref="AuthorizationMiddlewareResultHandler"/>.
        /// </summary>
        /// <returns></returns>
        public async Task HandleAsync(
            RequestDelegate requestDelegate,
            HttpContext httpContext,
            AuthorizationPolicy authorizationPolicy,
            PolicyAuthorizationResult policyAuthorizationResult)
        {
            if (policyAuthorizationResult.Forbidden && policyAuthorizationResult.AuthorizationFailure != null)
            {
                httpContext.Response.StatusCode = 403;
                var response = policyAuthorizationResult.AuthorizationFailure.FailedRequirements
                    .Select(req => ((ClaimsAuthorizationRequirement)req).AllowedValues)
                    .SelectMany(x => x);
                await httpContext.Response.WriteAsJsonAsync(response);
                return;
            }
            await _handler.HandleAsync(requestDelegate, httpContext, authorizationPolicy, policyAuthorizationResult);
        }
    }
}
