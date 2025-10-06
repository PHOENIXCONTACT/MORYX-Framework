// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Model
{
    /// <summary>
    /// Model of a resource/cell inside the factory
    /// </summary>
    [DataContract]
    [Obsolete]
    public class CellModel
    {
        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual string Identifier { get; set; }

        [DataMember]
        public virtual Dictionary<string, CellPropertySettings> CellPropertySettings { get; set; }

        [DataMember]
        public virtual CellLocationModel CellLocation { get; set; }

        [DataMember]
        public virtual string CellImageURL { get; set; }

        [DataMember]
        public virtual string CellIconName { get; set; }

        [DataMember]
        public virtual string CellName { get; set; }

        [DataMember]
        public virtual CellDataModel CellData { get; set; }

        [DataMember]
        public virtual long FactoryId { get; set; }
    }
}

