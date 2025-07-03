using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Orders.Assignment;
using Moryx.Runtime.Configuration;
using Moryx.Serialization;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Configuration of the document provider
    /// </summary>
    [DataContract]
    public class DocumentsConfig
    {
        /// <summary>
        /// Folder path to save the loaded documents
        /// </summary>
        [DataMember, DefaultValue(".\\Backups\\Orders")]
        [Description("Folder path to save the loaded documents")]
        public string DocumentsPath { get; set; }

        /// <summary>
        /// Configuration of the document loader
        /// </summary>
        [DataMember]
        [ModuleStrategy(typeof(IDocumentLoader))]
        [PluginConfigs(typeof(IDocumentLoader))]
        public DocumentLoaderConfig DocumentLoader { get; set; }
    }
}