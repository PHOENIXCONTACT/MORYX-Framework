using System;
using System.Collections.Generic;
using Moryx.Modules;

namespace Moryx.Users.Management
{
    /// <summary>
    /// User manager to handle involved users during the production
    /// </summary>
    internal interface IUserManager : IInitializablePlugin
    {
        /// <summary>
        /// Current user information
        /// </summary>
        IReadOnlyList<IUserData> Users { get; }

        /// <summary>
        /// Default user if no other users available
        /// </summary>
        IUserData DefaultUser { get; }

        /// <summary>
        /// Sign in a user by its card number
        /// </summary>
        void SignIn(IUserData user);

        /// <summary>
        /// Sign out a user by its card number
        /// </summary>
        void SignOut(IUserData user);

        /// <summary>
        /// Event to inform that a user was signed in
        /// </summary>
        event EventHandler<IUserData> UserSignedIn;

        /// <summary>
        /// Event to inform that a user was signed out
        /// </summary>
        event EventHandler<IUserData> UserSignedOut;
    }
}
