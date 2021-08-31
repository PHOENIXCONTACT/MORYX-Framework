// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    public class CarEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual int Price { get; set; }

        public virtual byte[] Image { get; set; }

        public virtual ICollection<WheelEntity> Wheels { get; set; }
    }
}
