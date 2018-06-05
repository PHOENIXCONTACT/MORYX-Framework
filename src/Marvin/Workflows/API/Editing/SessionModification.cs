using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Incremental modification of the session raised by the server and consumed by the client
    /// </summary>
    [DataContract]
    public sealed class SessionModification
    {
        /// <summary>
        /// Operation the user performed
        /// </summary>
        [DataMember]
        public UserOperation Operation { get; set; }

        /// <summary>
        /// Steps that were added, updated or removed
        /// </summary>
        [DataMember]
        public ModifiedStep[] StepModifications { get; set; }

        /// <summary>
        /// Connectors that were added or removed
        /// </summary>
        [DataMember]
        public ModifiedConnector[] ConnectorModifications { get; set; }

        /// <summary>
        /// Create a modification summary
        /// </summary>
        internal static SessionModificationContext Summary(UserOperation operation)
        {
            return new SessionModificationContext(operation);
        }
    }
}