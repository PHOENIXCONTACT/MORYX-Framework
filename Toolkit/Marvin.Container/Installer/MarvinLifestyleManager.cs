using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Marvin.Testing;

namespace Marvin.Container
{
    /// <summary>
    /// Marvin custom lifestyle that does not track created instances
    /// </summary>
    public class MarvinLifestyleManager : TransientLifestyleManager
    {
        /// <summary>
        /// Empty track implementation that does not track instances
        /// </summary>
        [OpenCoverIgnore]
        protected override void Track(Burden burden, IReleasePolicy releasePolicy)
        {
        }
    }
}
