// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Unit tests for <see cref="EntryConvert"/> method invocation
/// </summary>
[TestFixture]
public class EntryConvertInvokeMethodTests
{
    private readonly EntrySerialize_Methods _sut = new();

    [Test]
    public void PublicMethodsWillBeInvoked()
    {
        // Act
        var entry = EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry(nameof(EntrySerialize_Methods.InvocablePublic)));

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test]
    public void InternalMethodsWillBeInvoked()
    {
        // Act
        var entry = EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry(nameof(EntrySerialize_Methods.InvocableInternal)));

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test]
    public void ProtectedMethodsWillNotBeInvoked()
    {
        // Act / Assert
        Assert.Throws<System.MissingMethodException>(
            () => EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry("NonInvocableProtected")));
    }

    [Test]
    public void PrivateMethodsWillNotBeInvoked()
    {
        // Act / Assert
        Assert.Throws<System.MissingMethodException>(
            () => EntryConvert.InvokeMethod(_sut, NoParamsMethodEntry("NonInvocablePrivate")));
    }

    [Test(Description = "Async method without result invoked synchronous")]
    public void AsyncMethodWithoutResultInvokedSynchronous()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(EntrySerialize_Methods.AsyncWithoutResult));

        // Act
        var entry = EntryConvert.InvokeMethod(_sut, parameters);

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test(Description = "Async method with result invoked synchronous")]
    public void AsyncMethodWithResultInvokedSynchronous()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(EntrySerialize_Methods.AsyncWithStringResult));

        // Act
        var entry = EntryConvert.InvokeMethod(_sut, parameters);

        // Assert
        Assert.That(entry, Is.Not.Null);
        Assert.That(entry.Value.Current, Is.EqualTo("Test"));
    }

    [Test(Description = "Async method without result invoked async")]
    public async Task AsyncMethodWithoutResultInvokedAsync()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(EntrySerialize_Methods.AsyncWithoutResult));

        // Act
        var entry = await EntryConvert.InvokeMethodAsync(_sut, parameters);

        // Assert
        Assert.That(entry, Is.Null);
    }

    [Test(Description = "Async method with result invoked async")]
    public async Task AsyncMethodWithResultInvokedAsync()
    {
        // Arrange
        var parameters = NoParamsAsyncMethodEntry(nameof(EntrySerialize_Methods.AsyncWithStringResult));

        // Act
        var entry = await EntryConvert.InvokeMethodAsync(_sut, parameters);

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