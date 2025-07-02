using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Config for the seamless scheduler strategy
    /// </summary>
    internal class SeamlessSchedulerConfig: JobSchedulerConfig
    {
        /// <inheritdoc />
        public override string PluginName => nameof(SeamlessScheduler);
    }
}