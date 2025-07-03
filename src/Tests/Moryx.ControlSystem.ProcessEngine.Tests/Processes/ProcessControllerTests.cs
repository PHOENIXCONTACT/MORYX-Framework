using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Jobs.Setup;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.TestTools;
using Moryx.Model;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Notifications;
using Moryx.TestTools.UnitTest;
using Moryx.Tools;
using NUnit.Framework;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ProcessControllerTests : ProcessTestsBase
    {
        private ProcessController _processController;
        private IUnitOfWorkFactory<ProcessContext> _unitOfWorkFactory;
        private NotSoParallelOps _notSoParallelOps;
        private ProcessEntity _processEntity;
        private ProcessData _processData;
        private IRecipeProvider _recipeProvider;
        private ProcessStorage _storage;
        internal Mock<INotificationAdapter> NotificationAdapterMock { get; private set; }

        /// <summary>
        /// Instance from <see cref="ProcessController.ProcessChanged"/> is written to this field
        /// </summary>
        internal ProcessData UpdatedProcess { get; private set; }

        [SetUp]
        public void GlobalSetup()
        {
            ReflectionTool.TestMode = true;

            // Prepare in memory ControlSystem db
            _unitOfWorkFactory = new UnitOfWorkFactory<ProcessContext>(new InMemoryDbContextManager(nameof(ProcessControllerTests)));

            _notSoParallelOps = new NotSoParallelOps();

            var recipeProviderMock = new Mock<IRecipeProvider>();
            recipeProviderMock.SetupGet(r => r.Name).Returns("RecipeProviderMock");
            _recipeProvider = recipeProviderMock.Object;
        }

        [SetUp]
        public void Create()
        {
            CreateList();
            NotificationAdapterMock = new Mock<INotificationAdapter>();

            _storage = new ProcessStorage { UnitOfWorkFactory = _unitOfWorkFactory };
            _storage.Start();
            _processController = new ProcessController
            {
                ActivityPool = DataPool,
                UnitOfWorkFactory = _unitOfWorkFactory,
                ProcessStorage = _storage
            };
            _processController.ProcessChanged += OnProcessChanged;
            _processController.Initialize();
            _processController.Start();
        }


        /// Updates the variable when the event occurred.
        private void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            UpdatedProcess = args.ProcessData;
        }

        [TearDown]
        public void DestroyController()
        {
            _processController.ProcessChanged -= OnProcessChanged;
            DestroyList();
            UpdatedProcess = null;
            _processData = null;
            _processEntity = null;
            _notSoParallelOps.WaitForScheduledExecution(1000);

            using (var uow = _unitOfWorkFactory.Create())
            {
                _processController.Stop();
                _processController.Dispose();

                var jobRepo = uow.GetRepository<IJobEntityRepository>();
                var processRepo = uow.GetRepository<IProcessEntityRepository>();

                var processes = processRepo.Linq.Active().ToArray();
                processRepo.RemoveRange(processes);

                var jobs = jobRepo.Linq.Active().ToArray();
                jobRepo.RemoveRange(jobs);

                uow.SaveChanges();
            }
        }

        [TestCase(3, 1, 1, 1, Description = "Create three jobs and take one which is assigned to the process.")]
        [TestCase(3, 1, 2, 0, Description = "Create three jobs and take one which is not assigned to the process.")]
        [TestCase(3, 5, 1, 0, Description = "Create three jobs and take one which is not in the list. FirstOrDefault Test.")]
        public void LoadTest(int amount, int jobNr, int jobForProcess, int runningProcesses)
        {
            // Arrange
            var jobs = new IJobData[amount];
            for (int i = 0; i < amount; i++)
            {
                jobs[i] = CreateJobDataAndSave(0);
            }
            CreateProcess(jobs[jobForProcess].Id);

            // Act
            var processes = jobNr <= amount ? _processController.LoadProcesses(jobs[jobNr]) : new Collection<ProcessData>();


            // Assert
            Assert.That(processes.Count, Is.EqualTo(runningProcesses), "The wanted process was not added.");
        }
        
        [TestCase(RecipeType.Setup, Description = "Recipe Type Setup.")]
        [TestCase(RecipeType.Production, Description = "Recipe Type Production")]
        public void StartTest(RecipeType recipeType)
        {
            // Arrange
            var job = CreateJobDataAndSave(recipeType);
            var process = job.Recipe.CreateProcess();
            var processData = new ProcessData(process) {Job = job};

            // Act
            _processController.Start(processData);

            // Assert
            Assert.That(ModifiedProcess, Is.Not.Null, "The wanted process was not added.");
            Assert.That(ModifiedProcess.Job.Id, Is.EqualTo(job.Id), "The added job id is not the wanted.");
            Assert.That(ModifiedProcess.Process.Recipe.Id, Is.EqualTo(job.Recipe.Id), "The added recipe id is not the wanted.");
            Assert.That(ModifiedProcess.Process.Recipe.Revision, Is.EqualTo(job.Recipe.Revision), "The added recipe revision was not the wanted.");
            Assert.That(ModifiedProcess.State, Is.EqualTo(ProcessState.Ready), "The added process state was not the wanted.");
        }

        [Test]
        public void ResumeTest()
        {
            // Arrange
            var job = CreateJobDataAndSave(0);
            CreateProcess(job.Id);

            // Act
            var processes = _processController.LoadProcesses(job);
            job.AddProcesses(processes);
            _processController.Resume(processes);

            // Assert
            Assert.That(processes.Count, Is.EqualTo(1), "The wanted process was not added to the running process list.");
            var process = processes.First();
            Assert.That(process.State, Is.EqualTo(ProcessState.Restored));
        }

        [Test]
        public void AbortTest()
        {
            // Arrange
            var job = CreateJobDataAndSave(0);
            CreateProcess(job.Id);
            job.AddProcess(_processData);

            // Act
            _processController.Interrupt(job.RunningProcesses.Cast<ProcessData>(), true);

            // Assert
            Assert.That(job.RunningProcesses.Count, Is.EqualTo(1), "The wanted process was not added to the running process list.");
        }

        [Test]
        public void CompleteTest()
        {
            // Arrange
            var job = CreateJobDataAndSave(0);
            CreateProcess(job.Id);
            job.AddProcess(_processData);

            // Act
            _processController.Interrupt(job.RunningProcesses.Cast<ProcessData>(), false);

            // Assert
            Assert.That(job.RunningProcesses.Count, Is.EqualTo(1), "The wanted process was not added to the running process list.");
        }

        [Test]
        public void Cleanup()
        {
            // Arrange
            var job = CreateJobDataAndSave(0);
            CreateProcess(job.Id);
            job.AddProcess(_processData);

            _processData.State = ProcessState.Initial;

            // Act
            _processController.Cleanup(job.RunningProcesses);

            // Assert
            Assert.That(_processData.State, Is.EqualTo(ProcessState.CleaningUp), "The process has not changed its state to CleaningUp");
        }

        [Test]
        public void ProcessChangeEventTest()
        {
            // Arrange
            var job = CreateJobDataAndSave(0);
            CreateProcess(job.Id);

            // Act
            _processController.ActivityPool.UpdateProcess(_processData, ProcessState.Running);

            // Assert
            Assert.That(UpdatedProcess, Is.Not.Null, "The wanted process was not updated.");
            Assert.That(UpdatedProcess.Id, Is.EqualTo(_processData.Id), "The updated process was not the wanted one.");
        }

        private IJobData CreateJobDataAndSave(RecipeType recipeType)
        {
            var recipe = recipeType == RecipeType.Production
                ? (IWorkplanRecipe)DummyRecipe.BuildRecipe()
                : new SetupRecipe { Workplan = DummyRecipe.BuildWorkplan() };

            recipe.Origin = _recipeProvider;

            var job = recipeType == RecipeType.Production 
                ? (IJobData)new ProductionJobData((IProductionRecipe)recipe, 10) 
                : new SetupJobData(recipe) { NotificationAdapter = NotificationAdapterMock.Object };

            job.ProgressChanged += delegate { };

            //Save it so that relation could be found.
            var jobStorage = new JobStorage
            {
                UnitOfWorkFactory = _unitOfWorkFactory,
                ParallelOperations = _notSoParallelOps
            };

            jobStorage.Save(new ModifiedJobsFragment(new []{job}, null));

            return job;
        }

        private void CreateProcess(long jobId)
        {
            _processData = new ProcessData(new Process())
            {
                State = ProcessState.Ready,
            };

            using (var uow = _unitOfWorkFactory.Create())
            {
                _processEntity = uow.DbContext.ProcessEntities.Add(new()
                {
                    Id = IdShiftGenerator.Generate(jobId, NextId),
                    TypeName = "testProcess",
                    State = (int)_processData.State,
                    JobId = jobId,
                }).Entity;

                uow.DbContext.ActivityEntities.Add(new()
                {
                    Id = IdShiftGenerator.Generate(_processEntity.Id << 14, NextId),
                    TaskId = 1,
                    ResourceId = 1,
                    ProcessId = 1,
                });

                // Save
                uow.SaveChanges();

                _processData.Process.Id = _processEntity.Id;
            }
        }

        public enum RecipeType
        {
            Production = 0,
            Setup = 1
        }
    }
}