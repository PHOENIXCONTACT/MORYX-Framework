// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// View Model for managing users in the MORYX AccessManagement. Maps to a <see cref="MoryxUser"/>.
    /// </summary>
    public class ManageUserRolesViewModel
    {
        /// <summary>
        /// Maps to <see cref="IdentityUser{TKey}.Id"/>.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Maps to <see cref="IdentityUser{TKey}.UserName"/>.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// List of all <see cref="MoryxRole"/>s assigned to this <see cref="MoryxUser"/>.
        /// </summary>
        public IList<UserRolesViewModel> UserRoles { get; set; }
    }

    /// <summary>
    /// View model for managing <see cref="MoryxRole"/>s.
    /// </summary>
    public class UserRolesViewModel
    {
        /// <summary>
        /// Maps to <see cref="IdentityRole{TKey}.Name"/>
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// True if the role is to be assigned to the user; false otherwise.
        /// </summary>
        public bool Selected { get; set; }
    }
}
