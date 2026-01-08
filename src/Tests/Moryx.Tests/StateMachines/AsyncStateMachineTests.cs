// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Moryx.StateMachines;
using Moryx.Tests.AsyncTestMachine;
using NUnit.Framework;

namespace Moryx.Tests;

[TestFixture]
public class AsyncStateMachineTests
{
    [Test(Description = "Test the initial state. Throws an exception if the wrong state was selected.")]
    public async Task Initial()
    {
        //Arrange
        var context = await CreateContext();

        // Assert
        Assert.DoesNotThrowAsync(async delegate
        {
            // Act
            await context.State.InitialAsync();
        });
    }

    [Test(Description = "Test transition from A to B state. This state is only possible in AState")]
    public async Task AtoBState()
    {
        // Arrange
        var context = await CreateContext();

        // Act
        await context.State.AtoBAsync();

        // Assert
        Assert.That(context.AtoBTriggered);
        Assert.That(context.BtoCTriggered, Is.False);
        Assert.That(context.CtoATriggered, Is.False);
    }

    [Test(Description = "Test transition from B to C state. This state is only possible in BState")]
    public async Task BtoCState()
    {
        // Arrange
        var context = await CreateContext();

        // Act
        await context.State.AtoBAsync();
        await context.State.BtoCAsync();

        // Assert
        Assert.That(context.AtoBTriggered);
        Assert.That(context.BtoCTriggered);
        Assert.That(context.CtoATriggered, Is.False);
    }

    [Test(Description = "Test transition from C to A state. This state is only possible in CState")]
    public async Task CtoAState()
    {
        // Arrange
        var context = await CreateContext();

        // Act
        await context.State.AtoBAsync();
        await context.State.BtoCAsync();
        await context.State.CtoAAsync();

        // Assert
        Assert.That(context.AtoBTriggered);
        Assert.That(context.BtoCTriggered);
        Assert.That(context.CtoATriggered);
    }


    [Test]
    public async Task Reload()
    {
        // Assert
        var context = new MyAsyncContext();
        await StateMachine.InitializeAsync(context).WithAsync<MyAsyncStateBase>();
        await context.State.AtoBAsync();

        // Act
        var reloadedContext = new MyAsyncContext();
        var bkey = context.State.Key;
        await StateMachine.ReloadAsync(reloadedContext, bkey).WithAsync<MyAsyncStateBase>();

        // Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => reloadedContext.State.InitialAsync());

        // Act
        await context.State.BtoCAsync();

        // Assert
        Assert.That(context.BtoCTriggered);
        Assert.That(context.CtoATriggered, Is.False);
    }

    [Test(Description = "Brings the StateMachine to B and force to A without exit the current or enter the forced state.")]
    public async Task ForceStateAsync()
    {
        // Arrange
        var context = await CreateContext();
        await context.State.AtoBAsync();

        // Act
        await StateMachine.ForceAsync(context.State, MyStateBase.StateA);

        // Assert
        Assert.DoesNotThrowAsync(() => context.State.AtoBAsync());
    }

    [Test(Description = "Brings the StateMachine to B and force to A with exiting the current state.")]
    public async Task ForceStateWithExitCurrent()
    {
        // Arrange
        var context = await CreateContext();
        await context.State.AtoBAsync();
        context.BExited = false;

        // Act
        await StateMachine.ForceAsync(context.State, MyStateBase.StateA, true, false);

        // Assert
        Assert.That(context.BExited);
    }

    [Test(Description = "Brings the StateMachine to B and force to A with entering the forced state.")]
    public async Task ForceStateWithEnterForced()
    {
        // Arrange
        var context = await CreateContext();
        await context.State.AtoBAsync();
        context.AEntered = false;

        // Act
        await StateMachine.ForceAsync(context.State, MyStateBase.StateA, false, true);

        // Assert
        Assert.That(context.AEntered);
    }

    private static async Task<MyAsyncContext> CreateContext()
    {
        var context = new MyAsyncContext();
        await StateMachine.InitializeAsync(context).WithAsync<MyAsyncStateBase>();

        return context;
    }
}