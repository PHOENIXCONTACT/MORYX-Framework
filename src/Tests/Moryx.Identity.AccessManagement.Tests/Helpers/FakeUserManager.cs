// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Tests.Helpers
{
    public class FakeUserManager : MoryxUserManager
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<MoryxUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<MoryxUser>>().Object,
                new IUserValidator<MoryxUser>[0],
                new IPasswordValidator<MoryxUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<MoryxUser>>>().Object)
        {
        }
    }
}
