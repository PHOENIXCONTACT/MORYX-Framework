using System.Runtime.Serialization;
using Marvin.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// DTO representation of a <see cref="IWorkplanStep"/>
    /// </summary>
    [DataContract(IsReference = true)]
    public sealed class WorkplanStepModel
    {
        /// <summary>
        /// Id of the represented step
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Temporary id of the step after creation
        /// </summary>
        [DataMember]
        public string TemporaryId { get; set; }

        /// <summary>
        /// Server side type of this step
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Name of this step
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Classification of the step
        /// </summary>
        [DataMember]
        public StepClassification Classification { get; set; }

        /// <summary>
        /// Inputs of this step
        /// </summary>
        [DataMember]
        public ConnectorModel[] Inputs { get; set; }

        /// <summary>
        /// Outputs of this step
        /// </summary>
        [DataMember]
        public ConnectorModel[] Outputs { get; set; }

        /// <summary>
        /// Descriptions for the outputs
        /// </summary>
        [DataMember]
        public OutputDescriptionDto[] OutputDescriptions { get; set; }

        /// <summary>
        /// Serialized properties of the step
        /// </summary>
        [DataMember]
        public WorkplanStepInitializer[] Properties { get; set; }

        /// <summary>
        /// Reference to a subworkplan
        /// </summary>
        [DataMember]
        public long SubworkplanId { get; set; }
    }
}