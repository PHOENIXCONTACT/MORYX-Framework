using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Serialization;

namespace Moryx.ControlSystem.WorkerSupport
{
    /// <summary>
    /// Module configuration of the ProcessEngine <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Processors to transform instruction content and preview
        /// </summary>
        [DataMember, EntrySerialize]
        [Description("Regex based processors for instructions")]
        public InstructionProcessorConfig[] ProcessorConfigs { get; set; }
    }

    [DataContract]
    public class InstructionProcessorConfig
    {
        /// <summary>
        /// Regex pattern for <see cref="VisualInstruction.Content"/>
        /// </summary>
        [DataMember]
        public string ContentPattern { get; set; }

        /// <summary>
        /// Regex pattern for <see cref="VisualInstruction.Preview"/>
        /// </summary>
        [DataMember]
        public string PreviewPattern { get; set; }

        /// <summary>
        /// Regex replacement for <see cref="VisualInstruction.Content"/>
        /// </summary>
        [DataMember]
        public string ContentReplacement { get; set; }

        /// <summary>
        /// Regex replacement for <see cref="VisualInstruction.Preview"/>
        /// </summary>
        [DataMember]
        public string PreviewReplacement { get; set; }
    }
}