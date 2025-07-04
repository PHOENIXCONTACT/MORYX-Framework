// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity;
using Moryx.Identity.AccessManagement.Data;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// Maps to <see cref="MoryxUser"/>.
    /// </summary>
    public class MoryxUserModel
    {
        /// <summary>
        /// Maps to <see cref="IdentityUser{TKey}.Email"/>.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Maps to <see cref="IdentityUser{TKey}.UserName"/>.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Maps to <see cref="MoryxUser.Firstname"/>.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Maps to <see cref="MoryxUser.LastName"/>.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Maps to <see cref="IdentityUser{TKey}.PasswordHash"/>.
        /// </summary>
        public string Password { get; set; }
    }
}
