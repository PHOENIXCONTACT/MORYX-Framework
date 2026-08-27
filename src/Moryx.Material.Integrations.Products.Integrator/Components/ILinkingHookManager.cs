// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

/// <summary>
/// Orchestrates configured <see cref="Linking.ILinkingHook"/> plugins for all
/// <see cref="IProductLinkedMaterialContainer"/> resources at runtime.
/// </summary>
internal interface ILinkingHookManager : IAsyncPlugin
{
    void ProcessLinkingRequested(RequestContext context);

    void ProcessLinkingApplied(AppliedContext context);
}