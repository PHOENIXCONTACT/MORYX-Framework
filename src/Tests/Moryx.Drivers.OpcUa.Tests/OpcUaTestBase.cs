// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Drivers;
using Moryx.Drivers.OpcUa.Factories;
using Moryx.Drivers.OpcUa.Tests.Mocks;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Tools;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests;

public class OpcUaTestBase
{
    private const ushort IndexNamespace1 = 1;
    private const ushort IndexNamespace2 = 2;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Mock<ISession> _sessionMock;
    protected Dictionary<NodeId, ReferenceDescription> _rootNodes;
    protected OpcUaDriver _driver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected NamespaceTable NamespaceTable { get => CreateNamespaceTable(); }

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
            NodeId = new ExpandedNodeId("identifier1", IndexNamespace1),
            DisplayName = "sdfa",
            NodeClass = NodeClass.Object,
            BrowseName = "browsename1",
        };

        var node2 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier2", IndexNamespace1),
            DisplayName = "wers",
            NodeClass = NodeClass.Variable,
            BrowseName = "browsename2"
        };

        var node3 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier3", IndexNamespace2),
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
        var nextRefs = CreateNodes(NamespaceTable);
        var rootRefs = new ReferenceDescriptionCollection { nextRefs[0], nextRefs[2] };
        var ns1Level1Refs = new ReferenceDescriptionCollection { nextRefs[1] };
        _sessionMock = new Mock<ISession>();
        _sessionMock.Setup(s => s.NamespaceUris).Returns(NamespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>()))
            .Returns(true);

        var nextRefsDefault = new ReferenceDescriptionCollection();

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
        var nodeReaderFactoryMock = CreateNodeReaderFactoryMock(_rootNodes);

        _driver = await CreateDriver(subscriptionFactoryMock.Object, nodeReaderFactoryMock.Object);
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

    internal Mock<NodeReaderFactory> CreateNodeReaderFactoryMock(Dictionary<NodeId, ReferenceDescription> nodes)
    {
        var nodeReaderMock = new Mock<IOpcUaNodeReader>();
        nodeReaderMock.Setup(nr => nr.ReadNodeAsync(It.IsAny<string>(), It.IsAny<NamespaceTable>(), It.IsAny<ISession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string nodeId, NamespaceTable _, ISession __, CancellationToken ___) =>
            {
                var referenceDescription = nodes
                    .FirstOrDefault((kv) => kv.Key.ToString() == ExpandedNodeId.Parse(nodeId, NamespaceTable).ToString())
                    .Value;

                if (referenceDescription is not null)
                {
                    var node = new OpcUaNode(_driver, null, referenceDescription.NodeId, NamespaceTable)
                    {
                        DisplayName = referenceDescription.DisplayName.ToString(),
                        BrowseName = referenceDescription.BrowseName,
                        NodeClass = referenceDescription.NodeClass
                    };
                    return node;
                }
                return null;
            });

        var nodeReaderFactoryMock = new Mock<NodeReaderFactory>();
        nodeReaderFactoryMock.Setup(f => f.CreateNodeReader(It.IsAny<IModuleLogger>(), It.IsAny<IOpcUaDriver>()))
            .Returns(() => nodeReaderMock.Object);
        return nodeReaderFactoryMock;
    }

    internal static async Task<OpcUaDriver> CreateDriver(SubscriptionFactory subscriptionFactory, NodeReaderFactory nodeReaderFactory)
    {
        var driver = new OpcUaDriver()
        {
            PublishingInterval = 1000,
            SamplingInterval = 1000,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
        };
        await ((IAsyncInitializablePlugin)driver).InitializeAsync();
        driver.SubscriptionFactory = subscriptionFactory;
        driver.NodeReaderFactory = nodeReaderFactory;
        return driver;
    }

    protected void AwaitRunningState(Driver driver, long seconds, Action? onSuccess = null)
    {
        var wait = new AutoResetEvent(false);
        driver.StateChanged += (_, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                wait.Set();
            }
        };
        var set = wait.WaitOne(TimeSpan.FromSeconds(seconds));
        if (set || driver.CurrentState.Classification == StateClassification.Running)
        {
            onSuccess?.Invoke();
        }
        else
        {
            Assert.Fail($"RunningState not entered within {seconds} seconds");
        }
    }
}
