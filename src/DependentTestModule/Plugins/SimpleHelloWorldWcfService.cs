// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using Moryx.Container;

namespace Moryx.DependentTestModule
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    [Plugin(LifeCycle.Singleton, typeof(ISimpleHelloWorldWcfService))]
    public class SimpleHelloWorldWcfService : ISimpleHelloWorldWcfService
    {
        public const string ServerVersion = "1.2.0";

        public const string ServiceName = "SimpleHelloWorldWcfService";

        /// <summary>
        /// ImportServiceManager injected by castle
        /// </summary>
        [Named(SimpleHelloWorldWcfSvcMgr.ComponentName)]
        public ISimpleHelloWorldWcfSvcMgr ServiceManager { get; set; }

        public string Hello(string name)
        {
            return ServiceManager.Hello(name);
        }

        public string Throw(string name)
        {
            return ServiceManager.Throw(name);
        }
    }
}
