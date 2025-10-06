// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Users;

namespace Moryx.Orders.Management
{

    internal class NullUserManagement : IUserManagement
    {
        public IReadOnlyList<User> Users { get; } = Array.Empty<User>();

        public User DefaultUser { get; } = new DummyUser();

        public void SignIn(User user)
        {
        }

        public void SignOut(User user)
        {
        }

        public User GetUser(string identifier)
        {
            return GetUser(identifier, true);
        }

        public User GetUser(string identifier, bool fallbackDefault)
        {
            return fallbackDefault ? DefaultUser : null;
        }

        public event EventHandler<User> UserSignedIn;

        public event EventHandler<User> UserSignedOut;

        internal sealed class DummyUser : User
        {
            public DummyUser()
            {
                FirstName = "Marjory";
                LastName = "Stewart-Baxter";
                Identifier = "9999";
                SignedIn = true;
            }
        }
    }
}
