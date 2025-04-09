// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.Attributes;

namespace Moryx.TestTools.Test.Model
{
    public class CarEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual int Price { get; set; }

        public virtual byte[] Image { get; set; }

        [DateTimeKind(DateTimeKind.Local)]
        public DateTime ReleaseDateLocal { get; set; }

        public DateTime ReleaseDateUtc { get; set; }

        public virtual ICollection<WheelEntity> Wheels { get; set; }
    }
}
