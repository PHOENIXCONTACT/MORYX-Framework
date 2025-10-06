// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// View model to manage role permission assignements. Maps to <see cref="MoryxRole"/>.
    /// </summary>
    public class RolePermissionsViewModel
    {
        /// <summary>
        /// Maps to <see cref="IdentityRole{TKey}.Id"/>.
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Maps to <see cref="IdentityRole{TKey}.Name"/>.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Provides the list of permissions assigned to a <see cref="MoryxRole"/>
        /// </summary>
        public IList<PermissionViewModel> Permissions { get; set; }
    }

    /// <summary>
    /// View model to represent premissions for the assignement to a <see cref="MoryxRole"/>.
    /// </summary>
    public class PermissionViewModel
    {
        /// <summary>
        /// Maps to <see cref="Permission.Name"/>.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// True if the permission is to be assigned to the role; false otherwise.
        /// </summary>
        public bool Selected { get; set; }
    }
}

