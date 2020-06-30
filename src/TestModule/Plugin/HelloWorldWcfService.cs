// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ServiceModel;
using System.Threading;
using Moryx.Container;
using Moryx.Tools.Wcf;

namespace Moryx.TestModule
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    [Plugin(LifeCycle.Singleton, typeof(IHelloWorldWcfService))]
    public class HelloWorldWcfService : SessionService<IHelloWorldWcfServiceCallback, IHelloWorldWcfSvcMgr>, IHelloWorldWcfService
    {
        public const string ServerVersion = "4.7.1.1";
        public const string MinClientVersion = "4.2.0.0";
        public const string ServiceName = "HelloWorldWcfService";

        public string Hello(string name)
        {
            return ServiceManager.Hello(name);
        }

        public string Throw(string name)
        {
            return ServiceManager.Throw(name);
        }

        public void TriggerHelloCallback(string name)
        {
            ServiceManager.TriggerHelloCallback(name);
        }

        public void TriggerThrowCallback(string name)
        {
            ServiceManager.TriggerThrowCallback(name);
        }

        public void HelloCallback(string message)
        {
            Callback.HelloCallback(message);
        }

        public string ThrowCallback(string message)
        {
            return Callback.ThrowCallback(message);
        }

        public void DeferredDisconnect(int waitInMs)
        {
            new Timer(DeferredDisconnectCallback, this, waitInMs, -1);
        }

        private void DeferredDisconnectCallback(object state)
        {
            Close();
        }
    }
}
