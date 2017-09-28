using Marvin.Container;
using Marvin.Testing;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// Registration attribute for IRuntimeEnvironment
    /// </summary>
    [OpenCoverIgnore]
    public class RunmodeAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Constructor with life cycle
        /// </summary>
        /// <param name="name">Name of the runtime environment</param>
        public RunmodeAttribute(string name)
            : base(LifeCycle.Singleton, typeof(IRunmode))
        {
            Name = name;
        }
    }
}
