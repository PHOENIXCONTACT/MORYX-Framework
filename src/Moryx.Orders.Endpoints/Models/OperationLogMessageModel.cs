// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints
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

