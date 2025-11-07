// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.PickByLight
{
    /// <summary>
    /// Interface for the pick by light driver
    /// </summary>
    public interface IPickByLightDriver : IDriver
    {
        /// <summary>
        /// Activate instruction for this address
        /// </summary>
        /// <exception cref="DriverStateException">Will be thrown when the driver is in wrong state</exception>
        Task ActivateInstruction(string address, LightInstructions instruction);

        /// <summary>
        /// Deactivate an instruction
        /// </summary>
        /// <exception cref="DriverStateException">Will be thrown when the driver is in wrong state</exception>
        Task DeactivateInstruction(string address);

        /// <summary>
        /// Instruction was confirmed
        /// </summary>
        event EventHandler<InstructionConfirmation> InstructionConfirmed;
    }
}
