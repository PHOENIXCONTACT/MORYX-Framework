// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer;

namespace Marvin.Products.Samples
{
    public class NeedleInstance : ProductInstance<NeedleType>
    {
        public NeedleRole Role { get; set; }
    }
}
