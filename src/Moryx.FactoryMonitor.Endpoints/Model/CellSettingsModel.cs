// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Model
{
    [DataContract]
    public class CellSettingsModel
    {
        [DataMember]
        public virtual string Image { get; set; }
        [DataMember]
        public virtual string Icon { get; set; }
    }
}

