// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Samples
{
    public class NeedleInstance : ProductInstance<NeedleType>
    {
        public NeedleRole Role { get; set; }
    }
}
