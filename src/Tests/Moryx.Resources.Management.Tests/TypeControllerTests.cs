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

namespace Moryx.Resources.Management.Tests;

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
                typeof(ResourceWithImplicitApi),
                typeof(ExplicitEventResource),
                typeof(SharedEventNameResource),
                typeof(CustomDelegateResource)
            ]);

        _typeController = new ResourceTypeController
        {
            Container = containerMock.Object
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

    [Test(Description = "Events are unsubscribed from target after detach")]
    public void DetachedProxyStopsForwardingEvents()
    {
        // Arrange
        var instance = new SimpleResource { Id = 7 };
        var proxy = (ISimpleResource)_typeController.GetProxy(instance);
        var called = false;
        proxy.FooChanged += (sender, i) => called = true;
        instance.Foo = 10;
        Assert.That(called);

        // Act
        called = false;
        _typeController.Stop();
        instance.Foo = 20;

        // Assert
        Assert.That(called, Is.False);
    }

    [Test(Description = "Calling a method on a detached proxy throws ProxyDetachedException")]
    public void DetachedProxyThrowsOnMethodCall()
    {
        // Arrange
        var instance = new SimpleResource { Id = 31, Foo = 5 };
        var proxy = (ISimpleResource)_typeController.GetProxy(instance);
        _typeController.Stop();

        // Assert
        Assert.Throws<ProxyDetachedException>(() => proxy.MultiplyFoo(2));
    }

    [Test(Description = "Accessing a property on a detached proxy throws ProxyDetachedException")]
    public void DetachedProxyThrowsOnPropertyAccess()
    {
        // Arrange
        var instance = new SimpleResource { Id = 32, Foo = 5 };
        var proxy = (ISimpleResource)_typeController.GetProxy(instance);
        _typeController.Stop();

        // Assert
        Assert.Throws<ProxyDetachedException>(() => _ = proxy.Foo);
    }

    [Test]
    public void ProxySupportsGenericInterfaces()
    {
        // Arrange
        var resource = new ResourceWithGenericMethod { Id = 2, Name = "Some other Resource" };

        // Act
        var proxy = _typeController.GetProxy(resource);

        // Assert
        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy, Is.InstanceOf<ISimpleResource>());
        Assert.That(proxy, Is.InstanceOf<IGenericMethodCall>());
    }

    [Test(Description = "Generic methods on resource interfaces are forwarded through the proxy")]
    public void CallGenericMethodOnProxy()
    {
        // Arrange
        var resource = new ResourceWithGenericMethod { Id = 2, Name = "Some other Resource" };
        var proxy = (IGenericMethodCall)_typeController.GetProxy(resource);

        // Act
        var stringResult = proxy.GenericMethod("hello");
        var intResult = proxy.GenericMethod(42);

        // Assert
        Assert.That(stringResult, Is.EqualTo("hello"));
        Assert.That(intResult, Is.EqualTo(42));
    }

    [Test(Description = "Generic method returning a resource wraps it in a proxy")]
    public void GenericMethodReturningResourceIsProxied()
    {
        // Arrange
        var inner = new SimpleResource { Id = 50, Foo = 99 };
        var resource = new ResourceWithGenericMethod { Id = 2, Name = "Some other Resource" };
        var proxy = (IGenericMethodCall)_typeController.GetProxy(resource);

        // Act
        var result = proxy.GenericMethod<IResource>(inner);

        // Assert: result should be a proxy, not the raw resource
        Assert.That(result, Is.Not.SameAs(inner));
        Assert.That(result, Is.Not.InstanceOf<Resource>());
        Assert.That(result.Id, Is.EqualTo(50));
    }

    [Test(Description = "Exceptions thrown by the resource are not wrapped in TargetInvocationException")]
    public void ProxyUnwrapsTargetExceptions()
    {
        // Arrange
        var resource = new SimpleResource { Id = 33, Foo = 0 };
        var proxy = (ISimpleResource)_typeController.GetProxy(resource);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => proxy.ThrowingMethod());
    }

    [Test]
    public void ProxyBuilderSkipsGenericBaseTypes()
    {
        // Arrange
        var resource = new InheritingFromGenericResource { Id = 42, Name = "A non generic resource inheriting from a generic base type" };

        // Act
        var proxy = _typeController.GetProxy(resource);

        // Assert
        Assert.That(proxy, Is.Not.Null);
        Assert.That(typeof(GenericBaseResource<object>).IsAssignableFrom(proxy.GetType()), Is.False);
    }

    [Test]
    public void ProxifyGenericInterfaceFromFacade()
    {
        // Arrange
        var resource = new ResourceWithGenericMethod { Id = 2, Name = "Some other Resource" };

        // Act
        var proxy = resource.Proxify<IGenericMethodCall>(_typeController);

        // Assert
        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy, Is.InstanceOf<IGenericMethodCall>());
    }

    [Test(Description = "Explicit interface property get/set should work through proxy")]
    public void ReadAndWriteExplicitInterfaceProperty()
    {
        // Arrange
        var instance = new ExplicitEventResource { Id = 20 };

        // Act
        var proxy = (IExplicitEventResource)_typeController.GetProxy(instance);
        proxy.Bar = 42;

        // Assert
        Assert.That(proxy.Bar, Is.EqualTo(42));
    }

    [Test(Description = "Explicit interface method should work through proxy")]
    public void CallExplicitInterfaceMethodOnProxy()
    {
        // Arrange
        var instance = new ExplicitEventResource { Id = 21 };
        var proxy = (IExplicitEventResource)_typeController.GetProxy(instance);
        proxy.Bar = 5;

        // Act
        var result = proxy.DoubleBar();

        // Assert
        Assert.That(result, Is.EqualTo(10));
        Assert.That(proxy.Bar, Is.EqualTo(10));
    }

    [Test(Description = "Explicit interface event should be forwarded through proxy")]
    public void ForwardExplicitInterfaceEventFromProxy()
    {
        // Arrange
        var instance = new ExplicitEventResource { Id = 22 };
        var proxy = (IExplicitEventResource)_typeController.GetProxy(instance);

        object eventSender = null;
        var eventValue = 0;
        proxy.BarChanged += (sender, value) =>
        {
            eventSender = sender;
            eventValue = value;
        };

        // Act
        proxy.Bar = 99;

        // Assert
        Assert.That(eventSender, Is.Not.Null);
        Assert.That(eventSender, Is.EqualTo(proxy));
        Assert.That(eventValue, Is.EqualTo(99));
    }

    [Test(Description = "Two interfaces with the same event name are forwarded independently")]
    public void ForwardSameNamedEventsFromDifferentInterfaces()
    {
        // Arrange
        var instance = new SharedEventNameResource { Id = 40 };
        var proxy = _typeController.GetProxy(instance);
        var firstProxy = (IFirstEventSource)proxy;
        var secondProxy = (ISecondEventSource)proxy;

        int firstValue = 0, secondValue = 0;
        firstProxy.StatusChanged += (_, v) => firstValue = v;
        secondProxy.StatusChanged += (_, v) => secondValue = v;

        // Act: raise only the first event
        instance.RaiseFirst(42);

        // Assert: only the first handler fires
        Assert.That(firstValue, Is.EqualTo(42));
        Assert.That(secondValue, Is.EqualTo(0));

        // Act: raise only the second event
        instance.RaiseSecond(99);

        // Assert: only the second handler fires
        Assert.That(secondValue, Is.EqualTo(99));
        Assert.That(firstValue, Is.EqualTo(42));
    }

    [Test(Description = "Events with custom delegate types throw NotSupportedException during proxy creation")]
    public void ThrowOnCustomDelegateEvent()
    {
        // Arrange
        var instance = new CustomDelegateResource { Id = 41 };

        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() => _typeController.GetProxy(instance));
        Assert.That(ex!.Message, Does.Contain(nameof(ICustomDelegateResource.StatusReport)));
    }

    [Test(Description = "Proxy returns null for Name and Capabilities without throwing ProxyDetachedException")]
    public void NullPropertiesDoNotThrowDetached()
    {
        // Arrange
        var instance = new SimpleResource { Id = 30, Name = null };
        instance.UpdateCapabilities(null);
        var proxy = _typeController.GetProxy(instance);

        // Assert
        // Null values are returned, not a ProxyDetachedException
        Assert.That(proxy.Name, Is.Null);
        Assert.That(proxy.Capabilities, Is.Null);
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
