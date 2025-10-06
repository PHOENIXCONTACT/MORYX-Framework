// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity;

namespace Moryx.Identity.AccessManagement.Data
{
    /// <summary>
    /// The default implementation of <see cref="IdentityRole{TKey}"/> which uses a string 
    /// as the primary key and holds a set of permissions.
    /// </summary>
    public class MoryxRole : IdentityRole
    {
        /// <inheritdoc/>
        public MoryxRole()
        {
        }

        /// <inheritdoc/>
        public MoryxRole(string roleName) : base(roleName)
        {
        }

        /// <summary>
        /// Set of permissions assigned to this role.
        /// </summary>
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
