// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Integrations.Orders.Integrator.Components;
using Moryx.Runtime.Modules;

namespace Moryx.Material.Integrations.Orders.Integrator.Facade;

internal class OrderIntegrationFacade : FacadeBase, IOrderIntegration
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IOrderReferencesPool Pool { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public IReadOnlyList<OrderReference> GetOrderReferences() => Pool.GetAll();
}
