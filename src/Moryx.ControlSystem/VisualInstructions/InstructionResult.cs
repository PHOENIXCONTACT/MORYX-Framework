// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Object representing a possible result for an instructions
    /// </summary>
    public class InstructionResult
    {
        /// <summary>
        /// Key of the result. This value needs to be unique for the instruction the result is used for.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Human readable value of the result
        /// </summary>
        public string DisplayValue { get; set; }
    }
}

