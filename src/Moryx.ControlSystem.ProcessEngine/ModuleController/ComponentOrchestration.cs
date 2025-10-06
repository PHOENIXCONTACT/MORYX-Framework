// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Communication.Endpoints;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Setup;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Logging;

namespace Moryx.ControlSystem.ProcessEngine
{
    /// <summary>
    /// Central component to orchestrate the boot and shutdown of the ProcessEngine
    /// </summary>
    [Component(LifeCycle.Singleton)]
    internal class ComponentOrchestration
    {
        #region Dependencies

        public ModuleConfig Config { get; set; }

        public IJobDataList JobList { get; set; }

        public IJobManager JobManager { get; set; }

        public IJobDispatcher JobDispatcher { get; set; }

        public IJobScheduler JobScheduler { get; set; }

        public IJobStorage JobStorage { get; set; }

        public IProcessController ProcessController { get; set; }

        public IProcessStorage ProcessStorage { get; set; }

        public IActivityPoolListener[] PoolListeners { get; set; }

        public ISetupJobHandler SetupManager { get; set; }

        public IEndpointHostFactory HostFactory { get; set; }

        public IModuleLogger Logger { get; set; }

        #endregion

        public void Initialize()
        {
            // Called to make sure the component orchestration resolves all internal components first
        }

        public void Start()
        {
            JobStorage.Start();

            JobScheduler.Initialize(Config.JobSchedulerConfig);

            SetupManager.Start();

            ProcessStorage.Start();

            // Boot all listeners
            foreach (var listener in PoolListeners.OrderBy(l => l.StartOrder))
                listener.Initialize();

            JobList.Start();

            JobDispatcher.Start();

            JobScheduler.Start();

            JobManager.Configure(JobScheduler);
            JobManager.Start();

            // Start execution
            foreach (var listener in PoolListeners.OrderBy(l => l.StartOrder))
                listener.Start();

            if (JobManager.AwaitBoot(Config.BootSyncTimeoutSec))
                Logger.Log(LogLevel.Warning, new TimeoutException("Job manager ran into a timeout trying to restore the previous job list state!"), "Timeout");
        }

        public void Stop()
        {
            try
            {
                JobScheduler.Stop();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Shutting down process engine caused an exception in {name}", JobScheduler.GetType().Name);
            }

            SetupManager.Stop();
            JobDispatcher.Stop();

            JobManager.Stop();

            // Stop all pool listeners
            var ordered = PoolListeners.OrderByDescending(l => l.StartOrder).ToArray();
            foreach (var listener in ordered)
            {
                listener.Stop();
            }

            JobList.Stop();

            JobStorage.Stop();
        }
    }
}
