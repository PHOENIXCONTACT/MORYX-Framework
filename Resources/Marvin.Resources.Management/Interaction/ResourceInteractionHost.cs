using System.ComponentModel;
using Marvin.AbstractionLayer.Resources;
using Marvin.Resources.Interaction;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Default interaction resource which hosts the default web service
    /// </summary>
    [Description("Resource to host the default web service")]
    [ResourceRegistration(nameof(ResourceInteractionHost), typeof(IDefaultResource))]
    internal sealed class ResourceInteractionHost : InteractionResource<IResourceInteraction>, IDefaultResource
    {
        /// <summary>
        /// Constructor to set <see cref="HostConfig"/> defaults.
        /// </summary>
        public ResourceInteractionHost()
        {
            HostConfig = new HostConfig
            {
                Endpoint = "ResourceInteraction",
                BindingType = ServiceBindingType.BasicHttp,
                MetadataEnabled = true,
            };
        }
    }
}