// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;

namespace Marvin.DependentTestModule
{
    [Plugin(LifeCycle.Singleton, typeof(ISimpleHelloWorldWcfSvcMgr), Name = ComponentName)]
    public class SimpleHelloWorldWcfSvcMgr : ISimpleHelloWorldWcfSvcMgr
    {
        internal const string ComponentName = "SimpleHelloWorldWcfSvcMgr";

        public void Initialize()
        {
        }

        public void Start()
        {
        }

        public string Hello(string name)
        {
            return string.IsNullOrEmpty(name) ? "Hello world!" : string.Format("Hello {0}!", name);
        }

        public string Throw(string name)
        {
            // The exception here is on purpose!

            throw new System.NotImplementedException();
        }
    }
}
