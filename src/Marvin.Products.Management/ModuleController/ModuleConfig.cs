using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Products.Management.Importers;
using Marvin.Runtime.Configuration;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Configuration of this module
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Constructor which will set default values to the interaction host.
        /// </summary>
        public ModuleConfig()
        {
            InteractionHost = new HostConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "ProductInteraction",
                MetadataEnabled = true,
                HelpEnabled = true
            };
        }

        /// <summary>
        /// The interaction host.
        /// </summary>
        [DataMember]
        public HostConfig InteractionHost { get; set; }

        /// <summary>
        /// Released products can be edited
        /// </summary>
        [DataMember, Description("Released products can be edited")]
        public bool ReleasedProductEditable { get; set; }

        /// <summary>
        /// Flag if this application uses recipes
        /// </summary>
        [DataMember, Description("Flag if this application uses recipes")]
        public bool HasRecipes { get; set; }

        /// <summary>
        /// Flag if the module should use the null customization
        /// </summary>
        [DataMember, Description("Flag if the module should use the null customization")]
        public bool UseNullCustomization { get; set; }

        /// <summary>
        /// List of configured importes
        /// </summary>
        [DataMember, Description("List of configured importes")]
        [PluginConfigs(typeof(IProductImporter), false)]
        public List<ProductImporterConfig> Importers { get; set; }
    }
}
