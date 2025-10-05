// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Users
{
    /// <summary>
    /// User management to handle involved users during the production
    /// </summary>
    public interface IUserManagement
    {
        /// <summary>
        /// Current list of user information
        /// </summary>
        IReadOnlyList<User> Users { get; }

        /// <summary>
        /// Default user if no other users available
        /// </summary>
        User DefaultUser { get; }

        /// <summary>
        /// Signs in a user
        /// </summary>
        void SignIn(User user);

        /// <summary>
        /// Signs out a user
        /// </summary>
        void SignOut(User user);

        /// <summary>
        /// Returns the user with the given identifier. If not found, the default user will be returned
        /// </summary>
        /// <param name="identifier">Identifier of the user</param>
        User GetUser(string identifier);

        /// <summary>
        /// Returns the user with the given identifier
        /// </summary>
        /// <param name="identifier">Identifier of the user</param>
        /// <param name="fallbackDefault">If set to <c>true</c> the default user will be returned if the requested user is not found.</param>
        /// <returns></returns>
        User GetUser(string identifier, bool fallbackDefault);

        /// <summary>
        /// Event to inform that a user was signed in
        /// </summary>
        event EventHandler<User> UserSignedIn;

        /// <summary>
        /// Event to inform that a user was signed out
        /// </summary>
        event EventHandler<User> UserSignedOut;
    }
}
