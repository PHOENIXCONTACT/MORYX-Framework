// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Identity
{
    public interface IPasswordResetService
    {
        Task<PasswordReset> GetPasswordReset(string userId);

        Task<PasswordReset> GeneratePasswordReset(string userId);

        Task RemovePasswordReset(PasswordReset passwordReset);
    }
}

