// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Linking;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Executes the linking protocol for all <see cref="OrderLink"/>s, orchestrating the configured
/// <see cref="OrderLinkingHook"/> plugins.
/// </summary>
internal interface IOrderLinkHandler : ILinkHandler<OrderReference>
{
}
