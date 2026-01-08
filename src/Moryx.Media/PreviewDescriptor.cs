// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media
{
    /// <summary>
    /// Descriptor for an preview
    /// </summary>
    [DataContract]
    public sealed class PreviewDescriptor : FileDescriptor
    {
        /// <summary>
        /// Current state of the preview
        /// </summary>
        [DataMember]
        public PreviewState State { get; set; }
    }
}
