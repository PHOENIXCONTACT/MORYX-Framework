// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Users
{
    /// <summary>
    /// Reference class for a user e.g. to restore from facade before it is known.
    /// </summary>
    public sealed class UserReference : User
    {
        /// <summary>
        /// Creates a new instance of the user
        /// </summary>
        public UserReference(string identifier)
        {
            Identifier = identifier;
        }
    }
}