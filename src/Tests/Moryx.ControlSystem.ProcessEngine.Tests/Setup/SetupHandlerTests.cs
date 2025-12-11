// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Setup;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.ControlSystem.TestTools.Tasks;
using Moryx.Logging;
using Moryx.Workplans;
using NUnit.Framework;
using SetupClassification = Moryx.ControlSystem.Setups.SetupClassification;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Setup
{
    /// <summary>
    /// Tests for the setup-management
    /// </summary>
    [TestFixture]
    public class SetupManagerTests
    {
        private Mock<IJobDataList> _jobListMock;
        private Mock<IResourceManagement> _resourceManagerMock;
        private Mock<IJobDataFactory> _jobDataFactoryMock;
        private Mock<ISetupProvider> _providerMock;
        private readonly List<Mock<ISetupJobData>> _setupJobs = new();

        private readonly List<IJobData> _jobList = new();

        /// <summary>
        /// Initialize the test-environment before every test
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _jobListMock = new Mock<IJobDataList>();
            _jobListMock
                .Setup(j => j.Previous(It.IsAny<IJobData>()))
                .Returns<IJobData>(start =>
                {
                    var index = _jobList.IndexOf(start);
                    return index >= 1 ? _jobList[index - 1] : null;
                });
            _jobListMock
                .Setup(j => j.Next(It.IsAny<IJobData>()))
                .Returns<IJobData>(start =>
                {
                    var index = _jobList.IndexOf(start);
                    return index < _jobList.Count - 1 ? _jobList[index + 1] : null;
                });
            _jobListMock
                .Setup(j => j.Forward(It.IsAny<IJobData>()))
                .Returns<IJobData>(start =>
                {
                    var index = _jobList.IndexOf(start);
                    return _jobList.Skip(index + 1);
                });
            _jobListMock
                .Setup(j => j.Backward())
                .Returns(() =>
                {
                    var copy = _jobList.ToList();
                    copy.Reverse();
                    return copy;
                });
            _jobListMock
                .Setup(j => j.Backward(It.IsAny<IJobData>()))
                .Returns<IJobData>(start =>
                {
                    var copy = _jobList.ToList();
                    copy.Reverse();
                    var index = copy.IndexOf(start);
                    return copy.Skip(index + 1);
                });
            _jobListMock
                .Setup(jl => jl.GetEnumerator()).Returns(() => _jobList.GetEnumerator());
            _jobListMock.As<IEnumerable>()
                .Setup(jl => jl.GetEnumerator()).Returns(() => _jobList.GetEnumerator());

            _jobDataFactoryMock = new Mock<IJobDataFactory>();
            _jobDataFactoryMock.Setup(c => c.Create<ISetupJobData>(It.IsAny<IWorkplanRecipe>(), It.IsAny<int>()))
                .Returns(
                    (IWorkplanRecipe recipe, int amount) =>
                    {
                        var setupJobDataMock = new Mock<ISetupJobData>();
                        setupJobDataMock.SetupGet(j => j.Amount).Returns(amount);
                        setupJobDataMock.SetupGet(j => j.Recipe).Returns((SetupRecipe)recipe);
                        setupJobDataMock.SetupGet(j => j.Classification).Returns(JobClassification.Waiting);
                        setupJobDataMock.Setup(j => j.UpdateSetup(It.IsAny<SetupRecipe>())).Callback(
                            (SetupRecipe newRecipe) => { setupJobDataMock.SetupGet(j => j.Recipe).Returns(newRecipe); });

                        var jobDataMock = setupJobDataMock.As<IJobData>();
                        jobDataMock.SetupGet(j => j.Recipe).Returns(() => setupJobDataMock.Object.Recipe);
                        jobDataMock.SetupGet(j => j.Job).Returns(() => new EngineJob(setupJobDataMock.Object.Recipe, amount));

                        _setupJobs.Add(setupJobDataMock);
                        return setupJobDataMock.Object;
                    });

            _providerMock = new Mock<ISetupProvider>();
            _providerMock.Setup(p => p.RequiredSetup(It.IsAny<SetupExecution>(), It.IsAny<ProductionRecipe>(), It.IsAny<ISetupTarget>()))
                .Returns<SetupExecution, ProductionRecipe, ISetupTarget>(ProvideSetup);

            _resourceManagerMock = new Mock<IResourceManagement>();
            _resourceManagerMock
                .Setup(rmm => rmm.GetResources<ICell>(It.IsAny<ICapabilities>()))
                .Returns([]);
        }

        private SetupRecipe ProvideSetup(SetupExecution execution, ProductionRecipe recipe, ISetupTarget target)
        {
            var testRecipe = (TestRecipe)recipe;
            if (execution == SetupExecution.BeforeProduction && target.Cells(new TestSetupCapabilities { SetupState = testRecipe.SetupState }).Count == 0)
            {
                var workplan = new Workplan();
                workplan.Add(new MountTask());
                return new SetupRecipe
                {
                    Name = "Before " + recipe.Name,
                    Execution = execution,
                    TargetRecipe = recipe,
                    Workplan = workplan
                };
            }
            if (execution == SetupExecution.AfterProduction && target.Cells(new TestSetupCapabilities { SetupState = testRecipe.SetupState }).Any())
            {
                var workplan = new Workplan();
                return new SetupRecipe
                {
                    Name = "After " + recipe.Name,
                    Execution = execution,
                    TargetRecipe = recipe,
                    Workplan = workplan
                };
            }

            return null;
        }

        [TearDown]
        public void ClearJobList()
        {
            _jobList.Clear();
            _setupJobs.Clear();
        }

        #region Tests

        [Test(Description = "There should one SetupJob of type prepare before the only production job. " +
                            "Job should have a dependency on the SetupJob. Clean-up is only added as a placeholder")]
        public void AddOnePrepareSetupJobBeforeTheOnlyProductionJob()
        {
            // Arrange
            var manager = CreateSetupManager();
            AdjustCurrentResourceCapabilities(0);
            var recipe = CreateRecipe(1783, 42);
            var productionJob = CreateProductionJob("Lucky Luke", recipe).Object;
            _jobList.Add(productionJob);
            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { productionJob });

            // Act
            manager.Handle(schedulableLinkedList);

            // Assert
            Assert.That((schedulableLinkedList.First().Recipe as SetupRecipe)?.SetupClassification, Is.EqualTo(SetupClassification.Unspecified),
                "The dependency is not SetupType.Prepare.");
            Assert.That(schedulableLinkedList.Count, Is.EqualTo(3), "The job list contains more or less than 2 jobs.");
        }

        [Test(Description = "If the setup provider throws an exception when evaluating or providing the required steps " +
                            "in a setup, the job should be interrupted and reoved from the linked list.")]
        public void InterruptProductionJobOnSetupProviderException()
        {
            // Arrange
            _providerMock.Setup(p => p.RequiredSetup(It.IsAny<SetupExecution>(), It.IsAny<ProductionRecipe>(), It.IsAny<ISetupTarget>()))
                .Throws(() => new Exception());
            var manager = CreateSetupManager();
            AdjustCurrentResourceCapabilities(0);
            var recipe = CreateRecipe(1783, 42);
            var productionJobMock = CreateProductionJob("Lucky Luke", recipe);
            var productionJob = productionJobMock.Object;
            _jobList.Add(productionJob);
            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { productionJob });

            // Act
            manager.Handle(schedulableLinkedList);

            // Assert
            Assert.That(schedulableLinkedList.Count, Is.EqualTo(0), "The job for which no setup could be created should be removed");
            productionJobMock.Verify(j => j.Interrupt(), Times.Once(), "The job for which no setup could be created should be interrupted");
        }

        [Test(Description = "There should be only one SetupJob of type cleanup after the only production job. " +
                            "The cleanup Job must have a dependency on the production job.")]
        public void AddOneCleanupJobBehindTheOnlyProductionJob()
        {
            // Arrange
            var manager = CreateSetupManager();
            AdjustCurrentResourceCapabilities(42);
            manager.Start();

            var recipe = CreateRecipe(1783, 42);
            var productionJobData = CreateProductionJob("Lucky Luke", recipe).Object;
            _jobList.Add(productionJobData);
            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { productionJobData });

            // Act
            manager.Handle(schedulableLinkedList);

            // Assert
            var setupRecipe = (SetupRecipe)schedulableLinkedList.Last().Recipe;
            Assert.That(setupRecipe.Execution, Is.EqualTo(SetupExecution.AfterProduction), "The dependency is not SetupType.Unspecified.");
            // After requesting a new recipe, it becomes complete
            _setupJobs[0].SetupGet(j => j.RecipeRequired).Returns(true);
            AdjustCurrentResourceCapabilities(42);
            var state = new Mock<IJobState>();
            state.SetupGet(s => s.Classification).Returns(JobClassification.Running);
            _jobListMock.Raise(jl => jl.StateChanged += null, _jobListMock.Object, new JobStateEventArgs(_setupJobs[0].Object, null, state.Object));
            Assert.That(setupRecipe.Execution, Is.EqualTo(SetupExecution.AfterProduction), "The dependency is not SetupType.PostExecution.");

            Assert.That(schedulableLinkedList.Count, Is.EqualTo(2), "The job list contains more or less than 2 jobs.");
            var nodeJoe = schedulableLinkedList.Find(productionJobData);
            Assert.That(nodeJoe, Is.Not.Null, "Lucky Luke was not present in the list. Should not be possible since we worked already with Lucky Luke.");
            Assert.That(nodeJoe.Previous, Is.Null, "Lucky Luke is not on first place.");
            Assert.That(nodeJoe.Next, Is.Not.Null, "When this is null, then Lucky Luke has no dude behind him.");
            Assert.That(_setupJobs.First().Object, Is.EqualTo(nodeJoe.Next.Value), "The created cleanup job is not the one after Lucky Luke.");
        }

        [Test(Description = "If the setup provider throws an exception when evaluating or providing the required steps " +
                            "in a clean up, the job should be interrupted and reoved from the linked list.")]
        public void InterruptProductionJobOnCleanUpProviderException()
        {
            // Arrange
            _providerMock.Setup(p => p.RequiredSetup(It.IsAny<SetupExecution>(), It.IsAny<ProductionRecipe>(), It.IsAny<ISetupTarget>()))
                .Throws(() => new Exception());
            var manager = CreateSetupManager();
            AdjustCurrentResourceCapabilities(42);
            manager.Start();

            var recipe = CreateRecipe(1783, 42);
            var productionJobMock = CreateProductionJob("Lucky Luke", recipe);
            var productionJobData = productionJobMock.Object;
            _jobList.Add(productionJobData);
            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { productionJobData });

            // Act
            manager.Handle(schedulableLinkedList);

            // Assert
            Assert.That(schedulableLinkedList.Count, Is.EqualTo(0), "The job for which no clean up could be created should be removed");
            productionJobMock.Verify(j => j.Interrupt(), Times.Once(), "The job for which no clean up could be created should be interrupted");
        }

        [Test(Description = "There should be a prepare and a cleanup job before and after each job in the right order.")]
        public void AddAPrepareAndCleanUpJobBeforeAndAfterEachProductionJob()
        {
            // Arrange
            var manager = CreateSetupManager();

            var recipeJoe = CreateRecipe(1783, 100);
            var productionJoeJobData = CreateProductionJob("Joe Dalton", recipeJoe).Object;
            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { productionJoeJobData });
            _jobList.Add(productionJoeJobData);
            var recipeAverell = CreateRecipe(1784, 100);
            var productionAverellJobData = CreateProductionJob("Averell Dalton", recipeAverell).Object;
            _jobList.Add(productionAverellJobData);

            schedulableLinkedList.AddLast(productionAverellJobData);

            // Act
            manager.Handle(schedulableLinkedList);

            // Assert
            Assert.That(schedulableLinkedList.Count, Is.EqualTo(6), "Not all or to many setup jobs where created.");
            Assert.That(_setupJobs.Count, Is.EqualTo(4), "Not enough setup jobs created.");

            // First Prepare Setup Job
            var current = schedulableLinkedList.First;
            AssertHelper(current, NodePosition.First, SetupClassification.Unspecified, _setupJobs[0].Object);

            // Joe Dalton
            current = current.Next;
            AssertHelper(current, NodePosition.Middle, null, productionJoeJobData);

            // First Cleanup Setup Job.
            current = current.Next;
            AssertHelper(current, NodePosition.Middle, SetupClassification.Unspecified, _setupJobs[1].Object);

            // Second Prepare Setup Job.
            current = current.Next;
            AssertHelper(current, NodePosition.Middle, SetupClassification.Unspecified, _setupJobs[2].Object);

            // Averell Dalton
            current = current.Next;
            AssertHelper(current, NodePosition.Middle, null, productionAverellJobData);

            // Second Cleanup Setup Job.
            current = current.Next;
            AssertHelper(current, NodePosition.Last, SetupClassification.Unspecified, _setupJobs[3].Object);
        }

        [Test(Description = "There should be a prepare and a cleanup job before and after the second job, " +
            "even if the first job caused an exception.")]
        public void ContinueHandlingOfOtherJobsAfterException()
        {
            // Arrange
            _providerMock.Setup(p => p.RequiredSetup(It.IsAny<SetupExecution>(), It.Is<ProductionRecipe>(r => r.Id == 1783), It.IsAny<ISetupTarget>()))
                .Throws<Exception>();
            _providerMock.Setup(p => p.RequiredSetup(It.IsAny<SetupExecution>(), It.Is<ProductionRecipe>(r => r.Id == 1784), It.IsAny<ISetupTarget>()))
                .Returns<SetupExecution, ProductionRecipe, ISetupTarget>(ProvideSetup);
            var manager = CreateSetupManager();

            var recipeJoe = CreateRecipe(1783, 100);
            var productionJoeJobDataMock = CreateProductionJob("Joe Dalton", recipeJoe);
            var productionJoeJobData = productionJoeJobDataMock.Object;
            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { productionJoeJobData });
            _jobList.Add(productionJoeJobData);
            var recipeAverell = CreateRecipe(1784, 100);
            var productionAverellJobDataMock = CreateProductionJob("Averell Dalton", recipeAverell);
            var productionAverellJobData = productionAverellJobDataMock.Object;
            _jobList.Add(productionAverellJobData);

            schedulableLinkedList.AddLast(productionAverellJobData);

            // Act
            manager.Handle(schedulableLinkedList);

            // Assert
            // Joe Dalton
            productionJoeJobDataMock.Verify(j => j.Interrupt(), Times.Once(), "The job for which no setup could be created should be interrupted");

            // Averell Dalton
            Assert.That(schedulableLinkedList.Count, Is.EqualTo(3), "The job for which no clean up could be created should be removed and for " +
                "the other clean up and prepare should be created");
            // Prepare Setup Job.
            var current = schedulableLinkedList.First;
            AssertHelper(current, NodePosition.Middle, SetupClassification.Unspecified, _setupJobs[0].Object);
            // Production Job
            current = current.Next;
            AssertHelper(current, NodePosition.Middle, null, productionAverellJobData);
            // Cleanup Setup Job.
            current = current.Next;
            AssertHelper(current, NodePosition.Last, SetupClassification.Unspecified, _setupJobs[1].Object);
        }

        [Test(Description = "Two jobs of the same recipe should only get one pre- and post setup")]
        public void TwoJobsSameRecipeNoDoubleRecipe()
        {
            // Arrange
            var manager = CreateSetupManager();

            var recipe = CreateRecipe(1783, 42);
            var productionJackJobData = CreateProductionJob("Jack Dalton", recipe).Object;
            var productionWilliamJobData = CreateProductionJob("William Dalton", recipe).Object;
            _jobList.Add(productionJackJobData);
            _jobList.Add(productionWilliamJobData);
            var schedulableLinkedList = new LinkedList<IJobData>(_jobList);

            // Act - Pass jobs to setup manager
            manager.Handle(schedulableLinkedList);

            // Assert
            Assert.That(schedulableLinkedList.Count, Is.EqualTo(4));
            Assert.That(schedulableLinkedList.ElementAt(0), Is.EqualTo(_setupJobs[0].Object));
            Assert.That(schedulableLinkedList.ElementAt(3), Is.EqualTo(_setupJobs[1].Object));
        }

        [Test(Description = "For two jobs of the same recipe that cause an exception in the setup provider, only " +
            "one should be handled and both be interrupted and removed from the list.")]
        public void TwoJobsSameRecipeOnlyOneHandledOnException()
        {
            // Arrange
            _providerMock.Setup(p => p.RequiredSetup(It.IsAny<SetupExecution>(), It.IsAny<ProductionRecipe>(), It.IsAny<ISetupTarget>()))
                .Throws(() => new Exception());
            var manager = CreateSetupManager();

            var recipe = CreateRecipe(1783, 42);
            var productionJackJobDataMock = CreateProductionJob("Jack Dalton", recipe);
            var productionJackJobData = productionJackJobDataMock.Object;
            var productionWilliamJobDataMock = CreateProductionJob("William Dalton", recipe);
            var productionWilliamJobData = productionWilliamJobDataMock.Object;
            _jobList.Add(productionJackJobData);
            _jobList.Add(productionWilliamJobData);
            var schedulableLinkedList = new LinkedList<IJobData>(_jobList);

            // Act - Pass jobs to setup manager
            manager.Handle(schedulableLinkedList);

            // Assert
            Assert.That(schedulableLinkedList.Count, Is.EqualTo(0),
                "The jobs for which no setup could be created should be removed");
            productionJackJobDataMock.Verify(j => j.Interrupt(), Times.Once(),
                $"The job {nameof(productionJackJobData)} for which no setup could be created should be interrupted");
            productionWilliamJobDataMock.Verify(j => j.Interrupt(), Times.Once(),
                $"The job {nameof(productionWilliamJobData)} for which no setup could be created should be interrupted");
            _providerMock.Verify(p => p.RequiredSetup(It.IsAny<SetupExecution>(), It.IsAny<ProductionRecipe>(), It.IsAny<ISetupTarget>()), Times.Once,
                "After a job with a recipe caused an exception in the setup provider no other job of that recipe should be handled.");
        }

        [TestCaseSource(nameof(SchedulableJobs))]
        public void TwoJobsSameRecipeSecondIdle(object jobsParam, object schedulableJobsParam, bool[] setupsMap, bool[] cleanUpsMap)
        {
            // Arrange
            var jobs = (List<IJobData>)jobsParam;
            var schedulableJobs = (LinkedList<IJobData>)schedulableJobsParam;
            _jobList.AddRange(jobs);
            var manager = CreateSetupManager();

            // Act - Pass jobs to setup manager
            manager.Handle(schedulableJobs);

            // Assert
            Assert.That(setupsMap.SequenceEqual(schedulableJobs.Select(j => j.Job.IsPreparingSetup())));
            Assert.That(cleanUpsMap.SequenceEqual(schedulableJobs.Select(j => j.Job.IsCleaningUpSetup())));
        }

        private static IEnumerable<TestCaseData> SchedulableJobs()
        {
            var recipe = CreateRecipe(1783, 42);
            var productionJackJobData = CreateProductionJob("Jack Dalton", recipe, JobClassification.Waiting).Object;
            var productionWilliamJobData = CreateProductionJob("William Dalton", recipe, JobClassification.Idle).Object;
            var jobs = new List<IJobData> { productionJackJobData, productionWilliamJobData };

            bool[] setupsMap = [true, false, false];
            bool[] cleanupsMap = [false, false, true];
            yield return new TestCaseData(jobs, new LinkedList<IJobData>([productionJackJobData]), setupsMap, cleanupsMap)
                .SetDescription("A job should get a clean-up, even if the next one has the same recipe but is not schedulable");

            setupsMap = [true, false, false, false];
            cleanupsMap = [false, false, false, true];
            yield return new TestCaseData(jobs, new LinkedList<IJobData>(jobs), setupsMap, cleanupsMap)
                .SetDescription("A job should not get a clean-up, if the next one has the same recipe but and is schedulable");
        }

        [Test(Description = "Additional production job gets only a dependency on the existing prepare job and will add not other.")]
        public void AdditionalProductionJobOnlyGetsDependenciesOnExistingSetupJobs()
        {
            // Arrange
            var manager = CreateSetupManager();

            var recipe = CreateRecipe(1783, 42);
            var productionJackJobData = CreateProductionJob("Jack Dalton", recipe).Object;

            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { productionJackJobData });

            // create the setup jobs with help of the manager.
            manager.Handle(schedulableLinkedList);
            _jobList.AddRange(schedulableLinkedList);

            // Additional job added to list
            var productionWilliamJobData = CreateProductionJob("William Dalton", recipe).Object;
            var linkedWilliamList = new LinkedList<IJobData>(new List<IJobData> { productionWilliamJobData });
            _jobList.Insert(2, productionWilliamJobData);

            // Act
            manager.Handle(linkedWilliamList);

            // Assert
            Assert.That(_setupJobs.Count, Is.EqualTo(2), "There are more setup jobs than expected. Are the Daltons complete now?");
        }

        [TestCase(true, Description = "RetrySetup should be called once when the SetupJob failed and the state of the SetupJob changed.")]
        [TestCase(false, Description = "RetrySetup should never be called when the SetupJob not failed and the state of the SetupJob changed.")]
        public void SetupJobRetryIsCalledAfterSetupJobFailed(bool failed)
        {
            // Arrange
            var manager = CreateSetupManager();
            var recipe = CreateRecipe(1783, 42);
            var jobData = CreateProductionJob("Ran-Tan-Plan", recipe).Object;

            var schedulableLinkedList = new LinkedList<IJobData>(new List<IJobData> { jobData });
            manager.Handle(schedulableLinkedList);

            _setupJobs[0].SetupGet(r => r.RecipeRequired).Returns(failed);

            SetupRecipe newCreatedRecipe = null;
            _setupJobs[0].Setup(s => s.UpdateSetup(It.IsAny<SetupRecipe>()))
                .Callback((SetupRecipe newRecipe) => { newCreatedRecipe = newRecipe; });

            manager.Start();

            // Act
            var state = new Mock<IJobState>();
            state.SetupGet(s => s.Classification).Returns(JobClassification.Running);
            _jobListMock.Raise(r => r.StateChanged += null, this,
                new JobStateEventArgs(_setupJobs[0].Object, null, state.Object));

            // Assert
            if (failed)
            {
                _setupJobs[0].Verify(s => s.UpdateSetup(It.IsAny<SetupRecipe>()), Times.Once);
                Assert.That(newCreatedRecipe, Is.Not.Null);
                Assert.That(newCreatedRecipe.SetupClassification, Is.EqualTo(SetupClassification.Unspecified));
            }
            else
            {
                _setupJobs[0].Verify(s => s.UpdateSetup(It.IsAny<SetupRecipe>()), Times.Never);
                Assert.That(newCreatedRecipe, Is.Null);
            }
        }

        [TestCase(true, false, Description = "Abort prepare and clean-up on job completion")]
        [TestCase(true, true, Description = "Abort prepare and update clean-up on job completion")]
        [TestCase(false, false, Description = "Abort clean-up upon job completion")]
        [TestCase(false, true, Description = "Update clean-up on job completion")]
        public void AbortSetupsUponJobCompletion(bool hasPrepare, bool needsCleanup)
        {
            // Arrange
            var manager = CreateSetupManager();
            if (!hasPrepare)
                AdjustCurrentResourceCapabilities(42);
            var recipe = CreateRecipe(1783, 42);
            var jobData = CreateProductionJob("Ran-Tan-Plan", recipe);
            _jobList.Add(jobData.Object);

            manager.Start();

            var schedulerLinkedList = new LinkedList<IJobData>(new List<IJobData> { jobData.Object });
            manager.Handle(schedulerLinkedList);
            if (hasPrepare) // If we have prepare AND clean-up, add both
            {
                _jobList.Insert(0, _setupJobs[0].Object);
                _jobList.Add(_setupJobs[1].Object);
            }
            else
            {
                _jobList.Add(_setupJobs[0].Object);
            }

            // Act
            // First the production job is completed, what aborts the prepare
            var stateMock = new Mock<IJobState>();
            stateMock.SetupGet(s => s.Classification).Returns(JobClassification.Completed);
            _jobListMock.Raise(r => r.StateChanged += null, this, new JobStateEventArgs(jobData.Object, null, stateMock.Object));
            // Now the clean-up ist started, which causes recipe update
            if (needsCleanup)
                AdjustCurrentResourceCapabilities(42);
            else
                AdjustCurrentResourceCapabilities(0);
            var setup = _setupJobs[hasPrepare ? 1 : 0];
            setup.SetupGet(j => j.RecipeRequired).Returns(true);
            var state = new Mock<IJobState>();
            state.SetupGet(s => s.Classification).Returns(JobClassification.Running);
            _jobListMock.Raise(r => r.StateChanged += null, this,
                new JobStateEventArgs(setup.Object, null, state.Object));

            // Assert
            if (needsCleanup)
                _setupJobs[hasPrepare ? 1 : 0].Verify(j => j.UpdateSetup(It.IsNotNull<SetupRecipe>()), Times.Once);
            else
                _setupJobs[hasPrepare ? 1 : 0].Verify(j => j.UpdateSetup(null), Times.Once);
            if (hasPrepare)
                _setupJobs[0].Verify(j => j.Abort(), Times.Once);
        }

        [Test(Description = "A new Prepare is necessary if running jobs already executed their clean-up")]
        public void CreateNewPrepareDespiteRunning()
        {
            // Arrange
            var manager = CreateSetupManager();
            var recipe = CreateRecipe(1787, 42);
            var jobData = CreateProductionJob("Completing", recipe);
            _jobList.Add(jobData.Object);

            manager.Start();

            // Add setups
            var linkedList = new LinkedList<IJobData>(new List<IJobData> { jobData.Object });
            manager.Handle(linkedList);
            // "Complete" Pre-Execution and change state
            _jobList.Add(linkedList.Last.Value);
            jobData.SetupGet(j => j.Classification).Returns(JobClassification.Completing);

            // Replacement job AFTER the clean-up
            var replaceScrap = CreateProductionJob("Completing", recipe);
            _jobList.Add(replaceScrap.Object);

            // Act
            linkedList = new LinkedList<IJobData>(new List<IJobData> { replaceScrap.Object });
            manager.Handle(linkedList);

            // Assert
            Assert.That(_setupJobs.Count, Is.EqualTo(4));
        }

        [Test(Description = "If a job was placed before an unstarted clean-up, it does not need a prepare")]
        public void CreateNoPrepareForUnstartedCleanup()
        {
            // Arrange
            var manager = CreateSetupManager();
            var recipe = CreateRecipe(1787, 42);
            var jobData = CreateProductionJob("Completing", recipe);

            manager.Start();

            // Add setups
            var linkedList = new LinkedList<IJobData>(new List<IJobData> { jobData.Object });
            manager.Handle(linkedList);
            // "Complete" Pre-Execution and change state
            _jobList.Add(linkedList.Last.Value);

            // Replacement job BEFORE the clean-up
            var replaceScrap = CreateProductionJob("Replacement", recipe);
            _jobList.Insert(0, replaceScrap.Object);

            // Act
            linkedList = new LinkedList<IJobData>(new List<IJobData> { replaceScrap.Object });
            manager.Handle(linkedList);

            // Assert
            Assert.That(linkedList.Count, Is.EqualTo(1));
            Assert.That(_setupJobs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Create a SetupManager and initialize those fields, that are usually injected
        /// </summary>
        private SetupJobHandler CreateSetupManager()
        {
            var setupManager = new SetupJobHandler
            {
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                JobList = _jobListMock.Object,
                JobFactory = _jobDataFactoryMock.Object,
                ResourceManagement = _resourceManagerMock.Object,
                SetupProvider = _providerMock.Object
            };

            return setupManager;
        }

        private void AdjustCurrentResourceCapabilities(int setupState)
        {
            _resourceManagerMock
                .Setup(rmm => rmm.GetResources<ICell>(It.IsAny<TestSetupCapabilities>()))
                .Returns<ICapabilities>(capabilities =>
                {
                    var setupCapabilities = (TestSetupCapabilities)capabilities;
                    return setupCapabilities.SetupState == setupState
                        ? [new Mock<ICell>().Object]
                        : [];
                });
        }

        /// <summary>
        /// Creates an <see cref="TestRecipe"/> for special test handling
        /// </summary>
        private static TestRecipe CreateRecipe(long id, int state)
        {
            var recipe = new TestRecipe
            {
                Id = id,
                SetupState = state
            };

            return recipe;
        }

        /// <summary>
        /// Creates an <see cref="IProductionJobData"/>
        /// </summary>
        /// <param name="name">name of the JobData to identify it.</param>
        /// <param name="recipe">The Recipe which should be assigned.</param>
        /// <returns>A new <see cref="IProductionJobData"/></returns>
        private static Mock<IProductionJobData> CreateProductionJob(string name, TestRecipe recipe, JobClassification classification = JobClassification.Waiting)
        {
            var productionJobData = new Mock<IProductionJobData> { Name = name };
            productionJobData.SetupGet(j => j.Classification).Returns(classification);
            productionJobData.SetupGet(r => r.Recipe).Returns(recipe);
            productionJobData.SetupGet(j => j.Job).Returns(new EngineJob(recipe, 0));
            productionJobData.As<IJobData>().SetupGet(r => r.Recipe).Returns(recipe);

            return productionJobData;
        }

        private enum NodePosition
        {
            First,
            Middle,
            Last
        }

        private static void AssertHelper(LinkedListNode<IJobData> currentNode, NodePosition nodePosition, SetupClassification? setupClassification, IJobData matchingJob)
        {
            switch (nodePosition)
            {
                case NodePosition.First:
                    Assert.That(currentNode.Previous, Is.Null, "Something is before the first job, that is not correct.");
                    Assert.That(currentNode.Next, Is.Not.Null, "There is nothing behind this job. That is fatal!");
                    break;
                case NodePosition.Middle:
                    Assert.That(currentNode.Next, Is.Not.Null, "There is nothing behind this job. That is fatal!");
                    break;
                case NodePosition.Last:
                    Assert.That(currentNode.Next, Is.Null, "Why is there something behind the last job? Some Ghost Setups occured?");
                    break;
            }

            if (setupClassification != null)
            {
                Assert.That(((SetupRecipe)currentNode.Value.Recipe).SetupClassification, Is.EqualTo(setupClassification), $"The Recipe is not {setupClassification}.");
            }
            Assert.That(matchingJob, Is.EqualTo(currentNode.Value), "The Job at this position is not the one we thought it is.");

        }

        #endregion
    }
}

