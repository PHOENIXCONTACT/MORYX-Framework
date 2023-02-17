// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moq;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests
{
    [TestFixture]
    public class TracingActivityTest
    {
        private readonly DateTime _startDateTime = new DateTime(1000, 1, 10, 10, 10, 10);
        private readonly DateTime _endDateTime = new DateTime(2000, 12, 20, 20, 20, 20);

        /// <summary>
        /// Helper method to create tracings
        /// </summary>
        private T CreateTracing<T>() where T : Tracing, new()
        {
            Tracing trace = new T();
            var tracing = trace as FooTracing;
            if (tracing != null)
            {
                tracing.FooName = "42";
                tracing.FooNumber = 42;
                tracing.Progress = FooProgress.Loaded;
            }
            trace.Started = _startDateTime;
            trace.Completed = _endDateTime;

            return trace as T;
        }

        [Test]
        public void TestActivityCreatedTracingTransformOtherType()
        {
            Activity activity = CreateActivity();
            activity.Tracing.Started = _startDateTime;
            activity.Tracing.Completed = _endDateTime;

            var wpcTracing = activity.TransformTracing<FooTracing>();

            Assert.AreEqual(_startDateTime, wpcTracing.Started);
            Assert.AreEqual(_endDateTime, wpcTracing.Completed);
            Assert.AreEqual(null, wpcTracing.FooName);
            Assert.AreEqual(0, wpcTracing.FooNumber);
            Assert.AreEqual(FooProgress.Initial, wpcTracing.Progress);
        }

        [Test]
        public void TestActivityCreatedTracingTransformSameType()
        {
            Activity activity = CreateActivity();
            activity.Tracing.Started = _startDateTime;
            activity.Tracing.Completed = _endDateTime;
            var defaultTracing = activity.TransformTracing<Tracing>();

            Assert.AreEqual(_startDateTime, defaultTracing.Started);
            Assert.AreEqual(_endDateTime, defaultTracing.Completed);
        }

        [Test]
        public void TestAddTraceInformation()
        {
            Activity activity = CreateActivity();
            activity.Tracing.Started = _startDateTime;
            activity.Tracing.Completed = _endDateTime;

            var wpcTracing = activity.TransformTracing<FooTracing>()
                .Trace(t => t.FooName = "42")
                .Trace(t => t.FooNumber = 42)
                .Trace(t => t.Progress = FooProgress.Done);

            Assert.AreEqual(_startDateTime, wpcTracing.Started);
            Assert.AreEqual(_endDateTime, wpcTracing.Completed);
            Assert.AreEqual("42", wpcTracing.FooName);
            Assert.AreEqual(42, wpcTracing.FooNumber);
            Assert.AreEqual((int)FooProgress.Done, ((Tracing)activity.Tracing).Progress);
        }

        [Test]
        public void TestNullTracingTransform()
        {
            var trace = CreateTracing<Tracing>();

            var defaultTracing = trace.Transform<Tracing>();

            Assert.AreEqual(trace.Started, defaultTracing.Started);
            Assert.AreEqual(trace.Completed, defaultTracing.Completed);
        }

        [Test]
        public void TestTransformWpcToNullAndBack()
        {
            var tracing = CreateTracing<FooTracing>().Trace(t => t.Text = "Hello Word");
            var defaultTracing = tracing.Transform<Tracing>();
            var newWpcTracing = defaultTracing.Transform<FooTracing>();

            Assert.AreEqual(tracing.Started, newWpcTracing.Started);
            Assert.AreEqual(tracing.Completed, newWpcTracing.Completed);
            Assert.AreEqual(tracing.Text, newWpcTracing.Text);
            Assert.AreEqual("42", newWpcTracing.FooName);
            Assert.AreEqual(42, newWpcTracing.FooNumber);
        }

        [Test]
        public void TestWpcTracingTransform()
        {
            var trace = CreateTracing<FooTracing>();

            var wpcTracing = trace.Transform<FooTracing>()
                .Trace(t => t.Processing());

            Assert.AreEqual(trace.Started, wpcTracing.Started);
            Assert.AreEqual(trace.Completed, wpcTracing.Completed);
            Assert.AreEqual(trace.FooName, wpcTracing.FooName);
            Assert.AreEqual(trace.FooNumber, wpcTracing.FooNumber);
            Assert.AreEqual(FooProgress.Running, wpcTracing.Progress);
        }

        private static Activity CreateActivity()
        {
            var activityMock = new Mock<Activity<NullActivityParameters>>();
            return activityMock.Object;
        }

        private enum FooProgress
        {
            Initial = 0,
            Loaded = 10,
            Running = 50,
            Done = 100
        }

        private class FooTracing : Tracing
        {
            public string FooName { get; set; }

            public int FooNumber { get; set; }
            
            public new FooProgress Progress
            {
                get { return (FooProgress) base.Progress; }
                set { base.Progress = (int) value; }
            }

            public void Processing()
            {
                Progress = FooProgress.Running;
            }
        }
    }
}
