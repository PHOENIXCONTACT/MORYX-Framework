using Marvin.Modules;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Kernel bundle used to flag assemblies with elevated rights for cross bundle access
    /// </summary>
    public class KernelBundleAttribute : BundleAttribute
    {
        /// <summary>
        /// One and only constructor for this attribute
        /// </summary>
        public KernelBundleAttribute() : base("Runtime", RuntimePlatform.RuntimeVersion, true)
        {
        }
    }
}
