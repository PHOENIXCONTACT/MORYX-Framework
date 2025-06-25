using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Tests.Helpers
{
    public class FakeRoleManager : MoryxRoleManager
    {
        public FakeRoleManager()
            : base(new Mock<IRoleStore<MoryxRole>>().Object,
                new IRoleValidator<MoryxRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<MoryxRole>>>().Object)
        {
        }
    }
}