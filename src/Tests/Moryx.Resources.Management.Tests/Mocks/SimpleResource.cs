// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests
{
    public interface IDuplicateFoo : IResource
    {
        int Foo { get; }
    }

    public interface ISimpleResource : IResource
    {
        int Foo { get; set; }

        int MultiplyFoo(int factor);

        int MultiplyFoo(int factor, ushort offset);

        event EventHandler<int> FooChanged;

        event EventHandler<bool> FooEven;

        void RaiseEvent();

        event EventHandler SomeEvent;
    }

    public interface INonResourceInterface
    {
        void Validate();
    }

    [ResourceAvailableAs(typeof(INonResourceInterface))]
    public class SimpleResource : Resource, ISimpleResource, IDuplicateFoo, INonResourceInterface
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

        int ISimpleResource.MultiplyFoo(int factor, ushort offset)
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

        public void UpdateCapabilities(ICapabilities capabilities)
        {
            Capabilities = capabilities;
        }
        public void Validate()
        {
        }
    }
}
