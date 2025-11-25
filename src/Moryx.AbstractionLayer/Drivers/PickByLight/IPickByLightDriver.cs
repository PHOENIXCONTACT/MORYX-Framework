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
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
        Task<InstructionConfirmation> ActivateInstructionAsync(string address, LightInstructions instruction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivate an instruction
        /// </summary>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
        Task<InstructionConfirmation> DeactivateInstructionAsync(string address, CancellationToken cancellationToken = default);

        /// <summary>
        /// Instruction was confirmed
        /// </summary>
        event EventHandler<InstructionConfirmation> InstructionConfirmed;
    }
}
