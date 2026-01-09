// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Drivers.OpcUa.Factories;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests.Mocks;

internal class FakeSubscriptionFactory : SubscriptionFactory
{
    public override Subscription CreateSubscription(Subscription fromSubscription)
    {
        return base.CreateSubscription(fromSubscription);
    }
}
