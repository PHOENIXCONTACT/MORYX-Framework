using System.Runtime.Serialization;
using Marvin.Products.Management.Importers;

namespace Marvin.Products.Samples
{
    [DataContract]
    public class PrototypeImporterConfig : ProductImporterConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public override string PluginName => nameof(PrototypeImporter);
    }
}