// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;

namespace Marvin.AbstractionLayer.UI.Tests
{
    public class DetailsComponentSelector : DetailsComponentSelector<IDetailsComponent>
    {
        public DetailsComponentSelector(IContainer container) : base(container)
        {
        }
    }
}
