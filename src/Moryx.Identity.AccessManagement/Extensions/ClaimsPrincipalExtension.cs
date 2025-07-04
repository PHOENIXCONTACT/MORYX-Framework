// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Security.Claims;

namespace Moryx.Identity.AccessManagement
{
    internal static class ClaimsPrincipalExtension
    {
        public static string GeUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;

            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userId != null ? userId.Value : string.Empty;
        }

        public static string GetFullName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;

            var firstName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            var lastName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);

            if (firstName == null && lastName != null)
            {
                return lastName.Value;
            }

            if (firstName != null && lastName == null)
            {
                return firstName.Value;
            }

            if (firstName == null && lastName == null)
            {
                return "Unknown";
            }

            return $"{firstName?.Value} {lastName?.Value}";
        }
    }
}
