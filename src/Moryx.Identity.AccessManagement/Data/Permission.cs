// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;

namespace Moryx.Identity.AccessManagement.Data
{
    /// <summary>
    /// Defines a permission as an identifiable asset to be assigned to a collection of roles.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Creates a new permission instance with a generated <see cref="Permission.Id"/>.
        /// </summary>
        public Permission()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// A generated ID of this permission
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A human readable name of this permission.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The collection of roles this permission is assigned to.
        /// </summary>
        public virtual ICollection<MoryxRole> Roles { get; set; }
    }
}
