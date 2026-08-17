// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

/// <summary>
/// Lifecycle manager for <see cref="IProductLinkedMaterialContainer"/> resources.
/// Handles container registration and container lifecycle events.
/// It also forwards product-linking events to be handled by the <see cref="ILinkingHookManager"/>.
/// </summary>
internal interface IProductContainerManager : IAsyncPlugin
{
}