using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public interface IExtension
    {
        int Add(int value);
    }

    public interface IResourceWithImplicitApi : IPublicResource, IExtension
    {
        
    }

    public class ResourceWithImplicitApi : PublicResource, IResourceWithImplicitApi
    {
        public int Add(int value)
        {
            return value + 10;
        }
    }
}