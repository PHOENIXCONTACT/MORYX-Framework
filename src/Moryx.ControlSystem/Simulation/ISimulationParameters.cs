// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;

namespace Moryx.ControlSystem.Simulation;

/// <summary>
/// Optional interface for <see cref="IParameters"/> to define or bind the activities execution time
/// in combination with a <see cref="ISimulationDriver"/>
/// </summary>
public interface ISimulationParameters
{
    /// <summary>
    /// Execution time of the activity in 
    /// </summary>
    public TimeSpan ExecutionTime { get; }
}