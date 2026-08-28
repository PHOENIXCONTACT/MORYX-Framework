// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Integrations.Orders;

public interface IOrderIntegration
{
    IReadOnlyList<OrderReference> GetOrderReferences();
}
