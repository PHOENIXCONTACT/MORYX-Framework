// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Threading;
using NUnit.Framework;

namespace Moryx.Tests.Threading
{
    [TestFixture]
    public class NonStackingTimerCallbackTests
    {
        private int _counter;

        private NonStackingTimerCallback _nonStackingTimerCallback;

        [SetUp]
        public void Setup()
        {
            _counter = 0;
        }

        [Test]
        public void Recursion()
        {
            _nonStackingTimerCallback = new NonStackingTimerCallback(RecursiveCallback);

            _nonStackingTimerCallback.Callback(null);

            Assert.That(_counter, Is.EqualTo(1));
        }

        [Test]
        public void CallByCall()
        {
            _nonStackingTimerCallback = new NonStackingTimerCallback(SimpleCallback);

            _nonStackingTimerCallback.Callback(null);
            _nonStackingTimerCallback.Callback(null);

            Assert.That(_counter, Is.EqualTo(2));
        }

        private void RecursiveCallback()
        {
            _counter++;

            if (_counter > 1)
            {
                return;
            }

            _nonStackingTimerCallback.Callback(null);
        }

        private void SimpleCallback()
        {
            _counter++;
        }
    }
}
