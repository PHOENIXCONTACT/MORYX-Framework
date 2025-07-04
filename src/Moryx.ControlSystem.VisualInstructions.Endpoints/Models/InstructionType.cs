﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.VisualInstructions.Endpoints
{
    /// <summary>
    /// Process related with the instructions
    /// </summary>
    public enum InstructionType
    {
        /// <summary>
        /// Only display instructions
        /// </summary>
        Display,

        /// <summary>
        /// Execute complete process
        /// </summary>
        Execute,
    }
}
