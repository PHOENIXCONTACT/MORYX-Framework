namespace Marvin.Model
{
    /// <summary>
    /// Class to store information of a database update
    /// </summary>
    public class DatabaseUpdateInformation
    {
        /// <summary>
        /// Name of the update
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Inidicates if the update is installed
        /// </summary>
        public bool IsApplied { get; set; }
    }
}
