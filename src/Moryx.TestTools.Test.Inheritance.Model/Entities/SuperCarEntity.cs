// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.TestTools.Test.Model;

namespace Moryx.TestTools.Test.Inheritance.Model
{
    public class SuperCarEntity : CarEntity
    {
        public virtual bool IsSuper { get; set; }
    }
}
