// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moq;
using Moryx.AbstractionLayer.Activities;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests
{
    [TestFixture]
    public class TracingActivityTest
    {
        private readonly DateTime _startDateTime = new(1000, 1, 10, 10, 10, 10);
        private readonly DateTime _endDateTime = new(2000, 12, 20, 20, 20, 20);

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

            Assert.That(wpcTracing.Started, Is.EqualTo(_startDateTime));
            Assert.That(wpcTracing.Completed, Is.EqualTo(_endDateTime));
            Assert.That(wpcTracing.FooName, Is.EqualTo(null));
            Assert.That(wpcTracing.FooNumber, Is.EqualTo(0));
            Assert.That(wpcTracing.Progress, Is.EqualTo(FooProgress.Initial));
        }

        [Test]
        public void TestActivityCreatedTracingTransformSameType()
        {
            Activity activity = CreateActivity();
            activity.Tracing.Started = _startDateTime;
            activity.Tracing.Completed = _endDateTime;
            var defaultTracing = activity.TransformTracing<Tracing>();

            Assert.That(defaultTracing.Started, Is.EqualTo(_startDateTime));
            Assert.That(defaultTracing.Completed, Is.EqualTo(_endDateTime));
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

            Assert.That(wpcTracing.Started, Is.EqualTo(_startDateTime));
            Assert.That(wpcTracing.Completed, Is.EqualTo(_endDateTime));
            Assert.That(wpcTracing.FooName, Is.EqualTo("42"));
            Assert.That(wpcTracing.FooNumber, Is.EqualTo(42));
            Assert.That(activity.Tracing.Progress, Is.EqualTo((int)FooProgress.Done));
        }

        [Test]
        public void TestNullTracingTransform()
        {
            var trace = CreateTracing<Tracing>();

            var defaultTracing = trace.Transform<Tracing>();

            Assert.That(defaultTracing.Started, Is.EqualTo(trace.Started));
            Assert.That(defaultTracing.Completed, Is.EqualTo(trace.Completed));
        }

        [Test]
        public void TestTransformWpcToNullAndBack()
        {
            var tracing = CreateTracing<FooTracing>().Trace(t => t.Text = "Hello Word");
            var defaultTracing = tracing.Transform<Tracing>();
            var newWpcTracing = defaultTracing.Transform<FooTracing>();

            Assert.That(newWpcTracing.Started, Is.EqualTo(tracing.Started));
            Assert.That(newWpcTracing.Completed, Is.EqualTo(tracing.Completed));
            Assert.That(newWpcTracing.Text, Is.EqualTo(tracing.Text));
            Assert.That(newWpcTracing.FooName, Is.EqualTo("42"));
            Assert.That(newWpcTracing.FooNumber, Is.EqualTo(42));
        }

        [Test]
        public void TestWpcTracingTransform()
        {
            var trace = CreateTracing<FooTracing>();

            var wpcTracing = trace.Transform<FooTracing>()
                .Trace(t => t.Processing());

            Assert.That(wpcTracing.Started, Is.EqualTo(trace.Started));
            Assert.That(wpcTracing.Completed, Is.EqualTo(trace.Completed));
            Assert.That(wpcTracing.FooName, Is.EqualTo(trace.FooName));
            Assert.That(wpcTracing.FooNumber, Is.EqualTo(trace.FooNumber));
            Assert.That(wpcTracing.Progress, Is.EqualTo(FooProgress.Running));
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
                get { return (FooProgress)base.Progress; }
                set { base.Progress = (int)value; }
            }

            public void Processing()
            {
                Progress = FooProgress.Running;
            }
        }
    }
}
