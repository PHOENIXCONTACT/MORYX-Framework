// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools.Wcf.Tests
{
    internal class TestWcfClientFactory : BaseWcfClientFactory
    {
        public void Initialize(IWcfClientFactoryConfig config)
        {
            Initialize(config, null, new SimpleThreadContext());
        }
    }
}
