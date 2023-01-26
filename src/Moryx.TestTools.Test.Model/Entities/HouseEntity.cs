// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    public class HouseEntity : ModificationTrackedEntityBase
    {
        public virtual string Name { get; set; }

        public virtual int Size { get; set; }

        public virtual bool IsMethLabratory { get; set; }

        public virtual bool IsBurnedDown { get; set; }

        public virtual bool ToRent { get; set; }
    }
}
