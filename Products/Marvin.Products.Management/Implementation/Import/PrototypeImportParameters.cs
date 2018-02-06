using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Base class for importing a prototype
    /// </summary>
    public class PrototypeParameters : IImportParameters
    {
        /// <summary>
        /// Identifier of the new product
        /// </summary>
        [Description("Identifier of the new product"), Required]
        [DefaultValue("2901234")]
        [StringLength(7, MinimumLength = 7), RegularExpression(@"\d+")]
        public string Identifier { get; set; }

        /// <summary>
        /// Revision of the new product
        /// </summary>
        [Description("Revision of the new product"), Required]
        public short Revision { get; set; }

        /// <summary>
        /// Optional name of the product
        /// </summary>
        public string Name { get; set; }
    }
}