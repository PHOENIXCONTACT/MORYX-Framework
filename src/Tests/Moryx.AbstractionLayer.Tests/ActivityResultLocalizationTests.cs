using Moryx.AbstractionLayer.Tests.TestData;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.AbstractionLayer.Tests
{
    [TestFixture]
    public class ActivityResultLocalizationTests
    {

        [Test]
        public void ActivityResultDisplayName()
        {
            //Arrange
            TestTask testTask = new TestTask();
            string expectedFailedDisplayName = "This is a failed result";
            string expectedSuccesDisplayName = TestResults.Success.ToString();

            //Assign
            var outputDescriptions = testTask.OutputDescriptions;
            var failedResultDescription = outputDescriptions
                .FirstOrDefault(x => x.MappingValue == (long)TestResults.Failed);
            var successResultDescription = outputDescriptions
                .FirstOrDefault(x => x.MappingValue == (long)TestResults.Success);

            //assert
            Assert.That(failedResultDescription.Name, Is.EqualTo(expectedFailedDisplayName));
            Assert.That(successResultDescription.Name, Is.EqualTo(expectedSuccesDisplayName));
        }
    }
}
