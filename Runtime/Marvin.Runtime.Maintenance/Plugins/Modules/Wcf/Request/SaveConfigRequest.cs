namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Request to save the config of a module
    /// </summary>
    public class SaveConfigRequest
    {
        /// <summary>
        /// Config of the module
        /// </summary>
        public Config Config { get; set; }

        /// <summary>
        /// Update mode how the config will be applied
        /// </summary>
        public ConfigUpdateMode UpdateMode { get; set; }
    }
}
