using Marvin.Modules;
using Marvin.Testing;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Constants class for the bundle name and version
    /// </summary>
    [OpenCoverIgnore]
    public class ResourcesBundleAttribute : BundleAttribute
    {
        /// <summary>
        /// Name string of the resources bundle
        /// </summary>
        public const string Name = "Marvin.Resources";

        /// <summary>
        /// Current version of this bundle
        /// </summary>
        public const string BundeVersion = "2.0.0";

        /// <summary>
        /// One and only constructor for this attribute
        /// </summary>
        public ResourcesBundleAttribute() : base(Name, BundeVersion)
        {
        }
    }
}
