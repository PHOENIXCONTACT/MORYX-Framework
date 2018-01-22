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
            var tracing = trace as WpcTracing;
            if (tracing != null)
            {
                tracing.WpcIdentifier = "42";
                tracing.WpcPosition = 42;
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

            var wpcTracing = activity.TransformTracing<WpcTracing>();

            Assert.AreEqual(_startDateTime, wpcTracing.Started);
            Assert.AreEqual(_endDateTime, wpcTracing.Completed);
            Assert.AreEqual(null, wpcTracing.WpcIdentifier);
            Assert.AreEqual(0, wpcTracing.WpcPosition);
        }

        //activity.TransformTracing<WpcTracing>()
        //        .Trace(t => t.WpcIdentifier = CurrentWpc.ExternalId.ToString(CultureInfo.InvariantCulture))
        //        .Trace(t => t.WpcPosition = CurrentWpc.GetPosition(startActivity.Id));
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

            var wpcTracing = activity.TransformTracing<WpcTracing>()
                .Trace(t => t.WpcIdentifier = "42")
                .Trace(t => t.WpcPosition = 42);

            Assert.AreEqual(_startDateTime, wpcTracing.Started);
            Assert.AreEqual(_endDateTime, wpcTracing.Completed);
            Assert.AreEqual("42", wpcTracing.WpcIdentifier);
            Assert.AreEqual(42, wpcTracing.WpcPosition);
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
            var tracing = CreateTracing<WpcTracing>();
            var defaultTracing = tracing.Transform<DefaultTracing>();
            var newWpcTracing = defaultTracing.Transform<WpcTracing>();

            Assert.AreEqual(tracing.Started, newWpcTracing.Started);
            Assert.AreEqual(tracing.Completed, newWpcTracing.Completed);
            Assert.AreEqual(null, newWpcTracing.WpcIdentifier);
            Assert.AreEqual(0, newWpcTracing.WpcPosition);
        }

        [Test]
        public void TestWpcTracingTransform()
        {
            var trace = CreateTracing<WpcTracing>();

            var wpcTracing = trace.Transform<WpcTracing>();

            Assert.AreEqual(trace.Started, wpcTracing.Started);
            Assert.AreEqual(trace.Completed, wpcTracing.Completed);
            Assert.AreEqual(trace.WpcIdentifier, wpcTracing.WpcIdentifier);
            Assert.AreEqual(trace.WpcPosition, wpcTracing.WpcPosition);
        }

        private static Activity CreateActivity()
        {
            var activityMock = new Mock<Activity<NullActivityParameters>>();
            return activityMock.Object;
        }

        private class WpcTracing : Tracing
        {
            public override string Type => nameof(WpcTracing);

            public string WpcIdentifier { get; set; }

            public int WpcPosition { get; set; }
            

            protected override T Fill<T>(T instance)
            {
                var wpc = instance as WpcTracing;
                if (wpc != null)
                {
                    wpc.WpcIdentifier = WpcIdentifier;
                    wpc.WpcPosition = WpcPosition;
                }
                return instance;
            }
        }
    }
}