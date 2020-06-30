// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Products.Model
{
    public class ProductFileEntity : EntityBase
    {
        public int Version { get; set; }

        /// <summary>
        /// Original name of the file
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Type of this file
        /// </summary>
        public virtual string MimeType { get; set; }

        /// <summary>
        /// Path or URL to the file
        /// </summary>
        public virtual string FilePath { get; set; }

        /// <summary>
        /// Hash for the internal file manager
        /// </summary>
        public virtual string FileHash { get; set; }

        /// <summary>
        /// Type that references this file
        /// </summary>
        public virtual ProductTypeEntity Product { get; set; }
    }
}
