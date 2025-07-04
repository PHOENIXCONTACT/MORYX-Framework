﻿// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Response of the client for an <see cref="ActiveInstruction"/>
    /// </summary>
    public class ActiveInstructionResponse
    {
        /// <summary>
        /// Runtime unique identifier of this instruction per client-identifier
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Optional inputs provided by the user
        /// </summary>
        public object Inputs { get; set; }

        /// <summary>
        /// Selected result option by the user
        /// </summary>
        public InstructionResult SelectedResult { get; set; }
    }
}
