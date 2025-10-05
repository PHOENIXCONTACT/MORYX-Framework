// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Identity.AccessManagement.Data
{
    /// <summary>
    /// Mapping class for the m-to-n relation between roles and permissions
    /// </summary>
    public class PermissionRole
    {
        /// <summary>
        /// Id of the referenced role
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// The referenced role
        /// </summary>
        public virtual MoryxRole Role { get; set; }

        /// <summary>
        /// Id of the referenced permission
        /// </summary>
        public string PermissionId { get; set; }

        /// <summary>
        /// The referenced permission
        /// </summary>
        public virtual Permission Permission { get; set; }
    }
}
