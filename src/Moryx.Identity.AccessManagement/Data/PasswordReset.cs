// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moryx.Identity.AccessManagement.Data
{
    public class PasswordReset
    {
        /// <summary>
        /// The ID of this token
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The token value
        /// </summary>
        [Required]
        public string ResetToken { get; set; }

        /// <summary>
        /// The time the token expires
        /// </summary>
        [Required]
        public DateTime ExpiryTime { get; set; }

        /// <summary>
        /// Id of the <see cref="MoryxUser"/> this refresh token is linked to.
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// The <see cref="MoryxUser"/> this refresh token is linked to.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public MoryxUser User { get; set; }
    }
}

