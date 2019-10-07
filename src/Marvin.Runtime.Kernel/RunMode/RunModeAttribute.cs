using System;
using Marvin.Container;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Registration attribute for IRuntimeEnvironment
    /// </summary>
    public class RunModeAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Type of options parsed from arguments
        /// </summary>
        public Type OptionType { get; }

        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="name">Name of the runtime environment</param>
        /// <param name="optionType">Type of options parsed from arguments</param>
        public RunModeAttribute(string name, Type optionType)
            : base(LifeCycle.Singleton, typeof(IRunMode))
        {
            OptionType = optionType;
            Name = name;
        }
    }
}
