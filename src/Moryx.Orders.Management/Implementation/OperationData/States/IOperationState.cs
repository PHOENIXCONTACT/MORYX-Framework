// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.Orders.Management;

/// <summary>
/// Interface for the public representation of an operation state
/// </summary>
internal interface IOperationState
{
    /// <summary>
    /// Current key of the state <see cref="StateBase.Key"/>
    /// </summary>
    int Key { get; }

    /// <see cref="OperationStateClassification"/>
    OperationStateClassification Classification { get; }

    /// <summary>
    /// Flag if the operation can be assigned
    /// </summary>
    bool CanAssign { get; }

    /// <summary>
    /// Flag if the operation can begin
    /// </summary>
    bool CanBegin { get; }

    /// <summary>
    /// Flag if the target amount of the operation can be reduced
    /// </summary>
    bool CanReduceAmount { get; }

    /// <summary>
    /// Flag if the operation can be interrupted
    /// </summary>
    bool CanInterrupt { get; }

    /// <summary>
    /// Flag if the operation can be partial reported
    /// </summary>
    bool CanPartialReport { get; }

    /// <summary>
    /// Flag if the operation can be final reported
    /// </summary>
    bool CanFinalReport { get; }

    /// <summary>
    /// Flag if the operation can be adviced
    /// </summary>
    bool CanAdvice { get; }

    /// <summary>
    /// Flag if the operation was initially created
    /// </summary>
    bool IsCreated { get; }

    /// <summary>
    /// Flag if the operation creation was failed
    /// </summary>
    bool IsFailed { get; }

    /// <summary>
    /// Flag if the operation is currently creating
    /// </summary>
    bool IsAssigning { get; }

    /// <summary>
    /// Flag if the operation has reached the target amount
    /// </summary>
    bool IsAmountReached { get; }
}