// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authentication;

namespace Moryx.Identity
{
    /// <summary>
    /// Extension methods to configure MORYX Identity authentication.
    /// </summary>
    public static class MoryxIdentityExtensions
    {
        /// <summary>
        /// Enables MORYX Identity authentication using the default scheme <see cref="MoryxIdentityDefaults.AuthenticationScheme"/>.
        /// <para>
        /// MORYX Identity authentication performs authentication by extracting a JWT token from the <c>Authorization</c> request header and validating it against the MORYX Identity Server at the specified endpoint.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddMoryxIdentity(this AuthenticationBuilder builder)
            => builder.AddMoryxIdentity(MoryxIdentityDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Enables MORYX Identity authentication using a pre-defined scheme.
        /// <para>
        /// MORYX Identity authentication performs authentication by extracting a JWT token from the <c>Authorization</c> request header and validating it against the MORYX Identity Server at the specified endpoint.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddMoryxIdentity(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddMoryxIdentity(authenticationScheme, _ => { });

        /// <summary>
        /// Enables MORYX Identity authentication using the default scheme <see cref="MoryxIdentityDefaults.AuthenticationScheme"/>.
        /// <para>
        /// MORYX Identity authentication performs authentication by extracting a JWT token from the <c>Authorization</c> request header and validating it against the MORYX Identity Server at the specified endpoint.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="MoryxIdentityOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddMoryxIdentity(this AuthenticationBuilder builder, Action<MoryxIdentityOptions> configureOptions)
            => builder.AddMoryxIdentity(MoryxIdentityDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Enables MORYX Identity authentication using the specified scheme.
        /// <para>
        /// MORYX Identity authentication performs authentication by extracting a JWT token from the <c>Authorization</c> request header and validating it against the MORYX Identity Server at the specified endpoint.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="MoryxIdentityOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddMoryxIdentity(this AuthenticationBuilder builder, string authenticationScheme, Action<MoryxIdentityOptions> configureOptions)
            => builder.AddScheme<MoryxIdentityOptions, MoryxIdentityHandler>(authenticationScheme, configureOptions);
    }
}

