// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Container;

namespace Moryx.DependentTestModule
{
    public class WcfBaseImporterSubInitializer : ISubInitializer
    {
        public void Initialize(IContainer container)
        {
            if (!container.GetRegisteredImplementations(typeof(ISimpleHelloWorldWcfSvcMgrFactory)).Any())
                container.Register<ISimpleHelloWorldWcfSvcMgrFactory>();
        }
    }
}
