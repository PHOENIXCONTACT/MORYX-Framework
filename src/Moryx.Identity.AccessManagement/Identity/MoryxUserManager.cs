// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement
{
    /// <summary>
    /// Provides the APIs for managing <seealso cref="MoryxUser"/> in the persistance store.
    /// </summary>
    public class MoryxUserManager : UserManager<MoryxUser>
    {
        /// <inheritdoc/>
        public MoryxUserManager(IUserStore<MoryxUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<MoryxUser> passwordHasher,
            IEnumerable<IUserValidator<MoryxUser>> userValidators,
            IEnumerable<IPasswordValidator<MoryxUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<MoryxUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        /// <summary>
        /// Returns a flag indicating whether the given <paramref name="password"/> is valid for the
        /// specified <paramref name="user"/> and update the <see cref="MoryxUser.LastSignIn"/> timestamp.
        /// </summary>
        /// <param name="user">The user whose password should be validated.</param>
        /// <param name="password">The password to validate</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing true if
        /// the specified <paramref name="password" /> matches the one store for the <paramref name="user"/>,
        /// otherwise false.</returns>
        public override async Task<bool> CheckPasswordAsync(MoryxUser user, string password)
        {
            var checkPasswordResult = await base.CheckPasswordAsync(user, password);
            if (checkPasswordResult)
            {
                user.LastSignIn = DateTime.UtcNow;
                await UpdateAsync(user);
            }

            return checkPasswordResult;
        }
    }

    /// <summary>
    /// Provides the APIs for managing <see cref="MoryxRole"/>s in a persistence store.
    /// </summary>
    public class MoryxRoleManager : RoleManager<MoryxRole>
    {
        /// <inheritdoc/>
        public MoryxRoleManager(IRoleStore<MoryxRole> store,
            IEnumerable<IRoleValidator<MoryxRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<MoryxRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
