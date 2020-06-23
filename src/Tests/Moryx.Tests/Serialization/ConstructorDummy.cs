// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Tests
{
    public class ConstructorDummy
    {
        public ConstructorDummy(int foo) : this(foo, String.Empty)
        {
        }

        public ConstructorDummy(int foo, string text)
        {
            Foo = foo;
            Text = text;
        }

        public int Foo { get; }

        public string Text { get; }
    }
}
