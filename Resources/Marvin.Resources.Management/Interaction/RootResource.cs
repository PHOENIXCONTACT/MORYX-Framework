using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Root of the resource tree. This can be subclassed for application specific root types
    /// </summary>
    [Description("Root of the resource tree and host of the default web service.")]
    [ResourceRegistration(nameof(RootResource), typeof(IRootResource))]
    public class RootResource : InteractionResource<IResourceInteraction>, IRootResource
    {
        /// <summary>
        /// Unlike the <see cref="InteractionResource{TService}"/> the root resource uses the full
        /// instance as descriptor
        /// </summary>
        public override object Descriptor => this;

        /// <summary>
        /// Constructor to set <see cref="HostConfig"/> defaults.
        /// </summary>
        public RootResource()
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