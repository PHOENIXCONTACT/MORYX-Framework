using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public interface INonPublicResource : IResource
    {
    }

    public class NonPublicResource : Resource, INonPublicResource
    {
    }
}