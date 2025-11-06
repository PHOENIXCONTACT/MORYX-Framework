// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.Container;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.Logging;
using Moryx.Model.Repositories;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// This pool listener waits till all <see cref="IActivity"/> of a <see cref="IProcess"/> in state
    /// <see cref="ProcessState.Stopping"/> have completed and then sets the state to <see cref="ProcessState.Interrupted"/>.
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener))]
    internal sealed class ProcessInterruption : IActivityPoolListener, IDisposable
    {
        /// <summary>
        /// Logger to create feedback to administrators
        /// </summary>
        [UseChild(nameof(ProcessInterruption))]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Pool with processes
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Factory to open a database connection
        /// </summary>
        public IUnitOfWorkFactory<ProcessContext> UowFactory { get; set; }

        /// <summary>
        /// Module configuration to configure sample size for the dispatcher shutdown
        /// </summary>
        public ModuleConfig Config { get; set; }

        /// <inheritdoc />
        public int StartOrder => 160;

        public void Initialize()
        {
            ActivityPool.ProcessChanged += OnProcessChanged;
            ActivityPool.ActivityChanged += OnActivityChanged;
        }

        public void Start()
        {
        }

        public void Stop()
        {
            var stopWatch = new Stopwatch();

            // Calculate timeout and start stopwatch. We already include the time to find the timeout into the timeout.
            stopWatch.Start();
            var timeOut = TimeoutCalculation();

            // Await completion of processes
            while (stopWatch.ElapsedMilliseconds < timeOut && ActivityPool.ProcessCount > 0)
            {
                Thread.Sleep(1);
            }

            // Stop processes of activities that are still running
            foreach (var process in ActivityPool.Processes)
            {
                ActivityPool.UpdateProcess(process, ProcessState.Interrupted);
            }

            stopWatch.Stop();
        }

        public void Dispose()
        {
            ActivityPool.ProcessChanged -= OnProcessChanged;
            ActivityPool.ActivityChanged -= OnActivityChanged;
        }

        /// <summary>
        /// If the process stops and there are no running activities left, we set it to
        /// <see cref="ProcessState.Interrupted"/> immediately.
        /// </summary>
        private void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            ValidateStableState(args.Trigger, args.ProcessData);
        }

        /// <summary>
        /// When an activity changed to an <see cref="ActivityState.Completed"/> state we can check for a stable state as well.
        /// </summary>
        private void OnActivityChanged(object sender, ActivityEventArgs args)
        {
            if (args.Trigger != ActivityState.Completed)
                return;

            var activityData = args.ActivityData;
            var processData = activityData.ProcessData;
            ValidateStableState(processData.State, processData);
        }

        /// <summary>
        /// Check if the process has reached a stable state
        /// </summary>
        private void ValidateStableState(ProcessState trigger, ProcessData processData)
        {
            if (trigger != ProcessState.Stopping)
                return;

            var activities = processData.Activities;

            // Process was "Initial" after restart. (Interrupt during CompletingInterruptingWaiting) (Activities not loaded jet!)
            if (!activities.Any() && processData.ActivityIndex != 0)
            {
                ActivityPool.UpdateProcess(processData, ProcessState.Interrupted);
                return;
            }

            // Check if process is in a stable state
            if (activities.All(ad => ad.State != ActivityState.Running))
            {
                // Check if process has any progress worth saving
                if (activities.Any(a => a.State >= ActivityState.Running))
                    ActivityPool.UpdateProcess(processData, ProcessState.Interrupted);
                else
                    ActivityPool.UpdateProcess(processData, ProcessState.Discarded);
            }
        }

        /// <summary>
        /// Determines an appropriate timeout. This can be either manually set or by calculating
        /// the average execution time as well as the standard deviation. The config specifies the
        /// range for activities to use
        /// </summary>
        private long TimeoutCalculation()
        {
            // Use manual timeout
            if (Config.TimeoutCalculation == TimeoutCalculationType.Manual)
                return Config.ManualShutdownTimeoutSec * 1000;

            // Fetch sample size and their execution times from db
            IList<double> executionTimes;
            using (var uow = UowFactory.Create())
            {
                var activityRepo = uow.GetRepository<IActivityEntityRepository>();
                executionTimes = (from activity in activityRepo.Linq
                                  where activity.Result != null
                                  orderby activity.Id descending
                                  // This is not fully correct, but faster then ordering by timestamp
                                  select new { activity.Completed, activity.Started })
                    .Take(Config.SampleSize).ToList()
                    .Select(diff => (diff.Completed.Value - diff.Started.Value).TotalMilliseconds).ToList();
            }

            // If there are no activities yet we use manual value as well
            if (!executionTimes.Any())
                return Config.ManualShutdownTimeoutSec * 1000;

            // Calculate average and sigma range
            var averageTime = executionTimes.Average();
            var derivation = (from executionTime in executionTimes
                              let diff = averageTime - executionTime
                              select diff * diff).Average();
            var deviation = Math.Sqrt(derivation);

            var timeout = averageTime;
            switch (Config.TimeoutCalculation)
            {
                case TimeoutCalculationType.OneSigma:
                    timeout += deviation;
                    break;
                case TimeoutCalculationType.TwoSigma:
                    timeout += 2 * deviation;
                    break;
                case TimeoutCalculationType.ThreeSigma:
                    timeout += 3 * deviation;
                    break;
            }

            Logger.Log(LogLevel.Information, "Waiting for {0:F3}s for activities to complete!", timeout / 1000);

            return (long)timeout;
        }
    }
}
