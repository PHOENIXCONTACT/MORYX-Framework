// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Bindings;

namespace Moryx.ProcessData.Adapter.NotificationPublisher
{
    internal class NotificationBindingResolverFactory : BindingResolverFactory
    {
        protected override IBindingResolverChain CreateBaseResolver(string baseKey)
        {
            switch (baseKey)
            {
                case "Notification":
                    return new NullResolver();
                default:
                    return null;
            }
        }
    }
}
