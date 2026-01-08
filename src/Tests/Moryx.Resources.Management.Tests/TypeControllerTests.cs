// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moq;
using NUnit.Framework;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Resources.Management.Tests
{
    [TestFixture]
    public class TypeControllerTests
    {
        private IResourceTypeController _typeController;

        [SetUp]
        public void Setup()
        {
            // Mock of the container
            var containerMock = new Mock<IContainer>();
            containerMock.Setup(c => c.GetRegisteredImplementations(It.IsAny<Type>()))
                .Returns(() =>
                [
                    typeof(SimpleResource),
                    typeof(DerivedResource),
                    typeof(ReferenceResource),
                    typeof(NonPublicResource),
                    typeof(ResourceWithImplicitApi)
                ]);

            _typeController = new ResourceTypeController
            {
                Container = containerMock.Object,
                ProxyBuilder = new ResourceProxyBuilder()
            };
            _typeController.Start();
        }

        [TearDown]
        public void AfterTest()
        {
            _typeController.Stop();
        }

        [Test]
        public void ReadAndWriteProperties()
        {
            // Arrange: Create instance
            var resource = new SimpleResource { Id = 1, Foo = 1337 };

            // Act: Build Proxy
            var proxy = (ISimpleResource)_typeController.GetProxy(resource);
            var duplicate = (IDuplicateFoo)proxy;

            // Assert
            Assert.That(proxy.Foo, Is.EqualTo(resource.Foo));
            Assert.That(duplicate.Foo, Is.EqualTo(resource.Foo));
            proxy.Foo = 187;
            // duplicate.Foo = 10; ReadOnly but still uses the same property
            Assert.That(resource.Foo, Is.EqualTo(187));
            Assert.That(duplicate.Foo, Is.EqualTo(187));
        }

        [Test]
        public void UseBaseProxyForDerivedType()
        {
            // Arrange: Create instance
            var baseInstance = new SimpleResource { Id = 2 };
            var instance = new DerivedResource { Id = 3 };

            // Act: Build Proxy
            var baseProxy = (ISimpleResource)_typeController.GetProxy(baseInstance);
            var proxy = (ISimpleResource)_typeController.GetProxy(instance);

            // Assert: Make sure proxy is still the base type
            Assert.That(proxy.GetType(), Is.EqualTo(baseProxy.GetType()));
        }

        [Test]
        public void UseNewProxyForDerivedTypeWithNewInterface()
        {
            // Arrange: Create instance
            var baseInstance = new SimpleResource { Id = 2 };
            var instance = new DerivedResourceWithNewProxy { Id = 3 };

            // Act: Build Proxy
            var baseProxy = _typeController.GetProxy(baseInstance);
            var proxy = _typeController.GetProxy(instance);

            // Assert: Make sure proxy is still the base type
            Assert.That(baseProxy.GetType(), Is.Not.EqualTo(proxy.GetType()));
        }

        [Test]
        public void CallMethodOnProxy()
        {
            // Arrange: Create instance
            var instance = new SimpleResource { Id = 4, Foo = 10 };

            // Act: Build proxy and call method
            var proxy = (ISimpleResource)_typeController.GetProxy(instance);
            var result = proxy.MultiplyFoo(3);
            proxy.MultiplyFoo(2, 10);

            // Assert: Check result and modified foo
            Assert.That(result, Is.EqualTo(30));
            Assert.That(proxy.Foo, Is.EqualTo(70));
        }

        [Test(Description = "Calls a method on proxy from interface which is declared within the ResourceAvailableAsAttribute")]
        public void CallMethodOnProxyFromNonPublicResourceApi()
        {
            // Arrange
            var instance = new SimpleResource { Id = 4, Foo = 10 };

            // Act: Build proxy
            var proxy = _typeController.GetProxy(instance);

            // Assert
            Assert.That(proxy, Is.InstanceOf<INonResourceInterface>());
            Assert.DoesNotThrow(() => ((INonResourceInterface)proxy).Validate());
        }

        [Test]
        public void CallMethodOnDerivedType()
        {
            // Arrange: Create instance
            var instance = new DerivedResource { Id = 5, Foo = 10 };

            // Act: Build proxy and call method
            var proxy = (ISimpleResource)_typeController.GetProxy(instance);
            var result = proxy.MultiplyFoo(3);

            // Assert: Check result and modified foo
            Assert.That(result, Is.EqualTo(40));
            Assert.That(proxy.Foo, Is.EqualTo(40));
        }

        [Test(Description = "Test if implemented proxy supports inherited interfaces")]
        public void ProxySupportsInheritedInterfaces()
        {
            // Arrange: Create instance
            var instance = new ResourceWithImplicitApi();

            // Act:
            IResourceWithImplicitApi proxy = null;
            Assert.DoesNotThrow(() => proxy = (IResourceWithImplicitApi)_typeController.GetProxy(instance));

            // Assert:
            Assert.That(proxy, Is.InstanceOf<IExtension>());
            Assert.That(proxy.Add(10), Is.EqualTo(20));
        }

        [Test]
        public void ForwardEventsFromProxy()
        {
            // Arrange: Create instance and proxy
            var instance = new SimpleResource { Id = 6 };
            var proxy = (ISimpleResource)_typeController.GetProxy(instance);

            // Act: Register listener and change foo
            object eventSender = null, eventSender2 = null, eventSender3 = null;
            int eventValue = 0;
            ICapabilities capabilitiesValue = null;
            var finallyEven = false;
            Assert.DoesNotThrow(() => instance.Foo = 10);
            EventHandler<int> eventHandler = (sender, foo) =>
            {
                eventSender = sender;
                eventValue = foo;
            };
            EventHandler<ICapabilities> eventHandler2 = (sender, capabilities) =>
            {
                eventSender3 = sender;
                capabilitiesValue = capabilities;
            };

            proxy.FooChanged += eventHandler;
            proxy.FooEven += (sender, b) => finallyEven = b;
            proxy.SomeEvent += (sender, args) => eventSender2 = sender;
            proxy.CapabilitiesChanged += eventHandler2;
            instance.Foo = 100;
            instance.RaiseEvent();
            instance.UpdateCapabilities(NullCapabilities.Instance);
            proxy.FooChanged -= eventHandler;
            proxy.CapabilitiesChanged -= eventHandler2;

            // Assert: Check if eventSender is not null and equals the proxy
            Assert.That(eventSender, Is.Not.Null);
            Assert.That(eventSender2, Is.Not.Null);
            Assert.That(eventSender3, Is.Not.Null);
            Assert.That(eventValue, Is.Not.EqualTo(0));
            Assert.That(eventSender, Is.EqualTo(proxy));
            Assert.That(eventSender3, Is.EqualTo(proxy));
            Assert.That(capabilitiesValue, Is.EqualTo(NullCapabilities.Instance));
            Assert.That(eventValue, Is.EqualTo(100));
            Assert.That(finallyEven);
        }

        [Test]
        public void AfterDisposeTheProxyIsDetached()
        {
            // Arrange: Create a proxy and register to an event
            var instance = new SimpleResource { Id = 7 };
            var proxy = (ISimpleResource)_typeController.GetProxy(instance);
            var called = false;
            proxy.FooChanged += (sender, i) => called = true;
            instance.Foo = 10;
            Assert.That(called);

            // Act: Dispose the type controller and use the proxy again
            called = false;
            _typeController.Stop();
            instance.Foo = 10;

            // Assert: Event was not raised and proxy can no longer be used
            Assert.That(called, Is.False);
            Assert.Throws<ProxyDetachedException>(() => proxy.MultiplyFoo(2));
        }

        [Test]
        public void ProxyBuilderFiltersGenericInterfaces()
        {
            // Arrange
            var driver = new ResourceWithGenericMethod { Id = 2, Name = "Some other Resource" };

            // Act
            var proxy = (ISimpleResource)_typeController.GetProxy(driver);

            // Assert
            Assert.That(proxy, Is.Not.Null);
            Assert.That(typeof(IGenericMethodCall).IsAssignableFrom(proxy.GetType()), Is.False);
        }

        [Test]
        public void ProxyBuilderSkipsGenericBaseTypes()
        {
            // Arrange
            var driver = new InheritingFromGenericResource { Id = 42, Name = "A non generic resource inheriting from a generic base type" };

            // Act
            var proxy = _typeController.GetProxy(driver);

            // Assert
            Assert.That(proxy, Is.Not.Null);
            Assert.That(typeof(GenericBaseResource<object>).IsAssignableFrom(proxy.GetType()), Is.False);
        }

        [Test]
        public void FacadeExceptionForGenericProxy()
        {
            // Arrange
            var driver = new ResourceWithGenericMethod { Id = 2, Name = "Some other Resource" };

            // Assert
            Assert.Throws<NotSupportedException>(() => ResourceExtensions.Proxify<IGenericMethodCall>(driver, _typeController));
        }

        [Test]
        public void ReplaceWithProxy()
        {
            // Arrange: Create instance and reference
            var ref1 = new DerivedResource { Id = 9, Foo = 20 };
            var ref2 = new SimpleResource { Id = 10, Foo = 30 };
            var nonPub = new NonPublicResource { Name = "NonPublic" };
            var instance = new ReferenceResource
            {
                Id = 8,
                Reference = ref1,
                Reference2 = null,
                EvenMoreReferences = null,
                NonPublic = nonPub
            };
            instance.References = new ReferenceCollection<ISimpleResource>(instance,
                instance.GetType().GetProperty(nameof(ReferenceResource.References)), new List<IResource>())
            {
                ref2
            };

            // Act: Convert to proxy and access the reference
            var proxy = (IReferenceResource)_typeController.GetProxy(instance);
            var reference = proxy.Reference;
            var methodRef = proxy.GetReference();
            var references = proxy.MoreReferences.ToArray();
            var references2 = proxy.GetReferences();
            var nonPubProxy = proxy.NonPublic;

            ISimpleResource eventArgs = null;
            proxy.ReferenceChanged += (sender, resource) => eventArgs = resource;
            ISimpleResource[] eventArgs2 = null;
            proxy.SomeChanged += (sender, resources) => eventArgs2 = resources;

            // Act: Set resource property through proxy
            proxy.Reference = references[0];
            proxy.SetReference(reference);
            proxy.SetMany(references);

            // Make sure all references where replaced with proxies
            Assert.That(ref1, Is.Not.EqualTo(reference));
            Assert.That(ref2, Is.Not.EqualTo(references[0]));
            Assert.That(ref2, Is.Not.EqualTo(references2[0]));
            Assert.That(nonPub, Is.Not.EqualTo(nonPubProxy));
            Assert.That(reference.Foo, Is.EqualTo(20));
            Assert.That(methodRef, Is.EqualTo(reference));
            Assert.That(references[0].Foo, Is.EqualTo(30));
            Assert.That(references2[0].Foo, Is.EqualTo(30));
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Foo, Is.EqualTo(30));
            Assert.That(eventArgs2, Is.Not.Null);
            Assert.That(eventArgs2.Length, Is.EqualTo(1));
            Assert.That(eventArgs2[0].Foo, Is.EqualTo(30));
            Assert.That(nonPubProxy.Name, Is.EqualTo("NonPublic"));
            // Assert modifications of the setters
            Assert.That(ref2, Is.EqualTo(instance.Reference));
            Assert.That(3, Is.EqualTo(instance.References.Count));
            Assert.That(ref1, Is.EqualTo(instance.References.ElementAt(1)));
            // Make sure null references work
            Assert.DoesNotThrow(() => _ = proxy.Reference2);
            Assert.DoesNotThrow(() => proxy.Reference2 = null);
            Assert.DoesNotThrow(() => _ = proxy.EvenMoreReferences);
            Assert.DoesNotThrow(() => proxy.EvenMoreReferences = null);
        }

    }
}
