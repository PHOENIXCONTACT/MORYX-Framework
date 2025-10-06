// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using Moq;
using Moryx.Drivers.OpcUa.DriverStates;
using Moryx.Modules;
using Moryx.Tools;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class HandlingCyclicNodesTests : OpcUaTestBase
{
    [SetUp]
    public void Setup()
    {
        ReflectionTool.TestMode = true;
        var nextRefs = CreateNodes();

        _sessionMock = new Mock<ISession>();
        var byteArray = Array.Empty<byte>();
        _sessionMock.Setup(s => s.NamespaceUris).Returns(_namespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Returns(true);
        _sessionMock.Setup(s => s.Browse(null, null, ObjectIds.RootFolder, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), out byteArray, out nextRefs));

        var nextRefsDefault = new ReferenceDescriptionCollection() { nextRefs[1] };
        _sessionMock.Setup(s => s.Browse(null, null, It.Is<NodeId>(x => x != ObjectIds.RootFolder &&
            x.ToString() != ExpandedNodeId.ToNodeId(nextRefs[0].NodeId, _namespaceTable).ToString()), It.IsAny<uint>(), It.IsAny<BrowseDirection>(),
            ReferenceTypeIds.HierarchicalReferences, true, It.IsAny<uint>(), out byteArray, out nextRefsDefault));

        var cycle = new ReferenceDescriptionCollection();
        _sessionMock.Setup(s => s.Browse(null, null, ExpandedNodeId.ToNodeId(nextRefs[0].NodeId, _namespaceTable), It.IsAny<uint>(),
            It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), out byteArray, out nextRefsDefault));

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

    [Test(Description = "Do not show cyclic nodes in the UI")]
    public void DoNotShowCyclicNodesInTheUi()
    {
        //Arrange
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e is RunningState)
            {
                //Assert II
                Assert.That(_nodes.Count - 1, Is.EqualTo(_driver.Nodes.Count), "Number of browsed nodes doesn't fit");
                Assert.DoesNotThrow(() => JsonSerializer.Serialize(_driver.Nodes));
                wait.Set();
            }
        };

        //Act
        ((IPlugin)_driver).Start();

        //Assert
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), Is.True, "Driver was not running");

    }
}
