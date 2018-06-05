using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Class representing the shared session between client and server
    /// </summary>
    [DataContract]
    public class WorkplanEditingSession
    {
        /// <summary>
        /// Steps that can be used for this workplan
        /// </summary>
        [DataMember]
        public WorkplanStepRecipe[] AvailableSteps { get; set; }

        /// <summary>
        /// Id of the workplan
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Name of the workplan
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Version of the workplan
        /// </summary>
        [DataMember]
        public int Version { get; set; }

        /// <summary>
        /// Current state of the workplan
        /// </summary>
        [DataMember]
        public WorkplanState State { get; set; }

        /// <summary>
        /// All steps within the workplan
        /// </summary>
        [DataMember]
        public IList<WorkplanStepModel> Steps { get; set; }

        /// <summary>
        /// Connectors linking the inputs and outputs of steps
        /// </summary>
        [DataMember]
        public IList<ConnectorModel> Connectors { get; set; }
    }
}