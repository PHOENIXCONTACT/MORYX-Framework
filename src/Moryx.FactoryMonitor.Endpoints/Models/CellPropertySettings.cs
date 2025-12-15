// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// Setting for a displayable property of a cell
    /// </summary>
    [DataContract]
    public class CellPropertySettings
    {
        [DataMember]
        public virtual string ValueUnit { get; set; }
        [DataMember]
        public virtual string IconName { get; set; }
        [DataMember]
        public virtual bool IsDisplayed { get; set; }
        [DataMember]
        public virtual object CurrentValue { get; set; }
    }
}

