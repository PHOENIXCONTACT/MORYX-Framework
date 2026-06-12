// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Integrations.Orders.Integrator.Components;

/// <summary>
/// Orchestrates configured <see cref="Linking.LinkingHook"/> plugins for all
/// <see cref="IOrderLinkedMaterialContainer"/> resources at runtime.
/// </summary>
internal interface ILinkingHookManager : IAsyncPlugin
{
}