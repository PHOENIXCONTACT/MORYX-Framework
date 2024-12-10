using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Resources.Management.Tests
{
    public interface IGenericMethodCall : IPublicResource
    {
        /// <summary>
        /// Get channel using specialized API
        /// </summary>
        IList<TChannel> GenericMethod<TChannel>(string identifier);
    }

    public interface IDerivedFromGeneric : IGenericMethodCall
    {
        bool SkippedByInheritance { get; }
    }

    public class ResourceWithGenericMethod : Resource, IGenericMethodCall, ISimpleResource, IDerivedFromGeneric
    {
        public int Foo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICapabilities Capabilities => throw new NotImplementedException();

        public bool SkippedByInheritance => throw new NotImplementedException();

        public event EventHandler<int> FooChanged;
        public event EventHandler<bool> FooEven;
        public event EventHandler SomeEvent;
        public event EventHandler<ICapabilities> CapabilitiesChanged;

        public IList<TChannel> GenericMethod<TChannel>(string identifier) => throw new NotImplementedException();

        public int MultiplyFoo(int factor) => throw new NotImplementedException();

        public int MultiplyFoo(int factor, ushort offset) => throw new NotImplementedException();

        public void RaiseEvent() => throw new NotImplementedException();
    }

    public interface IGenericBaseResourceInterface : IResource { }

    public class GenericBaseResource<T> : Resource, IGenericBaseResourceInterface { }

    public class InheritingFromGenericResource : GenericBaseResource<object> { }
}
