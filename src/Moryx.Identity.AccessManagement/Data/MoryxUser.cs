using System;
using Microsoft.AspNetCore.Identity;

namespace Moryx.Identity.AccessManagement.Data
{
    /// <summary>
    /// Extended implementation of the default implementation of <see cref="IdentityUser{TKey}"/> which uses a 
    /// string as a primary key.
    /// </summary>
    public class MoryxUser : IdentityUser
    {
        /// <summary>
        /// The first name of this user
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// The surname of this user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Timestamp of the last sign in of the user.
        /// </summary>
        public DateTime LastSignIn { get; set; }

        public virtual PasswordReset PasswordReset { get; set; }
    }
}
