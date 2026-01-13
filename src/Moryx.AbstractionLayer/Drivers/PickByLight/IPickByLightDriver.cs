// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.PickByLight;

/// <summary>
/// Interface for the pick-by-light drivers
/// </summary>
public interface IPickByLightDriver : IDriver
{
    /// <summary>
    /// Sends instruction to pbl system <see cref="LightInstruction"/> <seealso cref="LightPosition"/>
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="instruction"></param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverException">Thrown when the driver encounters an error during execution.</exception>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<InstructionResult> ActivateInstructionAsync(LightInstruction instruction, IReadOnlyCollection<LightPosition> positions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the given light positions <see cref="LightPosition"/>
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverException">Thrown when the driver encounters an error during execution.</exception>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<InstructionResult> DeactivateInstructionAsync(IReadOnlyCollection<LightPosition> positions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event for messages received by the pbl system
    /// </summary>
    event EventHandler<PickByLightMessageEventArgs> MessageReceived;
}
