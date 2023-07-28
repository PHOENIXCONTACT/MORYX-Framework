using Moryx.Workplans;
using Moryx.AbstractionLayer;
using NUnit.Framework;
using Moryx.Tests.Workplans.Dummies;

namespace Moryx.Tests.Workplans
{

    [TestFixture]
    public class EqualTest
    {
        private Workplan firstWorkplan;
        private Workplan secondWorkplan;
        private Workplan thirdWorkplan;
        private Workplan fourthWorkplan;
        private Workplan fifthWorkplan;
        private Workplan sixthWorkplan;
        
        public Workplan CreateFirstWorkplan()
        {
            var plan = new Workplan();
            
            var start = plan.AddConnector("Start", NodeClassification.Start);
            var end = plan.AddConnector("End", NodeClassification.End);
            var failed = plan.AddConnector("Failed", NodeClassification.Failed);

            var input = start;
            var outputA = plan.AddConnector("A");
            var outputB = plan.AddConnector("B");

            plan.AddStep(new AssemblingTask(), new AssemblingParameters(), input, outputA, outputB, failed);

            input = outputA;
            plan.AddStep(new PackagingTask(), new AssemblingParameters(), input, end, end, failed);

            input = outputB;
            plan.AddStep(new ColorizingTask(), new AssemblingParameters(), input, end, failed, failed);
            return plan;
        }

        public Workplan CreateSecondWorkplan()
        {
            var plan = new Workplan();

            var start = plan.AddConnector("Start", NodeClassification.Start);
            var end = plan.AddConnector("End", NodeClassification.End);
            var failed = plan.AddConnector("Failed", NodeClassification.Failed);

            var input = start;
            var outptuA = input;
            var outputB = plan.AddConnector("A");

            plan.AddStep(new AssemblingTask(), new AssemblingParameters(), input, outptuA, outputB, failed);

            input = outputB;
            plan.AddStep(new PackagingTask(), new AssemblingParameters(), input, outputB, end, failed);

            return plan;
        }

        public Workplan CreateThirdWorkplan()
        {
            var plan = new Workplan();

            var start = plan.AddConnector("Start", NodeClassification.Start);
            var end = plan.AddConnector("End", NodeClassification.End);
            var failed = plan.AddConnector("Failed", NodeClassification.Failed);

            var outputA = plan.AddConnector("A");
            var outputB = plan.AddConnector("B");

            plan.AddStep(new AssemblingTask(), new AssemblingParameters(), start, outputA, outputB, failed);

            var input = outputA;

            plan.AddStep(new PackagingTask(), new AssemblingParameters(), input, start, end, failed);

            input = outputB;

            plan.AddStep(new ColorizingTask(), new AssemblingParameters(), input, outputA, end, failed);

            return plan; 
        }

        [SetUp]
        public void SetUp()
        {
            firstWorkplan = CreateFirstWorkplan();
            secondWorkplan = CreateFirstWorkplan();

            thirdWorkplan = CreateSecondWorkplan();
            fourthWorkplan = CreateSecondWorkplan();

            fifthWorkplan = CreateThirdWorkplan();
            sixthWorkplan = CreateThirdWorkplan();
        }

        [Test]
        public void TestEqualWorkplans()
        {
            bool result = firstWorkplan.Equals(secondWorkplan);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestEqualWorkplansSimpleLoop()
        {
            bool result = thirdWorkplan.Equals(fourthWorkplan);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestEqualWorkplansDoubleLoop()
        {
            bool result = fifthWorkplan.Equals(sixthWorkplan);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestUnequalWorkplans()
        {
            bool r = thirdWorkplan.Equals(fifthWorkplan);
            Assert.That(r, Is.False);
        }
    }

}
