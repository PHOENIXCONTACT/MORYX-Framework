using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Marvin.Serialization;
using Marvin.Tools;

namespace Marvin.Workflows
{
    /// <summary>
    /// Class representing a constructor argument or property based on the entry key DTO
    /// </summary>
    [DataContract]
    public sealed class WorkplanStepInitializer : Entry
    {
        /// <summary>
        /// Indicator that the value is used in the constructor and therefore required
        /// </summary>
        [DataMember]
        public bool FromConstructor { get; set; }

        /// <summary>
        /// Indicator if this argument references a subworkplan
        /// </summary>
        [DataMember]
        public bool SubWorkplan { get; set; }

        /// <summary>
        /// Create initializer for a property - orginal purpose of initializer
        /// </summary>
        internal static WorkplanStepInitializer FromParameter(ParameterInfo param)
        {
            // Create initializer from constuctor parameter
            var defaultValue = param.HasDefaultValue ? param.DefaultValue : null;
            var initAtt = param.GetCustomAttribute<InitializerAttribute>();

            var isWorkplan = WorkplanSerialization.IsWorkplanReference(param.ParameterType);
            var initializer = new WorkplanStepInitializer
            {
                FromConstructor = true,
                SubWorkplan = isWorkplan,
                Key = new EntryKey
                {
                    Identifier = param.Name,
                    Name = initAtt?.DisplayName ?? param.Name
                },
                Value = new EntryValue
                {
                    Type = isWorkplan ? EntryValueType.Int64 : EntryConvert.TransformType(param.ParameterType),
                    Default = defaultValue?.ToString()
                },
                Description = initAtt == null ? string.Empty : initAtt.Description
            };
            AdditonalOperations(initializer, param.ParameterType);

            return initializer;
        }

        /// <summary>
        /// Create initializer from workplan property
        /// </summary>
        internal static WorkplanStepInitializer FromWorkplanProperty(PropertyInfo workplanReference, object instance = null)
        {
            return new WorkplanStepInitializer
            {
                SubWorkplan = true,
                Key = new EntryKey
                {
                    Identifier = workplanReference.Name,
                    Name = workplanReference.Name
                },
                Value = new EntryValue
                {
                    Current = instance == null ? "0" : ((IWorkplan)workplanReference.GetValue(instance)).Id.ToString("D"),
                    Type = EntryValueType.Int64
                },
                Description = workplanReference.GetDescription()
            };
        }

        /// <summary>
        /// Additional operations to complete the different types
        /// </summary>
        /// <returns>True if the field should be skipped, otherwise false</returns>
        private static void AdditonalOperations(Entry entry, Type type)
        {
            switch (entry.Value.Type)
            {
                case EntryValueType.Class:
                    // Fill subentries for this class
                    entry.SubEntries = EntryConvert.EncodeClass(type, WorkplanSerialization.Simple).ToList();
                    break;
                case EntryValueType.Collection:
                    var proto = EntryConvert.ElementType(type);
                    var entryPrototype = new EntryPrototype(proto.Name, Activator.CreateInstance(proto));
                    var prototype = EntryConvert.Prototype(entryPrototype);
                    entry.Prototypes.Add(prototype);
                    break;
                case EntryValueType.Enum:
                    // Set possible values for enums
                    entry.Value.Possible = Enum.GetNames(type);
                    break;
            }
        }
    }
}