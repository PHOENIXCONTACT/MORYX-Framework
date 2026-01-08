// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity;
using Moryx.Identity.AccessManagement.Data;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Identity.AccessManagement.Models;

/// <summary>
/// Sets new password for user
/// </summary>
public class MoryxUserResetPasswordModel
{
    /// <summary>
    /// Maps to <see cref="IdentityUser{TKey}.UserName"/>.
    /// </summary>
    [Required]
    public string UserName { get; set; }

    /// <summary>
    /// Maps to <see cref="IdentityUser{TKey}.PasswordHash"/>.
    /// </summary>
    [Required]
    public string NewPassword { get; set; }

    /// <summary>
    /// Maps to <see cref="IdentityUser{TKey}.PasswordHash"/>.
    /// </summary>
    [Required]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }

    /// <summary>
    /// Maps to <see cref="PasswordReset.ResetToken"/>.
    /// </summary>
    [Required]
    public string ResetToken { get; set; }
}