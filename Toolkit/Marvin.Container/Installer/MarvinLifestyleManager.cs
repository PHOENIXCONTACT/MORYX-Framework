using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;

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
        protected override void Track(Burden burden, IReleasePolicy releasePolicy)
        {
        }
    }
}
