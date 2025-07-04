﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.PickByLight
{
    /// <summary>
    /// Output structure for a pick by light driver
    /// </summary>
    public struct LightInstructions
    {
        /// <summary>
        /// Activate light instructions
        /// </summary>
        public LightInstructions(int color, string instruction)
        {
            Active = true;
            Color = color;
            Instruction = instruction;
        }

        /// <summary>
        /// Instruction active
        /// </summary>
        public bool Active { get; }

        /// <summary>
        /// RGB color
        /// </summary>
        public int Color { get; }

        /// <summary>
        /// Instruction text
        /// </summary>
        public string Instruction { get; set; }
    }
}
