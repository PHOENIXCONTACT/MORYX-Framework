using System;
using System.Linq;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Jobs.Setup;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.ControlSystem.TestTools;
using Moryx.Logging;
using Moryx.Notifications;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    public class JobDataTestBase
    {
        private IProductionRecipe _recipe;
        internal Mock<IJobDispatcher> DispatcherMock { get; private set; }
        internal Mock<INotificationAdapter> NotificationAdapterMock { get; private set; }
        public Mock<IModuleLogger> LoggerMock { get; private set; }

        [SetUp]
        public void FixtureSetup()
        {
            var recipeProviderMock = new Mock<IRecipeProvider>();

            _recipe = DummyRecipe.BuildRecipe();
            _recipe.Id = 42;
            _recipe.Origin = recipeProviderMock.Object;

            recipeProviderMock.Setup(r => r.LoadRecipe(_recipe.Id)).Returns(_recipe);
        }

        [SetUp]
        public virtual void Setup()
        {
            DispatcherMock = new Mock<IJobDispatcher>();
            NotificationAdapterMock = new Mock<INotificationAdapter>();
            LoggerMock = new Mock<IModuleLogger>();
        }
                
        /// <summary>
        /// Creates a new production job with the given amount
        /// </summary>
        internal ProductionJobData GetProductionJob(uint amount)
        {
            var jobData = new ProductionJobData(_recipe, (int)amount)
            {
                Dispatcher = DispatcherMock.Object,
                Logger = LoggerMock.Object
            };
            jobData.ProgressChanged += delegate { };
            return jobData;
        }

        #region SetUp Jobs

        /// <summary>
        /// Creates a new setup job
        /// </summary>
        internal SetupJobData GetSetupJob(SetupExecution setupExecution = SetupExecution.BeforeProduction)
        {
            var setupRecipe = new SetupRecipe
            {
                TargetRecipe = _recipe,
                Workplan = _recipe.Workplan,
                Execution = setupExecution
            };

            // Disable a certain step
            setupRecipe.DisabledSteps.Add(_recipe.Workplan.Steps.First().Id);

            var jobData = new SetupJobData(setupRecipe)
            {
                Dispatcher = DispatcherMock.Object,
                Logger = LoggerMock.Object,
                NotificationAdapter = NotificationAdapterMock.Object,
                ModuleConfig = new ModuleConfig { SetupJobRetryLimit = ModuleConfig.DefaultSetupJobRetryLimit }
            };
            jobData.ProgressChanged += delegate { };
            return jobData;
        }

        /// <summary>
        /// Creates a new setup job in waiting state
        /// </summary>
        internal SetupJobData GetWaitingSetupJob(SetupExecution setupExecution = SetupExecution.BeforeProduction)
        {
            var waitingSetupJob = GetSetupJob(setupExecution);
            waitingSetupJob.Ready();
            return waitingSetupJob;
        }

        /// <summary>
        /// Creates a new setup job in running state
        /// </summary>
        internal SetupJobData GetRunningSetupJob()
        {
            var runningSetupJob = GetWaitingSetupJob();
            runningSetupJob.Start();
            return runningSetupJob;
        }

        /// <summary>
        /// Creates a new setup job in interrupting state
        /// </summary>
        internal SetupJobData GetInterruptingSetupJob()
        {
            var interruptingSetupJob = GetRunningSetupJob();
            interruptingSetupJob.Interrupt();
            return interruptingSetupJob;
        }

        /// <summary>
        /// Creates a new setup job in aborting state
        /// </summary>
        internal SetupJobData GetAbortingSetupJob()
        {
            var abortingSetupJob = GetRunningSetupJob();
            abortingSetupJob.Abort();
            return abortingSetupJob;
        }

        /// <summary>
        /// Creates a new setup job in requesting recipe state
        /// </summary>
        internal SetupJobData GetRequestRecipeSetupJob()
        {
            var requestingRecipeSetupJob = GetWaitingSetupJob(SetupExecution.AfterProduction);
            requestingRecipeSetupJob.Start();
            return requestingRecipeSetupJob;
        }
        
        /// <summary>
         /// Creates a new setup job in retrying setup blocked state
         /// </summary>
        internal SetupJobData GetRetrySetupBlockedSetupJob()
        {
            var retrySetupBlockedSetupJob = GetRunningSetupJob();
            retrySetupBlockedSetupJob.IncrementRetry();
            retrySetupBlockedSetupJob.IncrementRetry();
            retrySetupBlockedSetupJob.IncrementRetry();
            retrySetupBlockedSetupJob.ProcessChanged(new ProcessData(new Process()), ProcessState.Failure);

            return retrySetupBlockedSetupJob;
        }

        /// <summary>
        /// Creates a new setup job in completed state
        /// </summary>
        internal SetupJobData GetCompletedSetupJob()
        {
            var completedSetupJob = GetSetupJob();
            completedSetupJob.Load();
            return completedSetupJob;
        }

        #endregion

        /// <summary>
        /// Transitions which can be called at a job
        /// </summary>
        public enum Transition
        {
            Load,
            Ready,
            Start,
            Stop,
            Complete,
            Abort,
            Interrupt,
            UpdateSetup,
            UnblockRetry
        }

        /// <summary>
        /// Executes the transition by the given enum value
        /// </summary>
        internal void ExecuteTransition(Transition transition, IJobData jobData)
        {
            switch (transition)
            {
                case Transition.Ready:
                    jobData.Ready();
                    break;
                case Transition.Load:
                    jobData.Load();
                    break;
                case Transition.Start:
                    jobData.Start();
                    break;
                case Transition.Stop:
                    jobData.Stop();
                    break;
                case Transition.Complete:
                    jobData.Complete();
                    break;
                case Transition.Abort:
                    jobData.Abort();
                    break;
                case Transition.Interrupt:
                    jobData.Interrupt();
                    break;
                case Transition.UpdateSetup:
                    ((SetupJobData)jobData).UpdateSetup(new SetupRecipe());
                    break;
                case Transition.UnblockRetry:
                    ((SetupJobData)jobData).Acknowledge(new Notification(), null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transition), transition, "Transition should not be part in this case");
            }
        }
    }
}
