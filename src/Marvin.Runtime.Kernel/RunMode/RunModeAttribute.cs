using Marvin.Container;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Registration attribute for IRuntimeEnvironment
    /// </summary>
    public class RunModeAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="name">Name of the runtime environment</param>
        public RunModeAttribute(string name)
            : base(LifeCycle.Singleton, typeof(IRunMode))
        {
            Name = name;
        }
    }
}
