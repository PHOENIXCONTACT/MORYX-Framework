using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Runtime.Modules;

namespace Moryx.Users.Management.Facade
{
    internal class UserManagementFacade : FacadeBase, IUserManagement
    {
        public IUserManager UserManager { get; set; }

        public IReadOnlyList<User> Users
        {
            get
            {
                ValidateHealthState();
                return UserManager.Users.Select(u => u.User).ToArray();
            }
        }

        public User DefaultUser
        {
            get
            {
                ValidateHealthState();
                return UserManager.DefaultUser.User;
            }
        }

        public override void Activate()
        {
            base.Activate();

            UserManager.UserSignedIn += OnUserSignedIn;
            UserManager.UserSignedOut += OnUserSignedOut;
        }


        public override void Deactivate()
        {
            UserManager.UserSignedIn -= OnUserSignedIn;
            UserManager.UserSignedOut -= OnUserSignedOut;

            base.Deactivate();
        }

        private void OnUserSignedIn(object sender, IUserData userData)
        {
            UserSignedIn?.Invoke(this, userData.User);
        }

        private void OnUserSignedOut(object sender, IUserData userData)
        {
            UserSignedOut?.Invoke(this, userData.User);
        }

        public void SignIn(User user)
        {
            ValidateHealthState();

            var userData = GetUserData(user);
            UserManager.SignIn(userData);
        }

        public void SignOut(User user)
        {
            ValidateHealthState();

            var userData = GetUserData(user);
            UserManager.SignOut(userData);
        }

        public User GetUser(string identifier) =>
            GetUser(identifier, true);

        public User GetUser(string identifier, bool fallbackDefault)
        {
            ValidateHealthState();

            var userData = UserManager.Users.FirstOrDefault(u => u.User.Identifier.Equals(identifier));
            if (userData == null && fallbackDefault)
                return UserManager.DefaultUser.User;

            return userData?.User;
        }

        private IUserData GetUserData(User user)
        {
            if (user == null)
                throw new ArgumentNullException("User was null", nameof(user));

            var userData = UserManager.Users.FirstOrDefault(u => u.User == user);
            if (userData == null)
                throw new ArgumentNullException("User was not found in internal module", nameof(user));

            return userData;
        }

        public event EventHandler<User> UserSignedIn;

        public event EventHandler<User> UserSignedOut;
    }
}
