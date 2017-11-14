namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Start behaviour
    /// </summary>
    public enum ModuleStartBehaviour
    {
        /// <summary>
        /// Server module starts automatically 
        /// </summary>
        Auto,

        /// <summary>
        /// Server module is started manually by other modules
        /// </summary>
        Manual,

        /// <summary>
        /// Server module is only started as dependency of other modules
        /// </summary>
        OnDependency
    }
}