#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Users.Management.Model
{
    public class UserEntity : EntityBase
    {
        public virtual string Identifier { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual bool SignedIn { get; set; }
    }
}
