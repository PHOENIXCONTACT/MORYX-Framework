using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Tests.Helpers;
using NUnit.Framework;

namespace Moryx.Identity.AccessManagement.Tests
{
    [TestFixture]
    internal class HostExtensionsTests
    {
        private Mock<IHost> _hostMock;
        private IServiceCollection _serviceCollection;

        [SetUp]
        public void Setup()
        {
            _hostMock = new Mock<IHost>();
            _serviceCollection = new ServiceCollection();
        }

        [Test]
        public async Task SeedTest()
        {
            // Arrange
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var roleManagerMock = new Mock<FakeRoleManager>();
            var userManagerMock = new Mock<FakeUserManager>();
            var permissionManagerMock = new Mock<IPermissionManager>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();

            userManagerMock.Setup(mock => mock.CreateAsync(It.IsAny<MoryxUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success))
                .Verifiable();

            roleManagerMock.Setup(mock => mock.CreateAsync(It.IsAny<MoryxRole>()))
                .Returns(Task.FromResult(IdentityResult.Success))
                .Verifiable();

            permissionManagerMock.Setup(mock => mock.CreateAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success))
                .Verifiable();

            serviceProviderMock.Setup(mock => mock.GetService(typeof(ILoggerFactory))).Returns(loggerFactoryMock.Object);
            serviceProviderMock.Setup(mock => mock.GetService(typeof(MoryxRoleManager))).Returns(roleManagerMock.Object);
            serviceProviderMock.Setup(mock => mock.GetService(typeof(MoryxUserManager))).Returns(userManagerMock.Object);
            serviceProviderMock.Setup(mock => mock.GetService(typeof(IPermissionManager))).Returns(permissionManagerMock.Object);

            serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);
            serviceProviderMock.Setup(mock => mock.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);

            _hostMock.SetupGet(i => i.Services).Returns(serviceProviderMock.Object);

            // Act
            await _hostMock.Object.Seed();

            // Assert
            roleManagerMock.Verify(rm => rm.CreateAsync(It.IsAny<MoryxRole>()), Times.AtLeastOnce());
            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<MoryxUser>(), It.IsAny<string>()), Times.AtLeastOnce());
            permissionManagerMock.Verify(pm => pm.CreateAsync(It.IsAny<string>()), Times.AtLeastOnce());
        }
    }
}