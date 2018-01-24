using System;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public class MyResource : PublicResource, IMyResource, IDuplicateFoo
    {
        private int _foo;

        public int Foo
        {
            get { return _foo; }
            set
            {
                _foo = value;
                FooChanged?.Invoke(this, _foo);
                FooEven?.Invoke(this, value % 2 == 0);
            }
        }

        public virtual int MultiplyFoo(int factor)
        {
            return Foo *= factor;
        }

        public int MultiplyFoo(int factor, ushort offset)
        {
            return Foo = Foo * factor + offset;
        }

        public event EventHandler<int> FooChanged;

        public event EventHandler<bool> FooEven;

        public event EventHandler SomeEvent;
        public void RaiseEvent()
        {
            SomeEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DerivedResource : MyResource
    {
        public override int MultiplyFoo(int factor)
        {
            return Foo *= (factor + 1);
        }
    }
}