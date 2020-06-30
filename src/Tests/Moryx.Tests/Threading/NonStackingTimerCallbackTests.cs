// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Threading;
using NUnit.Framework;

namespace Marvin.Tests.Threading
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

            Assert.AreEqual(1, _counter);
        }

        [Test]
        public void CallByCall()
        {
            _nonStackingTimerCallback = new NonStackingTimerCallback(SimpleCallback);

            _nonStackingTimerCallback.Callback(null);
            _nonStackingTimerCallback.Callback(null);

            Assert.AreEqual(2, _counter);
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
