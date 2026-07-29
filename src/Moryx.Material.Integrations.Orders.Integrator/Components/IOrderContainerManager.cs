// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Lifecycle manager for <see cref="IOrderLinkedMaterialContainer"/> resources.
/// Handles container registration and container lifecycle events.
/// It also forwards order-linking events to be handled.
/// </summary>
internal interface IOrderContainerManager : IAsyncPlugin
{
}
