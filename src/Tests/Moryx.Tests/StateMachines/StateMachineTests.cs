// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Configuration;
using Moryx.StateMachines;
using NUnit.Framework;

namespace Moryx.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        [Test(Description = "Test the initial state. Throws an exception if the wrong state was selected.")]
        public void Inital()
        {
            //Arrange
            var context = CreateContext();

            // Assert
            Assert.DoesNotThrow(delegate
            {
                // Act
                context.State.Initial();
            });
        }

        [Test(Description = "Test transition from A to B state. This state is only possible in AState")]
        public void AtoBState()
        {
            // Arrange
            var context = CreateContext();

            // Act
            context.State.AtoB();

            // Assert
            Assert.IsTrue(context.AtoBTriggered);
            Assert.IsFalse(context.BtoCTriggered);
            Assert.IsFalse(context.CtoATriggered);
        }

        [Test(Description = "Test transition from B to C state. This state is only possible in BState")]
        public void BtoCState()
        {
            // Arrange
            var context = CreateContext();

            // Act
            context.State.AtoB();
            context.State.BtoC();

            // Assert
            Assert.IsTrue(context.AtoBTriggered);
            Assert.IsTrue(context.BtoCTriggered);
            Assert.IsFalse(context.CtoATriggered);
        }

        [Test(Description = "Test transition from C to A state. This state is only possible in CState")]
        public void CtoAState()
        {
            // Arrange
            var context = CreateContext();

            // Act
            context.State.AtoB();
            context.State.BtoC();
            context.State.CtoA();

            // Assert
            Assert.IsTrue(context.AtoBTriggered);
            Assert.IsTrue(context.BtoCTriggered);
            Assert.IsTrue(context.CtoATriggered);
        }

        [Test(Description = "Test will check the string representation of StateMachine.Dump")]
        public void Dump()
        {
            // Arrange
            var context = CreateContext();

            // Act
            var text = StateMachine.Dump(context.State);

            // Assert
            const string resultText = "Current: [AState (10)] - All: [AState (10)], [BState (20)], [CState (30)]";
            Assert.AreEqual(resultText, text);
        }

        [Test]
        public void Reload()
        {
            // Assert
            var context = new MyContext();
            StateMachine.Initialize(context).With<MyStateBase>();
            context.State.AtoB();

            // Act
            var reloadedContext = new MyContext();
            var bkey = context.State.Key;
            StateMachine.Reload(reloadedContext, bkey).With<MyStateBase>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => reloadedContext.State.Initial());

            // Act
            context.State.BtoC();

            // Assert
            Assert.IsTrue(context.BtoCTriggered);
            Assert.IsFalse(context.CtoATriggered);
        }

        [Test]
        public void ReloadUnknown()
        {
            // Arrange
            var context = new MyContext();

            // Assert
            Assert.Throws<InvalidOperationException>(delegate
            {
                // Act
                StateMachine.Reload(context, int.MaxValue).With<MyStateBase>();
            });
        }

        [Test(Description = "Brings the StateMachine to B and force to A without exit the current or enter the forced state.")]
        public void ForceState()
        {
            // Arrange
            var context = CreateContext();
            context.State.AtoB();

            // Act
            StateMachine.Force(context.State, MyStateBase.StateA);

            // Assert
            Assert.DoesNotThrow(() => context.State.AtoB());
        }

        [Test(Description = "Brings the StateMachine to B and force to A with exiting the current state.")]
        public void ForceStateWithExitCurrent()
        {
            // Arrange
            var context = CreateContext();
            context.State.AtoB();
            context.BExited = false;

            // Act
            StateMachine.Force(context.State, MyStateBase.StateA, true, false);

            // Assert
            Assert.IsTrue(context.BExited);
        }

        [Test(Description = "Brings the StateMachine to B and force to A with entering the forced state.")]
        public void ForceStateWithEnterForced()
        {
            // Arrange
            var context = CreateContext();
            context.State.AtoB();
            context.AEntered = false;

            // Act
            StateMachine.Force(context.State, MyStateBase.StateA, false, true);

            // Assert
            Assert.IsTrue(context.AEntered);
        }

        private static MyContext CreateContext()
        {
            var context = new MyContext();
            StateMachine.Initialize(context).With<MyStateBase>();

            return context;
        }
    }
}
