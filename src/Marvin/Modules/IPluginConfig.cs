namespace Marvin.Modules
{
    /// <summary>
    /// Interface for all plugin configurations
    /// </summary>
    public interface IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        string PluginName { get; }
    }
}
