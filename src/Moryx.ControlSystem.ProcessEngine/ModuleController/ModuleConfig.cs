// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.Notifications;
using Moryx.Runtime.Configuration;
using Moryx.Serialization;


namespace Moryx.ControlSystem.ProcessEngine
{
    /// <summary>
    /// Module configuration of the ProcessEngine <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        internal const int DefaultSetupJobRetryLimit = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleConfig"/> class.
        /// </summary>
        public ModuleConfig()
        {
            // Set default scheduler
            JobSchedulerConfig = new ParallelSchedulerConfig { MaxActiveJobs = 1 };
        }

        /// <summary>
        /// Strategy for scheduling jobs.
        /// </summary>
        [DataMember, Description("Strategy for scheduling jobs.")]
        [ModuleStrategy(typeof(IJobScheduler)), PluginConfigs(typeof(IJobScheduler), false)]
        public JobSchedulerConfig JobSchedulerConfig { get; set; }

        /// <summary>
        /// Configured resource selectors
        /// </summary>
        [PluginConfigs(typeof(ICellSelector))]
        [DataMember, Description("Resource selectors to filter and sort possible resources of an activity")]
        public List<CellSelectorConfig> ResourceSelectors { get; set; }

        /// <summary>
        /// Save any progress on product instances during process interruption
        /// </summary>
        [DataMember, Description("Save any progress on product instances during process interruption")]
        public bool SaveInstancesOnInterrupt { get; set; }

        /// <summary>
        /// Workplan executed when part is removed from production process
        /// </summary>
        [DataMember, Description("Message for the unmount activity.")]
        [DefaultValue("Remove broken process from carrier!")]
        public string RemovalMessage { get; set; }

        /// <summary>
        /// Notification config for unassigned activities.
        /// </summary>
        [DataMember, Description("Notification config for unassigned activities.")]
        [DefaultValue(Severity.Error)]
        public Severity UnassignedActivitySeverity { get; set; }

        /// <summary>
        /// Notification config for activity timeouts.
        /// </summary>
        [DataMember, Description("Notification config for activity timeouts.")]
        [DefaultValue(Severity.Warning)]
        public Severity ActivityTimeoutSeverity { get; set; }

        /// <summary>
        /// Strategy to use for the timeout calculation
        /// </summary>
        [DataMember, Description("Strategy to use for the activity timeout calculation.")]
        public TimeoutCalculationType TimeoutCalculation { get; set; }

        /// <summary>
        /// If not <see cref="TimeoutCalculationType.Manual"/> is selected provides the number of activities to use for shutdown timeout calculation
        /// </summary>
        [DataMember, DefaultValue(1000)]
        [Description("Number of activities to use for sigma range shutdown timeout calculation")]
        public int SampleSize { get; set; }

        /// <summary>
        /// If <see cref="TimeoutCalculationType.Manual"/> is selected provides the timeout in seconds
        /// </summary>
        [DataMember, DefaultValue(30)]
        [Description("Value for the manual calculation strategy for activity timeouts in seconds.")]
        public int ManualShutdownTimeoutSec { get; set; }

        /// <summary>
        /// Timeout for the job boot synchronization
        /// </summary>
        [DataMember, DefaultValue(5)]
        [Description("Timeout for the job boot synchronisation")]
        public int BootSyncTimeoutSec { get; set; }

        /// <summary>
        /// Complete initial jobs on reboot
        /// </summary>
        [DataMember, Description("Complete initial jobs on reboot")]
        public bool RebootCompleteInitial { get; set; }

        /// <summary>
        /// Timeout for the job boot synchronization
        /// </summary>
        [DataMember, DefaultValue(5)]
        [Description("Timeout for the job processing on module shutdown")]
        public int JobListStopTimeout { get; set; }

        /// <summary>
        /// Count of retries for a failed setup job. Smaller than 0 for infinite.
        /// </summary>
        [DataMember, DefaultValue(DefaultSetupJobRetryLimit)]
        [Description("Limit of retries for a failed setup job. Smaller than 0 for infinite.")]
        public int SetupJobRetryLimit { get; set; }
    }

    /// <summary>
    /// Class for configured notification content
    /// </summary>
    [DataContract]
    public class NotificationConfig
    {
        /// <summary>
        /// Title of the notification.
        /// </summary>
        [DataMember]
        [Description("Title of the notification.")]
        public string Title { get; set; }

        /// <summary>
        /// Message content of the notification.
        /// </summary>
        [DataMember]
        [Description("Message content of the notification.")]
        public string Message { get; set; }

        /// <summary>
        /// Severity of the notification.
        /// </summary>
        [DataMember]
        [Description("Severity of the notification.")]
        public Severity Severity { get; set; }
    }

    /// <summary>
    /// Timeout calculation type
    /// </summary>
    public enum TimeoutCalculationType
    {
        /// <summary>
        /// One sigma range
        /// </summary>
        OneSigma,

        /// <summary>
        /// Two sigma range
        /// </summary>
        TwoSigma,

        /// <summary>
        /// Three sigma range
        /// </summary>
        ThreeSigma,

        /// <summary>
        /// Estimate timeout manually
        /// </summary>
        Manual,
    }
}
