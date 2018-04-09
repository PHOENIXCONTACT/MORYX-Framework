namespace Marvin.Model
{
    /// <summary>
    /// Interface to configure the current database context mode
    /// </summary>
    public interface IContextMode
    {
        /// <summary>
        /// Returns the cureent context mode of the DbContext
        /// </summary>
        ContextMode CurrentMode { get; }

        /// <summary>
        /// Sets the new ContextMode on the DbContext
        /// </summary>
        void Configure(ContextMode mode);
    }
}