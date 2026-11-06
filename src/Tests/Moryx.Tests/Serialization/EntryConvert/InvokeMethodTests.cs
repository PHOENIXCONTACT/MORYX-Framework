// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with the invocation behaviours for different method types within the
/// <see cref="EntryConvert"/>
/// </summary>
[TestFixture]
public class InvokeMethodTests
{
    private readonly MethodClassDummy _dummies = new();

    [Test]
    public void CreateInstanceWithConstructor()
    {
        // Arrange
        var dummyType = typeof(MethodClassDummy);

        // Act
        var constructors = EntryConvert.EncodeConstructors(dummyType).ToArray();
        var constructor = constructors.First(c => c.Parameters.SubEntries.Count == 1);
        constructor.Parameters.SubEntry("number").Value.Current = "42";
        var instance = (MethodClassDummy)EntryConvert.CreateInstance(dummyType, constructor);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Number, Is.EqualTo(42));
        Assert.That(instance.Name, Is.EqualTo(string.Empty), "EntryConvert did not pick the correct overload");
    }

    [Test]
    public void PublicMethodsWillBeInvoked()
    {
        // Act
        var entry = EntryConvert.InvokeMethod(_dummies, NoParamsMethodEntry(nameof(MethodClassDummy.InvocablePublic)));

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test]
    public void InternalMethodsWillBeInvoked()
    {
        // Act
        var entry = EntryConvert.InvokeMethod(_dummies, NoParamsMethodEntry(nameof(MethodClassDummy.InvocableInternal)));

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test]
    public void ProtectedMethodsWillNotBeInvoked()
    {
        // Act / Assert
        Assert.Throws<MissingMethodException>(
            () => EntryConvert.InvokeMethod(_dummies, NoParamsMethodEntry("NonInvocableProtected")));
    }

    [Test]
    public void PrivateMethodsWillNotBeInvoked()
    {
        // Act / Assert
        Assert.Throws<MissingMethodException>(
            () => EntryConvert.InvokeMethod(_dummies, NoParamsMethodEntry("NonInvocablePrivate")));
    }

    [Test(Description = "Async method without result invoked synchronous")]
    public void AsyncMethodWithoutResultInvokedSynchronous()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(MethodClassDummy.AsyncWithoutResult));

        // Act
        var entry = EntryConvert.InvokeMethod(_dummies, parameters);

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test(Description = "Async method with result invoked synchronous")]
    public void AsyncMethodWithResultInvokedSynchronous()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(MethodClassDummy.AsyncWithStringResult));

        // Act
        var entry = EntryConvert.InvokeMethod(_dummies, parameters);

        // Assert
        Assert.That(entry, Is.Not.Null);
        Assert.That(entry.Value.Current, Is.EqualTo("Test"));
    }

    [Test(Description = "Async method without result invoked async")]
    public async Task AsyncMethodWithoutResultInvokedAsync()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(MethodClassDummy.AsyncWithoutResult));

        // Act
        var entry = await EntryConvert.InvokeMethodAsync(_dummies, parameters);

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test(Description = "Async method with result invoked async")]
    public async Task AsyncMethodWithResultInvokedAsync()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(MethodClassDummy.AsyncWithStringResult));

        // Act
        var entry = await EntryConvert.InvokeMethodAsync(_dummies, parameters);

        // Assert
        Assert.That(entry, Is.Not.Null);
        Assert.That(entry.Value.Current, Is.EqualTo("Test"));
    }

    private static MethodEntry NoParamsAsyncMethodEntry(string name)
    {
        return new MethodEntry
        {
            Name = name,
            Parameters = new Entry(),
            IsAsync = true
        };
    }

    private static MethodEntry NoParamsMethodEntry(string name)
    {
        return new MethodEntry
        {
            Name = name,
            Parameters = new Entry()
        };
    }
}
