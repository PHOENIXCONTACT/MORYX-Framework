// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

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
    protected Mock<ISession> _sessionMock;
    protected Dictionary<NodeId, ReferenceDescription> _nodes;
    protected ReferenceDescription _root;
    protected NamespaceTable _namespaceTable;
    protected OpcUaDriver _driver;

    protected (ushort, ushort) CreateNamespaceTable()
    {
        _namespaceTable = new NamespaceTable();
        var indexNamespace1 = (ushort)_namespaceTable.Append("http://pxcsdf");
        var indexNamespace2 = (ushort)_namespaceTable.Append("http://namespace2");
        return (indexNamespace1, indexNamespace2);
    }

    protected ReferenceDescriptionCollection CreateNodes()
    {
        var (indexNamespace1, indexNamespace2) = CreateNamespaceTable();
        var node1 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier1", indexNamespace1, _namespaceTable.GetString(indexNamespace1), 0),
            DisplayName = "sdfa",
            NodeClass = NodeClass.Object,
            BrowseName = "browsename1"
        };

        var node2 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier2", indexNamespace1, _namespaceTable.GetString(indexNamespace1), 0),
            DisplayName = "wers",
            NodeClass = NodeClass.Variable,
            BrowseName = "browsename2"
        };

        var node3 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier3", indexNamespace2, _namespaceTable.GetString(indexNamespace2), 0),
            DisplayName = "wers",
            NodeClass = NodeClass.Variable,
            BrowseName = "browsename3"
        };

        var root = new ReferenceDescription()
        {
            NodeId = ObjectIds.RootFolder,
            DisplayName = "root",
            NodeClass = NodeClass.Object,
            BrowseName = "root"
        };
        _nodes = new Dictionary<NodeId, ReferenceDescription>
        {
            { ExpandedNodeId.ToNodeId(node1.NodeId, _namespaceTable), node1 },
            { ExpandedNodeId.ToNodeId(node2.NodeId, _namespaceTable), node2 },
            { ExpandedNodeId.ToNodeId(node3.NodeId, _namespaceTable), node3 }
        };
        return [node1, node2, node3];
    }

    public void BasicSetup()
    {
        ReflectionTool.TestMode = true;
        var nextRefs = CreateNodes();

        _sessionMock = new Mock<ISession>();
        byte[] byteArray = null;
        _sessionMock.Setup(s => s.NamespaceUris).Returns(_namespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Returns(true);
        _sessionMock.Setup(s => s.Browse(null, null, ObjectIds.RootFolder, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), out byteArray, out nextRefs));

        var nextRefsDefault = new ReferenceDescriptionCollection();
        _sessionMock.Setup(s => s.Browse(null, null, It.Is<NodeId>(x => x != ObjectIds.RootFolder), It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
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
    }

    protected void CreateDriver()
    {
        _driver = new OpcUaDriver()
        {
            PublishingInterval = 1000,
            SamplingInterval = 1000,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
        };
        ((IInitializable)_driver).Initialize();

    }

}
