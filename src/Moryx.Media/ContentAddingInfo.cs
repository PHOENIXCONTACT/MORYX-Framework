// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Media
{
    /// <summary>
    /// Result of an adding request
    /// </summary>
    public enum ContentAddingResult
    {
        /// <summary>
        /// Content was added
        /// </summary>
        Ok,

        /// <summary>
        /// Content already exists
        /// </summary>
        AlreadyExists,

        /// <summary>
        /// Given master was not found
        /// </summary>
        MasterNotFound,

        /// <summary>
        /// A variant with a reserved name was given
        /// </summary>
        VariantNameReserved
    }

    /// <summary>
    /// Encapsulates information which will be generated while adding content to the media server.
    /// </summary>
    public class ContentAddingInfo
    {
        /// <summary>
        /// <see cref="ContentDescriptor"/>
        /// </summary>
        public ContentDescriptor Descriptor { get; set; }

        /// <summary>
        /// <see cref="VariantDescriptor"/>
        /// </summary>
        public VariantDescriptor Variant { get; set; }

        /// <summary>
        /// <see cref="ContentAddingResult"/>
        /// </summary>
        public ContentAddingResult Result { get; set; }
    }
}

