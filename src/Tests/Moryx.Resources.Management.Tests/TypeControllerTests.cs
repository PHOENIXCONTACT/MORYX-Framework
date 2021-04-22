// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moq;
using NUnit.Framework;

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
                .Returns(() => new[]
                {
                    typeof(SimpleResource),
                    typeof(DerivedResource),
                    typeof(ReferenceResource),
                    typeof(NonPublicResource),
                    typeof(ResourceWithImplicitApi)
                });

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
            var resource = new SimpleResource {Id = 1, Foo = 1337};

            // Act: Build Proxy
            var proxy = (ISimpleResource)_typeController.GetProxy(resource);
            var duplicate = (IDuplicateFoo)proxy;

            // Assert
            Assert.AreEqual(resource.Foo, proxy.Foo);
            Assert.AreEqual(resource.Foo, duplicate.Foo);
            proxy.Foo = 187;
            // duplicate.Foo = 10; ReadOnly but still uses the same property
            Assert.AreEqual(187, resource.Foo);
            Assert.AreEqual(187, duplicate.Foo);
        }

        [Test]
        public void UseBaseProxyForDerivedType()
        {
            // Arrange: Create instance
            var baseInstance = new SimpleResource { Id = 2};
            var instance = new DerivedResource {Id = 3};

            // Act: Build Proxy
            var baseProxy = (ISimpleResource) _typeController.GetProxy(baseInstance);
            var proxy = (ISimpleResource) _typeController.GetProxy(instance);

            // Assert: Make sure proxy is still the base type
            Assert.AreEqual(baseProxy.GetType(), proxy.GetType());
        }

        [Test]
        public void CallMethodOnProxy()
        {
            // Arrange: Create instance
            var instance = new SimpleResource { Id= 4, Foo = 10};

            // Act: Build proxy and call method
            var proxy = (ISimpleResource) _typeController.GetProxy(instance);
            var result = proxy.MultiplyFoo(3);
            proxy.MultiplyFoo(2, 10);

            // Assert: Check result and modified foo
            Assert.AreEqual(30, result);
            Assert.AreEqual(70, proxy.Foo);
        }

        [Test(Description = "Calls a method on proxy from interface which is declared within the ResourceAvailableAsAttribute")]
        public void CallMethodOnProxyFromNonPublicResourceApi()
        {
            // Arrange
            var instance = new SimpleResource { Id = 4, Foo = 10 };

            // Act: Build proxy
            var proxy = _typeController.GetProxy(instance);

            // Assert
            Assert.IsInstanceOf<INonResourceInterface>(proxy);
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
            Assert.AreEqual(40, result);
            Assert.AreEqual(40, proxy.Foo);
        }

        [Test(Description = "Test if implemented proxy supports inherited interfaces")]
        public void ProxySupportsInheritedInterfaces()
        {
            // Arrange: Create instance
            var instance = new ResourceWithImplicitApi();

            // Act:
            IResourceWithImplicitApi proxy = null;
            Assert.DoesNotThrow(() => proxy = (IResourceWithImplicitApi) _typeController.GetProxy(instance));

            // Assert:
            Assert.IsInstanceOf<IExtension>(proxy);
            Assert.AreEqual(20, proxy.Add(10));
        }

        [Test]
        public void ForwardEventsFromProxy()
        {
            // Arrange: Create instance and proxy
            var instance = new SimpleResource { Id = 6};
            var proxy = (ISimpleResource) _typeController.GetProxy(instance);

            // Act: Register listener and change foo
            object eventSender = null, eventSender2 = null;
            int eventValue = 0;
            var finallyEven = false;
            Assert.DoesNotThrow(() => instance.Foo = 10);
            EventHandler<int> eventHandler = (sender, foo) =>
            {
                eventSender = sender;
                eventValue = foo;
            };
            proxy.FooChanged += eventHandler;
            proxy.FooEven += (sender, b) => finallyEven = b;
            proxy.SomeEvent += (sender, args) => eventSender2 = sender;
            instance.Foo = 100;
            instance.RaiseEvent();
            proxy.FooChanged -= eventHandler;

            // Assert: Check if eventSender is not null and equals the proxy
            Assert.NotNull(eventSender);
            Assert.NotNull(eventSender2);
            Assert.AreNotEqual(0, eventValue);
            Assert.AreEqual(proxy, eventSender);
            Assert.AreEqual(100, eventValue);
            Assert.IsTrue(finallyEven);
        }

        [Test]
        public void AfterDisposeTheProxyIsDetached()
        {
            // Arrange: Create a proxy and register to an event
            var instance = new SimpleResource { Id = 7};
            var proxy = (ISimpleResource)_typeController.GetProxy(instance);
            var called = false;
            proxy.FooChanged += (sender, i) => called = true;
            instance.Foo = 10;
            Assert.IsTrue(called);

            // Act: Dispose the type controller and use the proxy again
            called = false;
            _typeController.Stop();
            instance.Foo = 10;

            // Assert: Event was not raised and proxy can no longer be used
            Assert.IsFalse(called);
            Assert.Throws<ProxyDetachedException>(() => proxy.MultiplyFoo(2));
        }

        [Test]
        public void ReplaceWithProxy()
        {
            // Arrange: Create instance and reference
            var ref1 = new DerivedResource { Id = 9, Foo = 20 };
            var ref2 = new SimpleResource { Id = 10, Foo = 30};
            var nonPub = new NonPublicResource {Name = "NonPublic"};
            var instance = new ReferenceResource
            {
                Id = 8,
                Reference = ref1,
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
            Assert.AreNotEqual(ref1, reference);
            Assert.AreNotEqual(ref2, references[0]);
            Assert.AreNotEqual(ref2, references2[0]);
            Assert.AreNotEqual(nonPub, nonPubProxy);
            Assert.AreEqual(20, reference.Foo);
            Assert.AreEqual(reference, methodRef);
            Assert.AreEqual(30, references[0].Foo);
            Assert.AreEqual(30, references2[0].Foo);
            Assert.NotNull(eventArgs);
            Assert.AreEqual(30, eventArgs.Foo);
            Assert.NotNull(eventArgs2);
            Assert.AreEqual(1, eventArgs2.Length);
            Assert.AreEqual(30, eventArgs2[0].Foo);
            Assert.AreEqual("NonPublic", nonPubProxy.Name);
            // Assert modifications of the setters
            Assert.AreEqual(instance.Reference, ref2);
            Assert.AreEqual(instance.References.Count(), 4);
            Assert.AreEqual(instance.References.ElementAt(1), ref1);
        }

    }
}
