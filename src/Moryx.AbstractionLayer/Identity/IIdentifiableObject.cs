// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Identity
{
    /// <summary>
    /// Base interface for objects with an <see cref="IIdentity"/>
    /// </summary>
    public interface IIdentifiableObject
    {
        /// <summary>
        /// Identity of this product
        /// </summary>
        IIdentity Identity { get; set; }
    }
}
