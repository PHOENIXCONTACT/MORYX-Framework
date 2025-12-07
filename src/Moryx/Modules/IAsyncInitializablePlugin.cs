// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Modules;

/// <summary>
/// Based on <see cref="IAsyncInitializable"/> and <see cref="IAsyncPlugin"/> it offers a simple three-stage lifecycle.
/// Initialize, start and dispose.
/// So with this interface a module plugins can be initialized before their start.
/// </summary>
public interface IAsyncInitializablePlugin : IAsyncInitializable, IAsyncPlugin
{
}
