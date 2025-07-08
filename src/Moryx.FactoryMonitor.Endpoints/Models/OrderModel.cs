// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.FactoryMonitor.Endpoints.Models;

namespace Moryx.FactoryMonitor.Endpoints.Model
{
    /// <summary>
    /// Initial order model to display in the UI
    /// </summary>
    [DataContract]
    public class OrderModel : OrderChangedModel
    {
        [DataMember]
        public string Color { get; set; }
    }
}

