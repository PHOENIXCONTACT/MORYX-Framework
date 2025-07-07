// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer;

namespace Moryx.Benchmarking
{
    [DataContract]
    public class BenchmarkTracing : Tracing
    {
        [DataMember]
        [DisplayName("Runtime"), Description("Processing time of the activity")]
        public long RuntimeMs { get; set; }
    }
}
