// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Tools.Wcf;

namespace Marvin.TestModule
{
    [DependencyRegistration(InstallerMode.All)]
    [Plugin(LifeCycle.Transient, typeof(IHelloWorldWcfConnector), Name = PluginName)]
    [ExpectedConfig(typeof(HelloWorldWcfConnectorConfig))]
    public class HelloWorldWcfConnector : BasicNetTcpConnectorPlugin<HelloWorldWcfConnectorConfig, IHelloWorldWcfSvcMgr, IHelloWorldWcfService>, IHelloWorldWcfConnector
    {
        internal const string PluginName = "HelloWorldWcfConnector";

        public void TriggerHelloCallback(string name)
        {
            ParallelOperations.ScheduleExecution(() => ServiceManager.HelloCallback(name), 100, Timeout.Infinite);
        }
    }
}
