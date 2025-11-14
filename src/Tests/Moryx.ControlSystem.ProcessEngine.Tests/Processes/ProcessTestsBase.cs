// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.Workplans;
using Moryx.ControlSystem.Capabilities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.ControlSystem.TestTools.Tasks;
using Moryx.Logging;
using Moryx.Model.Repositories;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    /// <summary>
    /// Instead of mocking the activity pool we simply create a test base that provides
    /// all tests with an instance of the activity pool
    /// </summary>
    public class ProcessTestsBase
    {
        /// <summary>
        /// Object under test
        /// </summary>
        internal IActivityDataPool DataPool { get; private set; }

        /// <summary>
        /// Instance from <see cref="IActivityDataPool.ProcessChanged"/> is written to this field
        /// </summary>
        internal ProcessData ModifiedProcess { get; set; }

        /// <summary>
        /// Instance from <see cref="IActivityDataPool.ActivityChanged"/> is written to this field
        /// </summary>
        internal ActivityData ModifiedActivity { get; set; }

        protected const long ProductionCellId = 1;
        protected const long MountCellId = 2;
        protected const long SerialCellId = 3;
        protected const long NewCellId = 4;

        protected const long ValidProcessId = 1;
        protected const long InvalidProcessId = 2;

        private static int IdSequence;

        public static int NextId => Interlocked.Increment(ref IdSequence);

        protected void CreateList()
        {
            var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
            DataPool = new ActivityPool { Logger = logger };
            DataPool.ProcessChanged += OnProcessChanged;
            DataPool.ActivityChanged += OnActivityChanged;
        }

        private void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            ModifiedProcess = args.ProcessData;
        }
        private void OnActivityChanged(object sender, ActivityEventArgs args)
        {
            ModifiedActivity = args.ActivityData;
        }

        protected void DestroyList()
        {
            ModifiedProcess = null;
            ModifiedActivity = null;

            DataPool.ProcessChanged -= OnProcessChanged;
            DataPool.ActivityChanged -= OnActivityChanged;

            DataPool = null;
        }

        protected Mock<ICell> CreateSerialCell(Mock<IResourceManagement> resourceManagementMock)
        {
            var serialCellMock = new Mock<ICell>();

            serialCellMock.SetupGet(r => r.Id).Returns(() => SerialCellId);
            serialCellMock.SetupGet(r => r.Capabilities).Returns(() => new AssignIdentityCapabilities(IdentitySource.Pool));

            resourceManagementMock.Setup(rm => rm.GetResources<ICell>(It.IsAny<AssignIdentityCapabilities>()))
                .Returns(() => [serialCellMock.Object]);

            resourceManagementMock.Setup(rm => rm.GetResource<ICell>(SerialCellId)).Returns(serialCellMock.Object);

            return serialCellMock;
        }

        protected Mock<ICell> CreateMountCell(Mock<IResourceManagement> resourceManagementMock, bool canMount, bool canUnmount)
        {
            var mountCellMock = new Mock<ICell>();
            mountCellMock.SetupGet(r => r.Id).Returns(MountCellId);
            mountCellMock.SetupGet(r => r.Capabilities).Returns(new MountCapabilities(canMount, canUnmount));

            resourceManagementMock.Setup(rm => rm.GetResources<ICell>(It.Is<MountCapabilities>(c => c.CanMount == canMount && c.CanUnmount == canUnmount)))
                .Returns(() => [mountCellMock.Object]);

            resourceManagementMock.Setup(rm => rm.GetResource<ICell>(MountCellId)).Returns(mountCellMock.Object);

            return mountCellMock;
        }

        protected Mock<ICell> CreateProductionCell(Mock<IResourceManagement> resourceManagementMock)
        {
            var productionCellMock = new Mock<ICell>();
            productionCellMock.SetupGet(r => r.Id).Returns(ProductionCellId);
            productionCellMock.SetupGet(r => r.Capabilities).Returns(NullCapabilities.Instance);

            resourceManagementMock.Setup(rm => rm.GetResource<ICell>(ProductionCellId)).Returns(productionCellMock.Object);

            return productionCellMock;
        }

        protected void RaiseReadyToWork(Mock<ICell> resource, ReadyToWork readyToWork)
        {
            resource.Raise(r => r.ReadyToWork += null, resource.Object, readyToWork);
        }

        protected void RaiseNotReadyToWork(Mock<ICell> resource, NotReadyToWork notReadyToWork)
        {
            resource.Raise(r => r.NotReadyToWork += null, resource.Object, notReadyToWork);
        }

        protected void RaiseActivityCompleted(Mock<ICell> resource, ActivityCompleted activityCompleted)
        {
            resource.Raise(r => r.ActivityCompleted += null, resource.Object, activityCompleted);
        }

        internal ActivityData FillPool(Activity activity, ICell cell)
        {
            var process = new ProductionProcess { Id = ValidProcessId };
            activity.Process = process;
            process.AddActivity(activity);

            var processData = new ProcessData(process);
            var activityData = new ActivityData(activity);

            DataPool.AddProcess(processData);
            DataPool.AddActivity(processData, activityData);

            activityData.Targets = [cell];

            DataPool.UpdateProcess(processData, ProcessState.EngineStarted);

            return activityData;
        }

        /// <summary>
        /// Create a certain process in the database
        /// </summary>
        internal void CreateProcessInDb(ProcessStorage storage, IUnitOfWork uow, ProcessData processData, string tokenJson)
        {
            var taskId = DummyRecipe.BuildRecipe().Workplan.Steps.First(s => s is MountTask).Id;

            var tokenHolderRepo = uow.GetRepository<ITokenHolderEntityRepository>();
            processData.Id = IdShiftGenerator.Generate(42, Interlocked.Increment(ref IdSequence));
            var activityData = new ActivityData(new MountActivity())
            {
                Id = IdShiftGenerator.Generate(processData.Id, Interlocked.Increment(ref IdSequence)),
                Resource = new CellReference(42),
                Task = new TaskTransition<MountActivity>(null, null) { Id = taskId },
                ProcessData = processData,
                Result = new ActivityResult { Numeric = 0, Success = true },
                State = ActivityState.Completed,
            };
            processData.AddActivity(activityData);
            activityData = new ActivityData(new MountActivity())
            {
                Id = IdShiftGenerator.Generate(processData.Id, Interlocked.Increment(ref IdSequence)),
                Resource = new CellReference(42),
                Task = new TaskTransition<MountActivity>(null, null) { Id = taskId },
                ProcessData = processData,
                State = ActivityState.Configured,
            };
            processData.AddActivity(activityData);

            storage.SaveProcess(processData);

            var tokenHolder = tokenHolderRepo.Create(taskId, tokenJson);
            tokenHolder.ProcessId = processData.Id;

            uow.SaveChanges();
        }
    }
}
