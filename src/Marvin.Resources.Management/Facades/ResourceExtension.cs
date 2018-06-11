using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management
{
    internal static class ResourceExtension
    {
        public static TResource Proxify<TResource>(this TResource source, IResourceTypeController typeController)
            where TResource : class, IPublicResource
        {
            return (TResource)typeController.GetProxy(source as Resource);
        }

        public static IEnumerable<TResource> Proxify<TResource>(this IEnumerable<TResource> source, IResourceTypeController typeController)
            where TResource : class, IPublicResource
        {
            return source.Select(r => r.Proxify(typeController));
        }
    }
}