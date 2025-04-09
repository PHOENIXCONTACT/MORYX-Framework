// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.PickByLight
{
    /// <summary>
    /// Instruction was confirmed
    /// </summary>
    public class InstructionConfirmation : EventArgs
    {
        /// <summary>
        /// Address of the confirmation
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Return code for the confirmation
        /// </summary>
        public int ReturnCode { get; set; }
    }
}
