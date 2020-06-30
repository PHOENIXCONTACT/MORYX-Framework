// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.TestTools.Test.Model;

namespace Marvin.TestTools.Test.Inheritance.Model
{
    public class SuperCarEntity : CarEntity
    {
        public virtual bool IsSuper { get; set; }
    }
}
