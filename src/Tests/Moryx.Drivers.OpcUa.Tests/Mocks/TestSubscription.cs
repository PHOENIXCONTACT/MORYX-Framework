// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests.Mocks;

public class TestSubscription : Subscription
{
    public TestSubscription(Subscription subscription)
    {
        PublishingEnabled = subscription.PublishingEnabled;
        PublishingInterval = subscription.PublishingInterval;
        LifetimeCount = subscription.LifetimeCount;
    }

    public void InjectSession(ISession session)
    {
        Session = session;
    }
}
