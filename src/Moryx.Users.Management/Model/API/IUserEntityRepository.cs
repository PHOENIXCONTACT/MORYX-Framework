#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.Users.Management.Model
{
    public interface IUserEntityRepository : IRepository<UserEntity>
    {
        UserEntity GetByIdentifier(string identifier);

        UserEntity Create(string firstName, string lastName, string identifier, bool signedIn);
    }
}
