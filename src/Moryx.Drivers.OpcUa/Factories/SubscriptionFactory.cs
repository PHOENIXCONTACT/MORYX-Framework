// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Factories;

internal class SubscriptionFactory
{
    public virtual Subscription CreateSubscription(Subscription fromSubscription)
    {
        return fromSubscription;
    }
}
