// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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

    protected Task BasicSetup()
    {
        ReflectionTool.TestMode = true;
        var nextRefs = CreateNodes(_namespaceTable);
        var rootRefs = new ReferenceDescriptionCollection { nextRefs[0], nextRefs[2] };
        var ns1Level1Refs = new ReferenceDescriptionCollection { nextRefs[1] };

        _sessionMock = new Mock<ISession>();
        byte[]? byteArray = null;
        _sessionMock.Setup(s => s.NamespaceUris).Returns(_namespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Returns(true);
        _sessionMock.Setup(s => s.Browse(null, null, ObjectIds.RootFolder, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), out byteArray, out rootRefs));
        _sessionMock.Setup(s => s.Browse(null, null, It.Is<NodeId>(node => node.ToString() == "nsu=http://pxcsdf;s=identifier1"), It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), out byteArray, out ns1Level1Refs));

        var nextRefsDefault = new ReferenceDescriptionCollection();
        _sessionMock.Setup(s => s.Browse(null, null, It.Is<NodeId>(x => x != ObjectIds.RootFolder), It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), out byteArray, out nextRefsDefault));

        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Callback((Subscription sub) =>
        {
            var prop = sub.GetType().GetProperties().FirstOrDefault(propInfo => propInfo.Name.Equals(nameof(sub.Session)));
            prop?.SetValue(sub, _sessionMock.Object);
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

        return CreateDriver();
    }

    protected Task CreateDriver()
    {
        _driver = new OpcUaDriver()
        {
            PublishingInterval = 1000,
            SamplingInterval = 1000,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
        };
        return ((IAsyncInitializable)_driver).InitializeAsync();
    }

}
