// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Moryx.Orders.Endpoints.Models
{
    [DataContract]
    public class OperationLogMessageModel
    {
        [DataMember]
        public LogLevel LogLevel { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Exception { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }
    }
}

