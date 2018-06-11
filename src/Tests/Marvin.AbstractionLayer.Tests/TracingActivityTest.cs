using System;
using Moq;
using NUnit.Framework;

namespace Marvin.AbstractionLayer.Tests
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
            var defaultTracing = activity.TransformTracing<DefaultTracing>();

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
            var trace = CreateTracing<DefaultTracing>();

            var defaultTracing = trace.Transform<DefaultTracing>();

            Assert.AreEqual(trace.Started, defaultTracing.Started);
            Assert.AreEqual(trace.Completed, defaultTracing.Completed);
        }

        [Test]
        public void TestTransformWpcToNullAndBack()
        {
            var tracing = CreateTracing<FooTracing>();
            var defaultTracing = tracing.Transform<DefaultTracing>();
            var newWpcTracing = defaultTracing.Transform<FooTracing>();

            Assert.AreEqual(tracing.Started, newWpcTracing.Started);
            Assert.AreEqual(tracing.Completed, newWpcTracing.Completed);
            Assert.AreEqual(null, newWpcTracing.FooName);
            Assert.AreEqual(0, newWpcTracing.FooNumber);
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

        private class FooTracing : Tracing, IActivityProgress
        {
            public override string Type => nameof(FooTracing);

            public string FooName { get; set; }

            public int FooNumber { get; set; }

            public double Relative => base.Progress;

            public new FooProgress Progress
            {
                get { return (FooProgress) base.Progress; }
                set { base.Progress = (int) value; }
            }

            public void Processing()
            {
                Progress = FooProgress.Running;
            }

            protected override void Fill<T>(T instance)
            {
                base.Fill(instance);

                var wpc = instance as FooTracing;
                if (wpc != null)
                {
                    wpc.FooName = FooName;
                    wpc.FooNumber = FooNumber;
                }
            }
        }
    }
}