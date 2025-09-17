// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of a <see cref="ProductType"/>
    /// </summary>
    public class DummyProductType : ProductType
    {
        /// <inheritdoc />
        protected override ProductInstance Instantiate()
        {
            return new DummyProductInstance();
        }
/// <inheritdoc/>

        public override bool Equals(object obj)
        {
            var toCompareWith = obj as DummyProductType;
            if (toCompareWith == null)
                return false;

            return toCompareWith.Id == Id && toCompareWith.Name == Name && toCompareWith.State == State 
                && ((toCompareWith.Identity is null && Identity is null) || toCompareWith.Identity.Equals(Identity));
        }
    }


    /// <summary>
    /// Dummy implementation of a <see cref="ProductType"/> with Product Parts 
    /// </summary>
    public class DummyProductTypeWithParts : DummyProductType
    {
        /// <inheritdoc />
        protected override ProductInstance Instantiate()
        {
            return new DummyProductInstance();
        }

        /// <summary>
        /// Dummy ProductPartLink
        /// </summary>
        public DummyProductPartLink ProductPartLink { get; set; }

        /// <summary>
        /// Dummy ProductPartLink enumerable
        /// </summary>
        public IEnumerable<DummyProductPartLink> ProductPartLinkEnumerable { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var toCompareWith = obj as DummyProductTypeWithParts;
            if (toCompareWith == null)
                return false;

            return base.Equals(toCompareWith) && 
                ((toCompareWith.ProductPartLink is null && ProductPartLink is null) || 
                toCompareWith.ProductPartLink.Equals(ProductPartLink))
                && ((toCompareWith.ProductPartLinkEnumerable is null && ProductPartLinkEnumerable is null) || 
                Enumerable.SequenceEqual<DummyProductPartLink>(toCompareWith.ProductPartLinkEnumerable, ProductPartLinkEnumerable));
        }
    }


    /// <summary>
    /// Dummy implementation of a <see cref="ProductType"/> with Files
    /// </summary>
    public class DummyProductTypeWithFiles : DummyProductType
    {
        /// <inheritdoc />
        protected override ProductInstance Instantiate()
        {
            return new DummyProductInstance();
        }

        /// <summary>
        /// First dummy ProductFile
        /// </summary>
        public ProductFile FirstProductFile { get; set; }

        /// <summary>
        /// Second dummy ProductFile
        /// </summary>
        public ProductFile SecondProductFile { get; set; }
        
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var toCompareWith = obj as DummyProductTypeWithFiles;
            if (toCompareWith == null)
                return false;

            return base.Equals(toCompareWith) &&
                (toCompareWith.FirstProductFile is null && FirstProductFile is null ||
                FirstProductFile.GetType().GetProperties().All(prop => prop.GetValue(toCompareWith.FirstProductFile) == prop.GetValue(FirstProductFile)))
                && (toCompareWith.SecondProductFile is null && SecondProductFile is null ||
                SecondProductFile.GetType().GetProperties().All(prop => prop.GetValue(toCompareWith.SecondProductFile) == prop.GetValue(SecondProductFile)));
        }
    }
}
