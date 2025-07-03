using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.Logging;
using Moryx.Model.Repositories;
using NUnit.Framework;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ProcessInterruptionTests : ProcessTestsBase
    {
        private const int SampleSize = 100;
        private const int TimeoutSec = 1;

        private readonly IList<ActivityEntity> _activities = new List<ActivityEntity>();
        private ProcessInterruption _processInterruption;

        [OneTimeSetUp]
        public void CreateActivities()
        {
            for (int id = 1; id <= SampleSize; id++)
            {
                var startDate = DateTime.Now.AddMinutes(-1 * id);
                var endDate = startDate.AddSeconds(1)
                    .AddMilliseconds(((id % 2 == 0 ? 100 : -100) * Math.Log(id)) % 200);

                _activities.Add(new ActivityEntity
                {
                    Id = id,
                    Started = startDate,
                    Completed = endDate,
                    Result = 0
                });
            }
        }

        [SetUp]
        public void CreateInterruption()
        {
            CreateList();

            _processInterruption = new ProcessInterruption
            {
                ActivityPool = DataPool,
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                Config = new ModuleConfig
                {
                    TimeoutCalculation = TimeoutCalculationType.Manual,
                    SampleSize = SampleSize,
                    ManualShutdownTimeoutSec = TimeoutSec
                }
            };
            _processInterruption.Initialize();
            _processInterruption.Start();
        }

        [TearDown]
        public void DestoryInterruption()
        {
            _processInterruption.Dispose();
            DestroyList();
        }

        [TestCase(TimeoutCalculationType.Manual, 1000, true, Description = "In manual mode the dispatcher should wait one second.")]
        [TestCase(TimeoutCalculationType.OneSigma, 1100, true, Description = "One sigma is 107ms. The dispatcher should wait at least 1100ms.")]
        [TestCase(TimeoutCalculationType.TwoSigma, 1200, true, Description = "Two sigma is 107ms. The dispatcher should wait at least 1210ms.")]
        [TestCase(TimeoutCalculationType.ThreeSigma, 1300, true, Description = "Three sigma is 107ms. The dispatcher should wait at least 1315ms.")]
        [TestCase(TimeoutCalculationType.Manual, 1000, false, Description = "In manual mode the dispatcher should wait one second.")]
        [TestCase(TimeoutCalculationType.OneSigma, 1000, false, Description = "One sigma is 107ms. The dispatcher should wait at least 1100ms.")]
        [TestCase(TimeoutCalculationType.TwoSigma, 1000, false, Description = "Two sigma is 107ms. The dispatcher should wait at least 1210ms.")]
        [TestCase(TimeoutCalculationType.ThreeSigma, 1000, false, Description = "Three sigma is 107ms. The dispatcher should wait at least 1315ms.")]
        public void CalculateTimeout(TimeoutCalculationType type, int expectedWaitTime, bool poolHasActivities)
        {
            // Arrange
            _processInterruption.Config.TimeoutCalculation = type;
            var activity = FillPool(new DummyActivity());
            DataPool.UpdateActivity(activity, ActivityState.Configured); // Add to open activities
            DataPool.UpdateActivity(activity, ActivityState.Running); // Set to running

            var activities = poolHasActivities ? _activities : Array.Empty<ActivityEntity>();

            var activityRepoMock = new Mock<IActivityEntityRepository>();
            activityRepoMock.SetupGet(a => a.Linq).Returns(activities.AsQueryable);

            var uowMock = new Mock<IUnitOfWork<ProcessContext>>();
            uowMock.Setup(uow => uow.GetRepository<IActivityEntityRepository>()).Returns(activityRepoMock.Object);

            var uowFactoryMock = new Mock<IUnitOfWorkFactory<ProcessContext>>();
            uowFactoryMock.Setup(f => f.Create()).Returns(uowMock.Object);

            // Act
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _processInterruption.UowFactory = uowFactoryMock.Object;
            _processInterruption.Stop();
            stopWatch.Stop();

            // Assert
            Assert.That(stopWatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(expectedWaitTime));
            Assert.That(stopWatch.ElapsedMilliseconds, Is.LessThanOrEqualTo(expectedWaitTime + 200));
        }

        private ActivityData FillPool(AbstractionLayer.Activity activity)
        {
            var process = new ProductionProcess { Id = 1 };
            activity.Process = process;
            process.AddActivity(activity);

            var processData = new ProcessData(process);
            var activityData = new ActivityData(activity);

            DataPool.AddProcess(processData);
            DataPool.AddActivity(processData, activityData);

            return activityData;
        }
    }
}