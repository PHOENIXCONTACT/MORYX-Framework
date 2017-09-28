using System;
using Marvin.StateMachines;
using NUnit.Framework;

namespace Marvin.PlatformTools.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        /// <summary>
        /// This test should only test the initial state.
        /// The method will throw an exception if the wrong state is selected
        /// </summary>
        [Test]
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

        /// <summary>
        /// Test transition from A to B state. This state is only possible in <see cref="AState"/>
        /// </summary>
        [Test]
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

        /// <summary>
        /// Test transition from B to C state. This state is only possible in <see cref="BState"/>
        /// </summary>
        [Test]
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

        /// <summary>
        /// Test transition from C to A state. This state is only possible in <see cref="CState"/>
        /// </summary>
        [Test]
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

        /// <summary>
        /// Test will check the string representation of <see cref="StateMachine.Dump"/>
        /// </summary>
        [Test]
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

        private static MyContext CreateContext()
        {
            var context = new MyContext();
            StateMachine.Initialize(context).With<MyStateBase>();

            return context;
        }
    }
}
