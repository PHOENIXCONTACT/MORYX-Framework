using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Container;
using Moryx.Model.Repositories;
using Moryx.Tools;
using Moryx.Users.Management.Model;

namespace Moryx.Users.Management
{
    [Component(LifeCycle.Singleton, typeof(IUserManager))]
    internal class UserManager : IUserManager
    {
        #region Dependencies

        public IUnitOfWorkFactory<UsersContext> UnitOfWorkFactory { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        #region Fields and Properties

        private readonly IList<IUserData> _users = new List<IUserData>();

        public IReadOnlyList<IUserData> Users => _users.Where(u => u != DefaultUser).ToArray();

        public IUserData DefaultUser { get; private set; }

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public void Start()
        {
            using var uow = UnitOfWorkFactory.Create();

            var userRepo = uow.GetRepository<IUserEntityRepository>();
            var restored = userRepo.GetAll().Select(UserStorage.Load);
            _users.AddRange(restored);

            var defaultUser = GetOrCreateDefaultUser(uow);
            defaultUser.SignedIn = true;
            DefaultUser = defaultUser;
        }

        /// <inheritdoc />
        public void Stop()
        {
            _users.Clear();
            DefaultUser = null;
        }

        /// <inheritdoc />
        public void SignIn(IUserData user) =>
            ChangeUserState(user, true);

        /// <inheritdoc />
        public void SignOut(IUserData user) =>
            ChangeUserState(user, false);

        private void ChangeUserState(IUserData user, bool signedIn)
        {
            // Sign in and persist information
            if (user.SignedIn == signedIn)
                return;

            user.SignedIn = signedIn;

            using (var uow = UnitOfWorkFactory.Create())
            {
                UserStorage.Save(uow, user);
            }

            if (signedIn)
                UserSignedIn?.Invoke(this, user);
            else
                UserSignedOut?.Invoke(this, user);
        }


        private IUserData GetOrCreateDefaultUser(IUnitOfWork uow)
        {
            var defaultUserData = _users.FirstOrDefault(u => u.User.Identifier.Equals(ModuleConfig.DefaultUser));
            if (defaultUserData != null)
                return defaultUserData;

            defaultUserData = new UserData
            {
                Identifier = ModuleConfig.DefaultUser,
                SignedIn = true
            };

            defaultUserData.User.FirstName = "Marjory";
            defaultUserData.User.LastName = "Stewart-Baxter";

            UserStorage.Save(uow, defaultUserData);

            _users.Add(defaultUserData);

            return defaultUserData;
        }

        /// <inheritdoc />
        public event EventHandler<IUserData> UserSignedIn;

        /// <inheritdoc />
        public event EventHandler<IUserData> UserSignedOut;
    }
}