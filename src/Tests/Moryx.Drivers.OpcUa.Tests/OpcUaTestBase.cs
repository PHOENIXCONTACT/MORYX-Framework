// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.Drivers.OpcUa.Factories;
using Moryx.Drivers.OpcUa.Tests.Mocks;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests;

public class OpcUaTestBase
{
    private const ushort IndexNamespace1 = 1;
    private const ushort IndexNamespace2 = 2;

    protected Mock<ISession> _sessionMock;
    protected Dictionary<NodeId, ReferenceDescription> _rootNodes;
    protected ReferenceDescription _root;
    protected NamespaceTable _namespaceTable = CreateNamespaceTable();
    protected OpcUaDriver _driver;

    protected static NamespaceTable CreateNamespaceTable()
    {
        var result = new NamespaceTable();
        result.Append("http://pxcsdf");
        result.Append("http://namespace2");
        return result;
    }

    protected ReferenceDescriptionCollection CreateNodes(NamespaceTable namespaceTable)
    {
        var node1 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier1", IndexNamespace1, namespaceTable.GetString(IndexNamespace1), 0),
            DisplayName = "sdfa",
            NodeClass = NodeClass.Object,
            BrowseName = "browsename1",
        };

        var node2 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier2", IndexNamespace1, namespaceTable.GetString(IndexNamespace1), 0),
            DisplayName = "wers",
            NodeClass = NodeClass.Variable,
            BrowseName = "browsename2"
        };

        var node3 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier3", IndexNamespace2, namespaceTable.GetString(IndexNamespace2), 0),
            DisplayName = "wers",
            NodeClass = NodeClass.Variable,
            BrowseName = "browsename3"
        };

        _rootNodes = new Dictionary<NodeId, ReferenceDescription>
        {
            { ExpandedNodeId.ToNodeId(node1.NodeId, namespaceTable), node1 },
            { ExpandedNodeId.ToNodeId(node3.NodeId, namespaceTable), node3 }
        };

        return [node1, node2, node3];
    }

    public async Task BasicSetup()
    {
        ReflectionTool.TestMode = true;
        var nextRefs = CreateNodes(_namespaceTable);
        var rootRefs = new ReferenceDescriptionCollection { nextRefs[0], nextRefs[2] };
        var ns1Level1Refs = new ReferenceDescriptionCollection { nextRefs[1] };
        _sessionMock = new Mock<ISession>();
        ByteStringCollection? byteArray = null;
        _sessionMock.Setup(s => s.NamespaceUris).Returns(_namespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>()))
            .Returns(true);
        _sessionMock.Setup(s => s.BrowseAsync(null, null, new List<NodeId> { ObjectIds.RootFolder }, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>())).ReturnsAsync((null, byteArray, [rootRefs], null));
        _sessionMock.Setup(s => s.BrowseAsync(null, null, It.Is<List<NodeId>>(node => node.First().ToString() == "nsu=http://pxcsdf;s=identifier1"), It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>())).ReturnsAsync((null, byteArray, [ns1Level1Refs], null));

        var nextRefsDefault = new ReferenceDescriptionCollection();
        _sessionMock.Setup(s => s.BrowseAsync(null, null, It.Is<List<NodeId>>(x => x.First() != ObjectIds.RootFolder), It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>())).ReturnsAsync((null, byteArray, [nextRefsDefault], null));

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

        _sessionMock.Setup(s => s.SetPublishingModeAsync(null, It.IsAny<bool>(), It.IsAny<UInt32Collection>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SetPublishingModeResponse()
            {
                Results = [StatusCodes.Good],
                DiagnosticInfos = []
            });

        var result = new MonitoredItemCreateResult(0);
        MonitoredItemCreateResultCollection results = [result];
        _sessionMock.Setup(s => s.CreateMonitoredItemsAsync(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(),
            It.IsAny<MonitoredItemCreateRequestCollection>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateMonitoredItemsResponse() { Results = [result], DiagnosticInfos = [] });

        _sessionMock.Setup(s => s.WriteAsync(null, It.IsAny<WriteValueCollection>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WriteResponse() { Results = new StatusCodeCollection([StatusCodes.Good]), DiagnosticInfos = [] });
        var subscriptionFactoryMock = CreateSubscriptionFactoryMock(_sessionMock.Object);

        _driver = await CreateDriver(subscriptionFactoryMock.Object);
    }

    internal static Mock<SubscriptionFactory> CreateSubscriptionFactoryMock(ISession session)
    {
        var subscriptionFactoryMock = new Mock<SubscriptionFactory>();
        subscriptionFactoryMock.Setup(f => f.CreateSubscription(It.IsAny<Subscription>()))
            .Returns<Subscription>((fromSubscription) =>
            {
                var subscription = new TestSubscription(fromSubscription);
                subscription.InjectSession(session);
                return subscription;
            });
        return subscriptionFactoryMock;
    }

    internal static async Task<OpcUaDriver> CreateDriver(SubscriptionFactory subscriptionFactory)
    {
        var driver = new OpcUaDriver()
        {
            PublishingInterval = 1000,
            SamplingInterval = 1000,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
        };
        await ((IAsyncInitializablePlugin)driver).InitializeAsync();
        driver.SubscriptionFactory = subscriptionFactory;
        return driver;
    }

}
