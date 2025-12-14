// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement
{
    public interface IPasswordResetService
    {
        Task<PasswordReset> GetPasswordResetAsync(string userId);

        Task<PasswordReset> GeneratePasswordResetAsync(string userId);

        Task RemovePasswordResetAsync(PasswordReset passwordReset);
    }
}

