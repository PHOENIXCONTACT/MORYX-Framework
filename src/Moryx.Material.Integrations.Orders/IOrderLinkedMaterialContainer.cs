// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders;

namespace Moryx.Material.Integrations.Orders;

/// <summary>
/// A <see cref="IMaterialContainer"/> with the ability to be linked to an <see cref="Order"/>.
/// </summary>
/// <remarks>
/// <see cref="IMaterialContainer"/>s are intended to be linked to at most one <see cref="Order"/> at a time.
/// </remarks>
public interface IOrderLinkedMaterialContainer : IMaterialContainer
{
    /// <summary>
    /// Link to the order of this container.
    /// </summary>
    OrderLink OrderLink { get; }
}
