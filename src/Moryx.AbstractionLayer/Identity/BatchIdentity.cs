// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.AbstractionLayer.Identity
{
    /// <summary>
    /// Identity to assign on <see cref="ProductInstance"/> to identify batches
    /// </summary>
    public class BatchIdentity : IIdentity
    {
        /// <inheritdoc />
        public string Identifier { get; private set; }

        /// <summary>
        /// Creates a new batch identity with the given batch identifier
        /// </summary>
        public BatchIdentity(string identifier)
        {
            Identifier = identifier;
        }

        /// <inheritdoc />
        public void SetIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        /// <inheritdoc />
        public bool Equals(IIdentity other)
        {
            return other is BatchIdentity batchIdentity && batchIdentity.Identifier == Identifier;
        }
    }
}
