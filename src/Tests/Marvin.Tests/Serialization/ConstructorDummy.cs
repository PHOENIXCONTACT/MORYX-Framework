using System;

namespace Marvin.Tests
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