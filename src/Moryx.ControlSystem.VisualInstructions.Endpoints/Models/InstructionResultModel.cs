// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.ControlSystem.VisualInstructions.Endpoints
{
    [DataContract]
    public class InstructionResultModel
    {
        /// <summary>
        /// Key of the result. This value needs to be unique for the instruction the result is used for.
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Human readable value of the result
        /// </summary>
        [DataMember]
        public string DisplayValue { get; set; }
    }
}

