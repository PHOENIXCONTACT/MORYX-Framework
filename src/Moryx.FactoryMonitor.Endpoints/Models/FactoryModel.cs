// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// Navigation model contains information for Navigating back to the parent ManufacturingFactory VIEW
    /// </summary>
    [DataContract]
    public class FactoryModel
    {
        /// <summary>
        /// Parent ID. Allow the view to Navigate to this parent
        /// </summary>
        [DataMember]
        public long ParentId { get; set; }

        /// <summary>
        /// Current Factory ID
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public virtual string BackgroundURL { get; set; }

        [DataMember]
        public virtual string Title { get; set; }
    }
}

