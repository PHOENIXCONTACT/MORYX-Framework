using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Processes.Endpoints;

namespace Moryx.ControlSystem.Processes.Endpoints.Tests;

[TestFixture]
public class ProcessEngineControllerGroupStreamTests
{
    [Test]
    public void GroupStream_ShouldNotThrow_WhenGroupIsNotProcessHolderGroup()
    {
        // Arrange: Return a group that is not a concrete ProcessHolderGroup
        var resourceManagement = new Mock<IResourceManagement>();
        var group = Mock.Of<IProcessHolderGroup>();

        resourceManagement.Setup(rm => rm.GetResourcesUnsafe<IProcessHolderGroup>(
                It.IsAny<Func<IProcessHolderGroup, bool>>()))
            .Returns(new[] { group });

        resourceManagement.Setup(rm => rm.GetResourcesUnsafe<IProcessHolderPosition>(
                It.IsAny<Func<IProcessHolderPosition, bool>>()))
            .Returns(Array.Empty<IProcessHolderPosition>());

        var controller = new ProcessEngineController(
            Mock.Of<IProcessControl>(),
            Mock.Of<IProductManagement>(),
            resourceManagement.Object,
            Mock.Of<IJobManagement>());

        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services
        };

        using var responseBody = new MemoryStream();
        httpContext.Response.Body = responseBody;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act & Assert: The stream must not throw an exception
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        Assert.DoesNotThrowAsync(() => controller.GroupStream(cts.Token));
    }  
}