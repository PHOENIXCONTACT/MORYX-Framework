// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer.Identity
{
    /// <summary>
    /// Base interface for objects with an <see cref="IIdentity"/>
    /// </summary>
    public interface IIdentifiableObject
    {
        /// <summary>
        /// Identity of this product
        /// </summary>
        IIdentity Identity { get; }
    }
}
