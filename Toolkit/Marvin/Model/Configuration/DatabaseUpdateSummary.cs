namespace Marvin.Model
{
    /// <summary>
    /// Result object created from running a database update
    /// </summary>
    public class DatabaseUpdateSummary
    {
        /// <summary>
        /// Flag that any updates were performed
        /// </summary>
        public bool WasUpdated { get; set; }

        /// <summary>
        /// All updates that were executed
        /// </summary>
        public DatabaseUpdate[] ExecutedUpdates { get; set; }
    }
}