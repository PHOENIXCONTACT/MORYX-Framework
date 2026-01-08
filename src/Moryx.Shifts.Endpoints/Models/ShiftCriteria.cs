// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Shifts.Endpoints.Models
{
    [DataContract]
    public class ShiftCriteria
    {
        [DataMember]
        public DateOnly? EarliestDate { get; set; }

        [DataMember]
        public DateOnly? LatestDate { get; set; }

    }
}

