// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Base class for importing a prototype
    /// </summary>
    public class PrototypeParameters
    {
        /// <summary>
        /// Identifier of the new product
        /// </summary>
        [DefaultValue("2901234")]
        [Description("Identifier of the new product"), Required]
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
