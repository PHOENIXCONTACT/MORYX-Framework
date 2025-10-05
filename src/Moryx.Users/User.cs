// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Users
{
    /// <summary>
    /// User Data class with the user information
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier of the user (e.g. a card number or personal number
        /// </summary>
        public virtual string Identifier { get; protected set; }

        /// <summary>
        /// First name of the user for display purposes
        /// </summary>
        public virtual string FirstName { get; protected set; }

        /// <summary>
        /// Last name of the user for display purposes
        /// </summary>
        public virtual string LastName { get; protected set; }

        /// <summary>
        /// Flag if the user is currently signed in
        /// </summary>
        public virtual bool SignedIn { get; protected set; }
    }
}