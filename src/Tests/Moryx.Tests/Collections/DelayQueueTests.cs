// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Collections;
using Moryx.Threading;
using NUnit.Framework;

namespace Moryx.Tests.Collections
{
    [TestFixture, Explicit]
    public class DelayQueueTests
    {
        private class DummyMessage
        {
        }

        private const int Delay = 300;

        private DelayQueue<DummyMessage> _queue;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly List<long> _times = new List<long>();

        [SetUp]
        public void CreateQueue()
        {
            _queue = new DelayQueue<DummyMessage>(new ParallelOperations(new NullLogger<ParallelOperations>()));
            _queue.Dequeued += OnQueueDequeued;
            _queue.Start(Delay);
            _stopwatch.Start();
        }

        [TearDown]
        public void ResetWatch()
        {
            _queue.Stop();
            _stopwatch.Reset();
            _times.Clear();
        }

        private void OnQueueDequeued(object sender, DummyMessage dummyMessage)
        {
            _stopwatch.Stop();
            _times.Add(_stopwatch.ElapsedMilliseconds);
            _stopwatch.Start();
        }

        [TestCase(-1, Description = "Enqueue directly after creation")]
        [TestCase(0.5, Description = "Enqueue with too short wait period")]
        [TestCase(1.5, Description = "Enqueue with sufficient wait period")]
        public void EnqueueAfterCreation(double delayFactor)
        {
            // Arrange
            var delay = (int)(Delay * delayFactor);
            if (delay > 0)
                Thread.Sleep(delay);

            // Act
            _queue.Enqueue(new DummyMessage());
            Thread.Sleep(Delay + 100);

            // Assert
            if (delay < Delay)
            {
                Assert.GreaterOrEqual(_times[0], Delay - 2);
                Assert.Less(_times[0], 2 * Delay);
            }
            else
            {
                Assert.GreaterOrEqual(_times[0], delay - 2);
                Assert.Less(_times[0], delay + Delay);
            }
        }

        [TestCase(true, Description = "Send two messages directly after each other after the inital wait time has passed")]
        [TestCase(false, Description = "Send two messages directly after each other before the inital wait time has passed")]
        public void DoubleMessage(bool queueReady)
        {
            // Arrange
            const int initalDelay = Delay + 100;
            if (queueReady)
                Thread.Sleep(initalDelay);

            // Act
            _queue.Enqueue(new DummyMessage());
            _queue.Enqueue(new DummyMessage());

            // Assert
            Thread.Sleep((queueReady ? initalDelay : Delay) + 2 * Delay);
            // Validate first message
            if (queueReady)
            {
                Assert.LessOrEqual(_times[0], Delay + initalDelay);
            }
            else
            {
                Assert.GreaterOrEqual(_times[0], Delay - 2);
            }
            // Validate second message
            var timeDiff = _times[1] - _times[0];
            Assert.GreaterOrEqual(timeDiff, Delay - 2);
            Assert.LessOrEqual(timeDiff, 2 * Delay);
        }

        [TestCase(-1, 10, Description = "Direct burst of 3 objects in 10ms interval")]
        [TestCase(50, 5, Description = "Burst of 3 objects after 50ms in 5ms interval")]
        [TestCase(10, 50, Description = "Burst of 3 objects after 10ms in 50ms interval")]
        public void EnqueueBurst(int initialDelay, int messageDelay)
        {
            // Arrange
            if (initialDelay > 0)
                Thread.Sleep(initialDelay);

            // Act
            const int loops = 4;
            for (var i = 1; i <= loops; i++)
            {
                _queue.Enqueue(new DummyMessage());
                Thread.Sleep(messageDelay);
            }
            Thread.Sleep(initialDelay + (loops + 1) * Delay);

            // Assert
            Assert.AreEqual(loops, _times.Count);
            for (var i = 0; i < _times.Count; i++)
            {
                var diff = i == 0 ? _times[i] : _times[i] - _times[i - 1];
                Assert.GreaterOrEqual(diff, Delay - 2);
            }
        }

        [Test(Description = "Stop queue without ever using it and make sure it throws an exeption and does not send anything")]
        public void StoppedQueueThrowsException()
        {
            // Arrange
            _queue.Stop();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _queue.Enqueue(new DummyMessage()));
            Assert.AreEqual(0, _times.Count);
        }

        [Test(Description = ""), Explicit]
        public void InterruptQueue()
        {
            // Arrange
            var localThreading = new ParallelOperations(new NullLogger<ParallelOperations>());

            // Act
            localThreading.ScheduleExecution(_queue.Enqueue, new DummyMessage(), (int)(Delay * 0.5), -1);
            localThreading.ScheduleExecution(_queue.Enqueue, new DummyMessage(), (int)(Delay * 1.5), -1);

            Thread.Sleep(Delay * 2);
            _queue.Stop();

            var preClear = _times.Count;
            _times.Clear();
            Thread.Sleep(Delay * 2);

            // Assert
            Assert.GreaterOrEqual(preClear, 0);
            Assert.AreEqual(0, _times.Count);
        }
    }
}
