using System;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Indicates that an result is a failure
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OutputTypeAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputType"></param>
        public OutputTypeAttribute(OutputType outputType)
        {
            OutputType = outputType;
        }

        /// <summary>
        /// Set output type
        /// </summary>
        public OutputType OutputType { get; }
    }
}
