using System;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public interface IMyResource : IPublicResource
    {
        int Foo { get; set; }

        int MultiplyFoo(int factor);

        int MultiplyFoo(int factor, ushort offset);

        event EventHandler<int> FooChanged;

        event EventHandler<bool> FooEven;

        void RaiseEvent();

        event EventHandler SomeEvent;
    }

    public interface IDuplicateFoo : IPublicResource
    {
        int Foo { get; }
    }
}