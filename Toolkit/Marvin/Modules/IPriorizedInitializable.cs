namespace Marvin.Modules
{
    /// <summary>
    /// Inizializable component wich can set a priority for the initialization process
    /// </summary>
    public interface IPriorizedInitializable : IInitializable
    {
        /// <summary>
        /// Sets an runlevel for the initialization phase
        /// </summary>
        Priority Priority { get; }
    }

    /// <summary>
    /// Priority for the <see cref="IPriorizedInitializable"/>. P0 is the highest priority.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Priority 0
        /// </summary>
        P0,

        /// <summary>
        /// Priority 1
        /// </summary>
        P1,

        /// <summary>
        /// Priority 2
        /// </summary>
        P2,

        /// <summary>
        /// Priority 3
        /// </summary>
        P3,

        /// <summary>
        /// Priority 4
        /// </summary>
        P4,
    }
}