// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media
{
    /// <summary>
    /// Descriptor combining different variants for a content GUID
    /// </summary>
    [DataContract]
    public sealed class ContentDescriptor
    {
        /// <summary>
        /// Unique Id of this content
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Hash for the JSON file storing this <see cref="ContentDescriptor"/>
        /// </summary>
        public string FileHash { get; set; }

        /// <summary>
        /// Original file name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Variants of this content
        /// </summary>
        [DataMember]
        public List<VariantDescriptor> Variants { get; set; } = new List<VariantDescriptor>();

        /// <summary>
        /// Create a new content descriptor from guid
        /// </summary>
        public ContentDescriptor(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Retrieve the master variant
        /// </summary>
        /// <returns></returns>
        public VariantDescriptor GetMaster() => GetVariant(MediaConstants.MasterName);

        /// <summary>
        /// Get a certain variant
        /// </summary>
        /// <param name="variantName"></param>
        /// <returns></returns>
        public VariantDescriptor GetVariant(string variantName) => Variants.FirstOrDefault(v => v.Name == variantName);
    }
}

