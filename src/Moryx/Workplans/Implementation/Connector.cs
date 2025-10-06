// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Drawing;
using System.Runtime.Serialization;

namespace Moryx.Workplans
{
    /// <summary>
    /// Connector implementations
    /// </summary>
    [DataContract(IsReference = true)]
    public class Connector : IConnector
    {
        /// <inheritdoc/>
        [DataMember]
        public long Id { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public NodeClassification Classification { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public Point Position { get; set; }

        /// <inheritdoc/>
        public virtual IPlace CreateInstance()
        {
            return new Place
            {
                Id = Id,
                Classification = Classification
            };
        }
    }
}
