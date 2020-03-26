// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Adapter for product files
    /// </summary>
    [DataContract]
    public class ProductFile
    {
        /// <summary>
        /// Original name of the file
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Type of this file
        /// </summary>
        [DataMember]
        public string MimeType { get; set; }

        /// <summary>
        /// Path or URL to the file
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }

        /// <summary>
        /// Hash for the internal file manager
        /// </summary>
        [DataMember]
        public string FileHash { get; set; }
    }
}
