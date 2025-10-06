// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Factory
{
    /// <summary>
    /// Position of a resource/machine
    /// </summary>
    [DataContract]
    public class Position
    {
        [DataMember]
        public double PositionX { get; set; }

        [DataMember]
        public double PositionY { get; set; }
    }
}