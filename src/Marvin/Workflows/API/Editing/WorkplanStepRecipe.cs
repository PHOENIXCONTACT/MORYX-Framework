using System;
using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Exchange type for workplans 
    /// </summary>
    [DataContract]
    public sealed class WorkplanStepRecipe
    {
        /// <summary>
        /// Id set on recipe by client to identifiy response
        /// </summary>
        [DataMember]
        public string TemporaryId { get; set; }

        /// <summary>
        /// Server side index of this step type
        /// </summary>
        [DataMember]
        public int Index { get; set; }

        /// <summary>
        /// Name of this step
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Long description of this step
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Classification of the step
        /// </summary>
        [DataMember]
        public StepClassification Classification { get; set; }

        /// <summary>
        /// All initializers required to create an instance of this step
        /// </summary>
        [DataMember]
        public WorkplanStepInitializer[] Initializers { get; set; }
    }
}