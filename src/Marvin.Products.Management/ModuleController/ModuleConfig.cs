using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer;
using Marvin.Configuration;
using Marvin.Products.Management.Importers;
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
        /// List of configured importers
        /// </summary>
        [DataMember, Description("List of configured importes")]
        [PluginConfigs(typeof(IProductImporter), false)]
        public List<ProductImporterConfig> Importers { get; set; }

        /// <summary>
        /// TODO: Remove in AL5 and read from configured recipe strategies and do the same for products
        /// </summary>
        [DataMember, Description("Recipe types that can be configured through the UI")]
        public List<SupportRecipe> SupportedRecipes { get; set; }
    }

    /// <summary>
    /// Necessary intermediate class for Platform2.7 Maintenance
    /// </summary>
    [DataContract]
    public class SupportRecipe
    {
        /// <summary>
        /// The value itself
        /// </summary>
        [DataMember, PossibleTypes(typeof(IProductRecipe))]
        public string Type { get; set; }
    }
}
