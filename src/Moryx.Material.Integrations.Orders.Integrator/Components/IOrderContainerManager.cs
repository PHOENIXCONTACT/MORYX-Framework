// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Lifecycle manager for <see cref="IOrderLinkedMaterialContainer"/> resources.
/// Handles activation/deactivation of <see cref="OrderReference"/> instances based on
/// integration startup, shutdown and order business events, and cascades unlinking when
/// containers are removed.
/// </summary>
internal interface IOrderContainerManager : IAsyncPlugin
{
}