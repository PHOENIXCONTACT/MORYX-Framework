using Moryx.Model.Repositories;
using Moryx.Users.Management.Model;

namespace Moryx.Users.Management
{
    internal static class UserStorage
    {
        public static UserEntity Save(IUnitOfWork uow, IUserData userData)
        {
            using var context = (UsersContext)uow.DbContext;

            var entity = context.UserEntities.Find(userData.Id);
            if (entity == null)
            {
                entity = context.UserEntities
                    .Add(new UserEntity())
                    .Entity;
            }

            entity.Identifier = userData.Identifier;
            entity.SignedIn = userData.SignedIn;
            entity.FirstName = userData.User.FirstName;
            entity.LastName = userData.User.LastName;

            uow.SaveChanges();
            return entity;
        }

        public static UserData Load(UserEntity entity)
        {
            var userData = new UserData
            {
                Identifier = entity.Identifier,
                SignedIn = entity.SignedIn
            };

            userData.User.FirstName = entity.FirstName;
            userData.User.LastName = entity.LastName;

            ((IPersistentObject) userData).Id = entity.Id;

            return userData;
        }
    }
}