// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;
using Marvin.Modules;
using Marvin.Tools.Wcf;

namespace Marvin.DependentTestModule
{
    [DependencyRegistration(InstallerMode.All, Initializer = typeof(WcfBaseImporterSubInitializer))]
    [Plugin(LifeCycle.Transient, typeof(ISimpleHelloWorldWcfConnector), Name = nameof(SimpleHelloWorldWcfConnector))]
    [ExpectedConfig(typeof(SimpleHelloWorldWcfConnectorConfig))]
    public class SimpleHelloWorldWcfConnector : BasicWcfConnectorPlugin<SimpleHelloWorldWcfConnectorConfig, ISimpleHelloWorldWcfSvcMgr>, ISimpleHelloWorldWcfConnector
    {
        /// <summary>Injected property </summary>
        public ISimpleHelloWorldWcfSvcMgrFactory ServiceManagerFactory { get; set; }

        public override void Start()
        {
            ServiceManager = ServiceManagerFactory.Create(SimpleHelloWorldWcfSvcMgr.ComponentName);

            // Start wcf host with the binding specific service
            Service = HostFactory.CreateHost<ISimpleHelloWorldWcfService>(Config.ConnectorHost);
            Service.Start();
        }
    }
}
