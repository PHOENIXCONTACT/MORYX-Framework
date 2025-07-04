// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// View model to login
    /// </summary>
    public class UserLoginModel
    {
        /// <summary>
        /// User name of the user to be logged in.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Password of the user to be logged in.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
