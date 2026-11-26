// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Lifecycle manager for <see cref="IOrderLinkedMaterialContainer"/> resources.
/// Handles container registration and container lifecycle events.
/// It also attaches the <see cref="IOrderLinkHandler"/> to the <see cref="OrderLink"/> of each container.
/// </summary>
internal interface IOrderContainerManager : IAsyncPlugin
{
}
