// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Notifications;
using Moryx.Orders.Management.Assignment;
using Moryx.Orders.Management.Model;
using Moryx.Orders.Management.Properties;
using Moryx.StateMachines;
using Moryx.Tools;
using Moryx.Users;
using Microsoft.Extensions.Logging;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Business object for operation data
    /// </summary>
    [Component(LifeCycle.Transient, typeof(IOperationData))]
    internal class OperationData : IOperationData, IAsyncStateContext, ILoggingComponent, INotificationSender
    {
        private readonly IOperationSavingContext _savingContext;

        private readonly SemaphoreSlim _stateLock = new(1, 1);

        private readonly List<OperationReport> _reports;
        private readonly List<OperationAdvice> _advices;
        private readonly List<Job> _jobs;

        private DispatchHandler _dispatchHandler;
        private OperationDataStateBase _state;

        #region Dependencies

        public IModuleLogger Logger { get; set; }

        public IJobHandler JobHandler { get; set; }

        public IOperationAssignment OperationAssignment { get; set; }

        public ICountStrategy CountStrategy { get; set; }

        public INotificationAdapter NotificationAdapter { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        public OperationData(IOperationSavingContext savingContext)
        {
            _savingContext = savingContext;
            Operation = new InternalOperation();
            _reports = new List<OperationReport>();
            _advices = new List<OperationAdvice>();
            _jobs = [];
        }

        async Task IAsyncStateContext.SetStateAsync(StateBase state, CancellationToken cancellationToken)
        {
            // ReSharper disable InconsistentlySynchronizedField
            _state = (OperationDataStateBase)state;

            Operation.State = _state.GetFullClassification();
            Operation.StateDisplayName = _state.GetType().GetDisplayName();

            await _savingContext.SaveOperation(this);

            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        /// <inheritdoc cref="IOperationData"/>
        public InternalOperation Operation { get; }

        string INotificationSender.Identifier => $"{OrderData.Number}-{Number}";

        /// <inheritdoc cref="IOperationData"/>
        public Guid Identifier => Operation.Identifier;

        /// <inheritdoc cref="IOperationData"/>
        public string Number => Operation.Number;

        /// <inheritdoc cref="IOperationData"/>
        public int TotalAmount => Operation.TotalAmount;

        /// <inheritdoc cref="IOperationData"/>
        public int TargetAmount => Operation.TargetAmount;

        /// <inheritdoc cref="IOperationData"/>
        public ProductType Product => Operation.Product;

        public IOrderData OrderData { get; set; }

        /// <inheritdoc />
        public OperationAssignState AssignState
        {
            get;
            set
            {
                field = value;
                //Update the state of the internal operation
                if (_state is not null)
                    Operation.State = _state.GetFullClassification();
            }
        }

        /// <inheritdoc cref="IOperationData"/>
        // ReSharper disable once InconsistentlySynchronizedField
        public IOperationState State => _state;

        /// <summary>
        /// Flag if operation have reached the amount.
        /// </summary>
        internal bool AmountReached => CountStrategy.AmountReached(Operation);

        /// <summary>
        /// Flag if the operation can reach the amount with the current jobs
        /// </summary>
        internal bool CanReachAmount => CountStrategy.CanReachAmount(Operation);

        /// <summary>
        /// Sum of SuccessCount and RunningCount of jobs. All running will be classified as "success"
        /// </summary>
        internal int ReachableAmount => CountStrategy.ReachableAmount(Operation);

        /// <inheritdoc />
        public async Task<IOperationData> Initialize(OperationCreationContext context, IOrderData orderData, IOperationSource source)
        {
            await StateMachine.InitializeAsync(this).WithAsync<OperationDataStateBase>();
            _dispatchHandler = new DispatchHandler(this);

            AssignState = OperationAssignState.Initial;

            OrderData = orderData;
            orderData.AddOperation(this);

            Operation.Identifier = Guid.NewGuid();
            Operation.CreationContext = context;
            Operation.Source = source;
            Operation.Name = context.Name;
            Operation.Parts = context.Parts?.Select(p => new ProductPart
            {
                Name = p.Name,
                Identity = new ProductIdentity(p.Number, 0),
                Quantity = p.Quantity,
                Unit = p.Unit,
                StagingIndicator = p.StagingIndicator,
                Classification = p.Classification
            }).ToArray() ?? [];

            Operation.Number = context.Number;
            Operation.TotalAmount = context.TotalAmount;
            Operation.PlannedStart = context.PlannedStart;
            Operation.PlannedEnd = context.PlannedEnd;
            Operation.TargetCycleTime = context.TargetCycleTime;
            Operation.Unit = context.Unit;
            Operation.TargetStock = context.TargetStock;

            var overDeliveryAmount = context.OverDeliveryAmount;
            if (overDeliveryAmount < context.TotalAmount)
                overDeliveryAmount = context.TotalAmount;

            Operation.OverDeliveryAmount = overDeliveryAmount;

            var underDeliveryAmount = context.UnderDeliveryAmount;
            if (underDeliveryAmount > context.TotalAmount)
                underDeliveryAmount = context.TotalAmount;

            Operation.UnderDeliveryAmount = underDeliveryAmount;

            // Facade references
            Operation.Product = new ProductReference(new ProductIdentity(context.ProductIdentifier, context.ProductRevision));

            return this;
        }

        /// <inheritdoc />
        public async Task<IOperationData> Initialize(OperationEntity entity, IOrderData orderData)
        {
            _dispatchHandler = new DispatchHandler(this);

            OrderData = orderData;
            orderData.AddOperation(this);

            OperationStorage.RestoreOperationData(this, entity);

            await StateMachine.ReloadAsync(this, entity.State).WithAsync<OperationDataStateBase>();

            return this;
        }

        /// <inheritdoc />
        public async Task SetSortOrder(int sortOrder)
        {
            if (sortOrder == Operation.SortOrder)
                return;

            Operation.SortOrder = sortOrder;

            await _savingContext.SaveOperation(this);

            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        /// <summary>
        /// Adds the given reports to the internal report list without locking or raising events
        /// </summary>
        public void RestoreReportsUnsynchronized(IReadOnlyList<OperationReport> reports)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _reports.AddRange(reports);
            Operation.Reports = reports.ToArray();
        }

        /// <summary>
        /// Add the given advices to the internal advice list without locking or raising events
        /// </summary>
        public void RestoreAdvicesUnsynchronized(IReadOnlyList<OperationAdvice> advices)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _advices.AddRange(advices);
            Operation.Advices = advices.ToArray();
        }

        /// <summary>
        /// Adds the given job to the internal job list, without locking and raising events
        /// </summary>
        public void RestoreJobsUnsynchronized(IReadOnlyList<Job> jobs)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _jobs.AddRange(jobs);
            Operation.Jobs = jobs.ToArray();
        }

        /// <inheritdoc cref="IOperationData"/>
        public async Task AddJob(Job job)
        {
            Log(LogLevel.Information, "Job {0} was added", job.Id);

            lock (_jobs)
            {
                _jobs.Add(job);
                Operation.Jobs = _jobs.ToArray();
            }

            UpdateProgress();
            await _savingContext.SaveOperation(this);
            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        /// <inheritdoc cref="IOperationData"/>
        public Task Assign()
        {
            Log(LogLevel.Information, "Starting assignment");

            return _stateLock.ExecuteAsync(() => _state.Assign());
        }

        /// <inheritdoc cref="IOperationData"/>
        public Task AssignCompleted(bool success)
        {
            if (success)
                Operation.CreationContext = null;

            return _stateLock.ExecuteAsync(() => _state.AssignCompleted(success));
        }

        internal async Task HandleAssignCompleted(bool success)
        {
            Log(success ? LogLevel.Information : LogLevel.Warning, "Assignment completed and was {0}",
                success ? "successful" : "not successful");

            AssignState = success ? OperationAssignState.Assigned : OperationAssignState.Failed;
            await _savingContext.SaveOperation(this);
            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        internal void HandleAssign()
        {
            OperationAssignment.Assign(this);
        }

        internal void HandleReassign()
        {
            OperationAssignment.Reassign(this);
        }

        /// <inheritdoc cref="IOperationData"/>
        public Task Abort()
        {
            Log(LogLevel.Information, "Aborting operation");

            return _stateLock.ExecuteAsync(() => _state.Abort());
        }

        internal async Task HandleAbort()
        {
            Aborted?.Invoke(this, new OperationEventArgs(this));
            await _savingContext.RemoveOperation(this);
        }

        /// <inheritdoc cref="IOperationData"/>
        public async Task Restore()
        {
            var restorableJobs = StashRestorableJobs();
            RestoreJobs(restorableJobs);

            // Restore creation information
            await OperationAssignment.Restore(this);

            // Send update
            UpdateProgress();
            await _savingContext.SaveOperation(this);
            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        private long[] StashRestorableJobs()
        {
            long[] restorableJobs;
            lock (_jobs)
            {
                restorableJobs = _jobs.Select(j => j.Id).ToArray();
                _jobs.Clear();
            }

            return restorableJobs;
        }

        private void RestoreJobs(long[] restorableJobs)
        {
            var restoredJobs = JobHandler.Restore(restorableJobs);
            lock (_jobs)
            {
                _jobs.AddRange(restoredJobs);
                Operation.Jobs = _jobs.ToArray();
            }
            var jobInformation = restorableJobs.Length != 0 ? $"Jobs {string.Join(", ", restorableJobs)}" : "No jobs";
            Log(LogLevel.Information, "{jobInformation} were restored", jobInformation);
        }

        /// <inheritdoc cref="IOperationData"/>
        public async Task Resume()
        {
            // Restore on state
            await _stateLock.ExecuteAsync(() => _state.Resume());
        }

        /// <inheritdoc cref="IOperationData"/>
        public BeginContext GetBeginContext()
        {
            var context = GetOperationInfo<BeginContext>();

            // If there are not produced parts, the residual amount will be set
            // If there are more produced parts as planned, the residual amount is 0
            context.ResidualAmount = Operation.TotalAmount > Operation.TargetAmount ? Operation.TotalAmount - Operation.TargetAmount : 0;
            context.PartialAmount = Operation.TargetAmount;
            context.MinimalTargetAmount = Operation.TargetAmount;
            context.CanReduce = _state.CanReduceAmount;

            return context;
        }

        /// <inheritdoc cref="IOperationData"/>
        public async Task Adjust(int amount, User user)
        {
            Log(LogLevel.Information, "The target amount of the operation will be adjusted by amount {amount} by user {user}",
                amount, user.Identifier);

            await _stateLock.ExecuteAsync(() =>
            {
                if (amount >= 0)
                {
                    return _state.IncreaseTargetBy(amount, user);
                }
                else
                {
                    return _state.DecreaseTargetBy(amount, user);
                }
            });
        }

        /// <summary>
        /// Calculates the current amount and dispatches a job
        /// </summary>
        internal async Task HandleIncreaseTargetBy(int partialAmount)
        {
            // Save the first start time of the production
            Operation.Start ??= DateTime.Now;

            // partial amount can be 0 to restart an interrupting operation
            // We do not have to increase the TargetAmount so far
            if (partialAmount > 0)
            {
                Operation.TargetAmount += partialAmount;
                await _savingContext.SaveOperation(this);
                Updated?.Invoke(this, new OperationEventArgs(this));
            }

            await DispatchJob();
        }

        /// <summary>
        /// Calculates the new target amount, completes the current jobs and
        /// dispatches a job if necessary
        /// </summary>
        internal async Task HandleDecreaseTargetBy(int amount)
        {
            Operation.TargetAmount += amount;
            await JobHandler.Complete(this);
            await DispatchJob();
            await _savingContext.SaveOperation(this);
            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        /// <summary>
        /// Raises the started event
        /// </summary>
        internal void HandleStarted(User user)
        {
            Started?.Invoke(this, new StartedEventArgs(this, user));
        }

        /// <param name="user"></param>
        /// <inheritdoc cref="IOperationData"/>
        public Task Interrupt(User user)
        {
            Log(LogLevel.Debug, "Operation will be interrupted by user {0}",
                user.Identifier);

            return _stateLock.ExecuteAsync(() => _state.Interrupt(user));
        }

        /// <summary>
        /// Will complete all jobs and executes a partial report
        /// </summary>
        internal Task HandleManualInterrupting()
        {
            return JobHandler.Complete(this);
        }

        /// <summary>
        /// Will handle manual interrupts. The interrupt was triggered by the user.
        /// Will throw the <see cref="Interrupted"/> event
        /// </summary>
        internal async Task HandleManualInterrupted()
        {
            Operation.TargetAmount = ReachableAmount;

            await _savingContext.SaveOperation(this);
            Updated?.Invoke(this, new OperationEventArgs(this));
            Interrupted?.Invoke(this, new OperationEventArgs(this));
        }

        /// <summary>
        /// Will handle the interrupt if all jobs are completed
        /// Will be called by the state machine
        /// </summary>
        internal async Task HandleInterrupted()
        {
            Operation.TargetAmount = ReachableAmount;

            await _savingContext.SaveOperation(this);

            Updated?.Invoke(this, new OperationEventArgs(this));
            Interrupted?.Invoke(this, new OperationEventArgs(this));
        }

        /// <inheritdoc cref="IOperationData"/>
        public Task Report(OperationReport report)
        {
            Log(LogLevel.Information, "Operation will be reported with SuccessCount {0} and FailureCount {1} by user {2}",
                report.SuccessCount, report.FailureCount, report.User.Identifier);

            if (report.SuccessCount < 0 || report.FailureCount < 0)
            {
                const string error = "Amounts less than zero cannot be reported!";
                Log(LogLevel.Error, error);
                throw new ArgumentException(error);
            }

            return _stateLock.ExecuteAsync(() => _state.Report(report));
        }

        /// <inheritdoc cref="IOperationData"/>
        public AdviceContext GetAdviceContext()
        {
            var adviceContext = _stateLock.Execute(() => _state.GetAdviceContext());

            return adviceContext;
        }

        internal AdviceContext HandleAdviceContext()
        {
            return new()
            {
                AdvicedAmount = Operation.Advices.OfType<OrderAdvice>().Sum(a => a.Amount)
            };
        }

        /// <inheritdoc cref="IOperationData"/>
        public Task Advice(OperationAdvice advice)
        {
            Log(LogLevel.Information, "Operation will be adviced for ToteBoxNumber {0}", advice.ToteBoxNumber);

            var orderAdvice = advice as OrderAdvice;
            var pickPartAdvice = advice as PickPartAdvice;

            void ThrowError(string error)
            {
                Log(LogLevel.Error, error);
                throw new ArgumentException(error);
            }

            if (orderAdvice is { Amount: <= 0 })
                ThrowError("Amount less then or equals zero cannot be adviced!");

            if (pickPartAdvice != null && !Operation.Parts.Contains(pickPartAdvice.Part))
                ThrowError("The part to advice is not part of the operation!");

            if (orderAdvice == null && pickPartAdvice == null)
                ThrowError("Advices of type " + advice.GetType().Name + " cannot be handled.");

            return _stateLock.ExecuteAsync(() => _state.Advice(advice));
        }

        internal async Task HandleAdvice(OperationAdvice advice)
        {
            lock (_advices)
            {
                _advices.Add(advice);
                Operation.Advices = _advices.ToArray();
            }

            await _savingContext.SaveOperation(this);
            Updated?.Invoke(this, new OperationEventArgs(this));
            Adviced?.Invoke(this, new AdviceEventArgs(this, advice));
        }

        /// <summary>
        /// Will throw the <see cref="Completed"/> event
        /// </summary>
        internal async Task HandleCompleted(OperationReport report)
        {
            Operation.End = DateTime.Now;

            lock (_reports)
            {
                _reports.Add(report);
                Operation.Reports = _reports.ToArray();
            }

            await _savingContext.SaveOperation(this);

            Updated?.Invoke(this, new OperationEventArgs(this));
            Completed?.Invoke(this, new ReportEventArgs(this, report));
        }

        /// <summary>
        /// Will throw the <see cref="PartialReport"/> event
        /// </summary>
        internal async Task HandlePartialReport(OperationReport report)
        {
            lock (_reports)
            {
                _reports.Add(report);
                Operation.Reports = _reports.ToArray();
            }

            await _savingContext.SaveOperation(this);

            Updated?.Invoke(this, new OperationEventArgs(this));
            PartialReport?.Invoke(this, new ReportEventArgs(this, report));
        }

        /// <inheritdoc cref="IOperationData"/>
        public ReportContext GetReportContext()
        {
            var reportContext = _stateLock.Execute(() => _state.GetReportContext());
            return reportContext;
        }

        internal ReportContext HandleReportContext()
        {
            var reportedSuccess = Operation.Reports.Sum(r => r.SuccessCount);
            var reportedFailure = Operation.Reports.Sum(r => r.FailureCount);

            var unreportedFailure = Operation.Progress.ScrapCount - reportedFailure;
            if (unreportedFailure < 0)
                unreportedFailure = 0;

            var unreportedSuccess = Operation.Progress.SuccessCount - reportedSuccess;
            if (unreportedSuccess < 0)
                unreportedSuccess = 0;

            var context = GetOperationInfo<ReportContext>();

            context.UnreportedFailure = unreportedFailure;
            context.UnreportedSuccess = unreportedSuccess;

            context.ReportedSuccess = reportedSuccess;
            context.ReportedFailure = reportedFailure;

            return context;
        }

        /// <inheritdoc cref="IOperationData"/>
        public void AssignProduct(ProductType productType)
        {
            Operation.Product = productType;
        }

        /// <inheritdoc cref="IOperationData"/>
        public async Task AssignRecipes(IReadOnlyList<IProductRecipe> recipes)
        {
            // Replaces the recipes with the given new list
            Operation.Recipes.Clear();
            Operation.Recipes.AddRange(recipes);

            await _savingContext.SaveOperation(this);

            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        public async Task RecipeChanged(IProductRecipe productRecipe)
        {
            if (productRecipe.TemplateId == 0)
            {
                AssignState |= OperationAssignState.Changed;
                Log(LogLevel.Debug, "Template changed. AssignState is now 'Changed'");

                Updated?.Invoke(this, new OperationEventArgs(this));
            }
            else
            {
                Log(LogLevel.Debug, "Recipe changed. Recipe instance will be updated");

                // Only update a recipe if it is part of the current recipes
                var affectedRecipe = Operation.Recipes.FirstOrDefault(r => r.Id == productRecipe.Id);
                if (affectedRecipe == null)
                    return;

                Operation.Recipes.Remove(affectedRecipe);
                Operation.Recipes.Add(productRecipe);

                await _savingContext.SaveOperation(this);

                Updated?.Invoke(this, new OperationEventArgs(this));

                // Complete running jobs to stop the production of the outdated recipe
                // New jobs will be dispatched automatically with the new recipes
                await JobHandler.Complete(this);
            }
        }

        /// <inheritdoc cref="IOperationData"/>
        public async Task JobProgressChanged(Job job)
        {
            UpdateProgress();

            await _stateLock.ExecuteAsync(() => _state.ProgressChanged(job));

            ProgressChanged?.Invoke(this, new OperationEventArgs(this));
        }

        /// <inheritdoc cref="IOperationData"/>
        public async Task JobStateChanged(JobStateChangedEventArgs args)
        {
            UpdateProgress();

            await _stateLock.ExecuteAsync(() => _state.JobsUpdated(args));
        }

        internal Task DispatchJob()
        {
            return _dispatchHandler.TryDispatch();
        }

        /// <inheritdoc />
        public async Task UpdateSource(IOperationSource source)
        {
            Operation.Source = source;

            await _savingContext.SaveOperation(this);
            Updated?.Invoke(this, new OperationEventArgs(this));
        }

        private void UpdateProgress()
        {
            var relevantJobs = CountStrategy.RelevantJobs(Operation).ToArray();
            var progress = Operation.Progress;

            // Progress on relevant jobs
            progress.RunningCount = relevantJobs.Sum(j => j.RunningProcesses.Count);
            progress.SuccessCount = relevantJobs.Sum(j => j.SuccessCount);
            progress.FailureCount = relevantJobs.Sum(j => j.FailureCount);
            progress.ReworkedCount = relevantJobs.Sum(j => j.ReworkedCount);
            progress.PendingCount = relevantJobs.Where(j => j.Classification <= JobClassification.Running)
                .Sum(j => j.Amount - j.SuccessCount - j.RunningProcesses.Count - j.FailureCount);

            var scrap = progress.FailureCount - progress.ReworkedCount;
            progress.ScrapCount = scrap < 0 ? 0 : scrap;

            // Progress on all jobs
            var allJobs = Operation.Jobs; // use thread save list
            progress.ProgressRunning = allJobs.Sum(j => j.RunningProcesses.Count);
            progress.ProgressSuccess = allJobs.Sum(j => j.SuccessCount);
            progress.ProgressScrap = allJobs.Sum(j => j.FailureCount - j.ReworkedCount) >= 0
                ? allJobs.Sum(j => j.FailureCount - j.ReworkedCount) : 0;

            progress.ProgressPending =
                allJobs.Where(j => j.Classification < JobClassification.Completed).Sum(j => j.Amount) +
                allJobs.Where(j => j.Classification == JobClassification.Completed).Sum(j => j.SuccessCount) -
                allJobs.Sum(j => j.SuccessCount + j.RunningProcesses.Count + j.FailureCount);
        }

        private void Log(LogLevel logLevel, string message, params object[] parameters) =>
            Logger.Log(logLevel, $"{OrderData.Number}-{Operation.Number}: {message}", parameters);

        private TInfo GetOperationInfo<TInfo>()
            where TInfo : OperationInfo, new()
        {
            var info = new TInfo();

            // do all in a single job lock
            var successCount = Operation.Progress.SuccessCount;
            var scrapCount = Operation.Progress.ScrapCount;

            if (scrapCount < 0)
                scrapCount = 0;

            info.SuccessCount = successCount;
            info.ScrapCount = scrapCount;

            return info;
        }

        /// <inheritdoc cref="IOperationData.Updated"/>
        public event EventHandler<OperationEventArgs> Updated;

        /// <inheritdoc cref="IOperationData.Updated"/>
        public event EventHandler<OperationEventArgs> Aborted;

        /// <inheritdoc cref="IOperationData.Started"/>
        public event EventHandler<StartedEventArgs> Started;

        /// <inheritdoc cref="IOperationData.Interrupted"/>
        public event EventHandler<OperationEventArgs> Interrupted;

        /// <inheritdoc cref="IOperationData.Completed"/>
        public event EventHandler<ReportEventArgs> Completed;

        /// <inheritdoc cref="IOperationData.PartialReport"/>
        public event EventHandler<ReportEventArgs> PartialReport;

        /// <inheritdoc cref="IOperationData.Adviced"/>
        public event EventHandler<AdviceEventArgs> Adviced;

        /// <inheritdoc cref="IOperationData.ProgressChanged"/>
        public event EventHandler<OperationEventArgs> ProgressChanged;

        /// <summary>
        /// Handler to ensure a save dispatching.
        /// Sometimes it can happen that a job update trigger another dispatching while the dispatching isn't finally done
        /// </summary>
        private sealed class DispatchHandler
        {
            private readonly OperationData _operationData;
            private readonly ICountStrategy _countStrategy;
            private readonly IJobHandler _jobHandler;
            private readonly Lock _dispatchLock = new();

            private bool _isDispatching;
            private bool _isDispatchingRequested;

            public DispatchHandler(OperationData operationData)
            {
                _operationData = operationData;
                _countStrategy = operationData.CountStrategy;
                _jobHandler = operationData.JobHandler;
            }

            public async Task TryDispatch()
            {
                lock (_dispatchLock)
                {
                    // If the operation is currently dispatching then remember that another dispatch is requested.
                    // Otherwise just remember that right now a dispatch will be started.
                    if (_isDispatching)
                    {
                        _isDispatchingRequested = true;
                        return;
                    }

                    _isDispatching = true;
                }

                var missingAmounts = _countStrategy.MissingAmounts(_operationData.Operation);
                if (missingAmounts.Count == 0)
                {
                    _operationData.Log(LogLevel.Error, "There is nothing to dispatch. Check the {0}.", _countStrategy.GetType().Name);

                    await TryRequestedDispatches();
                    return;
                }

                _operationData.Log(LogLevel.Debug, "At least one job should be dispatched");

                await _jobHandler.Dispatch(_operationData, missingAmounts);
                await TryRequestedDispatches();
            }

            private async Task TryRequestedDispatches()
            {
                lock (_dispatchLock)
                {
                    _isDispatching = false;

                    if (!_isDispatchingRequested)
                        return;

                    // Dispatch again if a dispatch was requested during the last dispatching
                    _isDispatchingRequested = false;
                }

                await TryDispatch();
            }
        }

        #region Notifications

        void INotificationSender.Acknowledge(Notification notification, object tag)
        {
            NotificationAdapter.Acknowledge(this, notification);
        }

        internal void ShowAmountReachedNotification()
        {
            if (ModuleConfig.DisableAmountReachedNotification)
                return;

            NotificationAdapter.Publish(this, new Notification(Strings.OperationData_AmountReachedNotificationTitle,
                string.Format(Strings.OperationData_AmountReachedNotificationMessage,
                    $"{OrderData.Number}-{Number}"), Severity.Info));

        }

        internal void AcknowledgeAmountReachedNotification()
        {
            NotificationAdapter.AcknowledgeAll(this);
        }

        #endregion
    }
}

