// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [ResourceRegistration]
    public class Machine : Resource
    {
        [DataMember, EditorVisible]
        public string CurrentState { get; set; }
    }
}
