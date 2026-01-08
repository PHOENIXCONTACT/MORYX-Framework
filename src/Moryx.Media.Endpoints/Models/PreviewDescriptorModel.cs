// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Media.Endpoints.Models
{
    [DataContract]
    internal class PreviewDescriptorModel : FileDescriptorModel
    {
        /// <summary>
        /// State of the preview
        /// </summary>
        [DataMember]
        public PreviewState PreviewState { get; set; }
    }
}
