// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Drivers.OpcUa.States;
using Moryx.Tools;
using NUnit.Framework;
using Opc.Ua.Client;
using Opc.Ua;
using Moryx.Modules;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class HandleContinuationPoint : OpcUaTestBase
{
    private ReferenceDescriptionCollection _references;

    [SetUp]
    public async Task SetUp()
    {
        _references = CreateNodes(_namespaceTable);

        ReflectionTool.TestMode = true;

        _sessionMock = new Mock<ISession>();
        var byteArray = Array.Empty<byte>();
        _sessionMock.Setup(s => s.NamespaceUris).Returns(_namespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Returns(true);

        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Callback((Subscription sub) =>
        {
            var prop = sub.GetType().GetProperties().FirstOrDefault(propInfo => propInfo.Name.Equals(nameof(sub.Session)));
            prop?.SetValue(sub, _sessionMock.Object);
        });

        _sessionMock.Setup(s => s.CreateSubscriptionAsync(
            null,
            It.IsAny<double>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            It.IsAny<byte>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateSubscriptionResponse()
            {
                SubscriptionId = 12,
                RevisedPublishingInterval = 12,
                RevisedLifetimeCount = 5,
                RevisedMaxKeepAliveCount = 5
            });

        var result = new MonitoredItemCreateResult(0);
        MonitoredItemCreateResultCollection results = [result];
        _sessionMock.Setup(s => s.CreateMonitoredItemsAsync(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(),
            It.IsAny<MonitoredItemCreateRequestCollection>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateMonitoredItemsResponse() { Results = [result], DiagnosticInfos = [] });

        _sessionMock
            .Setup(s => s.SetPublishingModeAsync(null, It.IsAny<bool>(), It.IsAny<UInt32Collection>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SetPublishingModeResponse()
            {
                Results = [StatusCodes.Good],
                DiagnosticInfos = []
            });

        var subscriptionFactoryMock = CreateSubscriptionFactoryMock(_sessionMock.Object);

        _driver = await CreateDriver(subscriptionFactoryMock.Object);
        _driver._session = _sessionMock.Object;
    }

    [Test]
    public async Task TestContinuationPoint()
    {
        //Arrange
        var byteArray = new ByteStringCollection([[0, 1]]);
        var nextRefs1 = new ReferenceDescriptionCollection() { _references[0] };
        var nextRefs2 = new ReferenceDescriptionCollection();
        foreach (var nextRef in _references.GetRange(1, _references.Count - 1))
        {
            nextRefs2.Add(nextRef);
        }

        _sessionMock.Setup(s => s.BrowseAsync(null, null, new List<NodeId> { ObjectIds.RootFolder }, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>())).ReturnsAsync((null, byteArray, [nextRefs1], null));

        ByteStringCollection? furtherContinuationPoint = null;
        _sessionMock
            .Setup(s => s.BrowseNextAsync(null, byteArray, false, CancellationToken.None))
            .ReturnsAsync((null, furtherContinuationPoint, [nextRefs2], null));

        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e is RunningState)
            {
                //Assert II
                Assert.That(_driver.Nodes.Count, Is.EqualTo(_references.Count));
                wait.Set();
            }
        };

        //Act
        await ((IAsyncPlugin)_driver).StartAsync();

        //Assert
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), Is.True, "Driver was not running");
    }

}
