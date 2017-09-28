using System.Runtime.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance
{
    /// <summary>
    /// Result of the database dump.
    /// </summary>
    [DataContract]
    public class DumpResult
    {
        /// <summary>
        /// Constructor for the databse dump result.
        /// </summary>
        /// <param name="message">The message of the result.</param>
        public DumpResult(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The message of the result.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Name of the dump.
        /// </summary>
        [DataMember]
        public string DumpName { get; set; }
    }
}
