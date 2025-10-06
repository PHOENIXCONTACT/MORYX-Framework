// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement
{
    /// <summary>
    /// Implements the <see cref="UserClaimsPrincipalFactory{TUser, TRole}"/> using <see cref="MoryxUser"/> 
    /// and <see cref="MoryxRole"/>.
    /// </summary>
    public class MoryxClaimsPrincipalFactory : UserClaimsPrincipalFactory<MoryxUser, MoryxRole>
    {
        /// <inheritdoc />
        public MoryxClaimsPrincipalFactory(
            MoryxUserManager userManager,
            MoryxRoleManager roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
        }

        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> from a <see cref="MoryxUser"/> with claims to the 
        /// <see cref="MoryxUser.Firstname"/> and <see cref="MoryxUser.LastName"/> asynchronously.
        /// </summary>
        /// <param name="user">The user to create a <see cref="ClaimsPrincipal"/> from.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsPrincipal"/>.</returns>
        public override async Task<ClaimsPrincipal> CreateAsync(MoryxUser user)
        {
            var principal = await base.CreateAsync(user);

            var claimIdentity = ((ClaimsIdentity)principal.Identity);
            if (claimIdentity == null)
                return principal;

            if (!string.IsNullOrEmpty(user.Firstname))
                claimIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.Firstname));

            if (!string.IsNullOrEmpty(user.LastName))
                claimIdentity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

            return principal;
        }
    }
}
