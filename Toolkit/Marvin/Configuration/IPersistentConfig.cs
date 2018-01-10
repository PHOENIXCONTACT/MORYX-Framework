namespace Marvin.Configuration
{
    internal interface IPersistentConfig
    {
        /// <summary>
        /// Indicates if the config should be persisted if not changed
        /// </summary>
        bool PersistDefaultConfig { get; }
    }
}
