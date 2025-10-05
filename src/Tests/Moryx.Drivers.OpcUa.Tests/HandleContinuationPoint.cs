// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moq;
using Moryx.Tools;
using NUnit.Framework;
using Opc.Ua.Client;
using Opc.Ua;
using Moryx.Modules;
using Moryx.Drivers.OpcUa.DriverStates;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class HandleContinuationPoint : OpcUaTestBase
{
    private ReferenceDescriptionCollection _references;

    [SetUp]
    public void SetUp()
    {
        ReflectionTool.TestMode = true;
        _references = CreateNodes();

        _sessionMock = new Mock<ISession>();
        var byteArray = new byte[0];
        _sessionMock.Setup(s => s.NamespaceUris).Returns(_namespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Returns(true);

        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Callback((Subscription sub) =>
        {
            var prop = sub.GetType().GetProperties().FirstOrDefault(propInfo => propInfo.Name.Equals(nameof(sub.Session)));
            prop.SetValue(sub, _sessionMock.Object);
        });

        uint subscriptionId = 12;
        double revisedPublishingInterval = 12;
        uint revisedLifetimeCounter = 5;
        uint revisedKeepAliveCount = 5;
        _sessionMock.Setup(s => s.CreateSubscription(
            null,
            It.IsAny<double>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            It.IsAny<byte>(),
            out subscriptionId,
            out revisedPublishingInterval,
            out revisedLifetimeCounter,
            out revisedKeepAliveCount)).Callback(() =>
            {
                subscriptionId = 12;
            });

        var result = new MonitoredItemCreateResult(0);
        MonitoredItemCreateResultCollection results = [result];
        DiagnosticInfoCollection diagnosticInfos;
        _sessionMock.Setup(s => s.CreateMonitoredItems(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(),
            It.IsAny<MonitoredItemCreateRequestCollection>(), out results, out diagnosticInfos));

        CreateDriver();
        _driver._session = _sessionMock.Object;
    }

    [Test]
    public void TestContinuationPoint()
    {
        //Arrange
        var byteArray = new byte[] { 0, 1 };
        var nextRefs1 = new ReferenceDescriptionCollection() { _references[0] };
        var nextRefs2 = new ReferenceDescriptionCollection();
        foreach (var nextRef in _references.GetRange(1, _references.Count - 1))
        {
            nextRefs2.Add(nextRef);
        }

        _sessionMock.Setup(s => s.Browse(null, null, ObjectIds.RootFolder, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
           true, It.IsAny<uint>(), out byteArray, out nextRefs1));

        byte[] furtherContinuationPoint = null;
        _sessionMock.Setup(s => s.BrowseNext(null, false, byteArray, out furtherContinuationPoint, out nextRefs2));

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
        ((IPlugin)_driver).Start();

        //Assert
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), Is.True, "Driver was not running");
    }

}
