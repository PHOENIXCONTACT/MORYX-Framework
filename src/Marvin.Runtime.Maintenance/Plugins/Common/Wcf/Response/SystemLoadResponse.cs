namespace Marvin.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Response model for the system load
    /// </summary>
    public class SystemLoadResponse
    {
        /// <summary>
        /// Current cpu load
        /// </summary>
        public ulong CPULoad { get; set; }

        /// <summary>
        /// Current memory usage in percent
        /// </summary>
        public double SystemMemoryLoad { get; set; }
    }
}
