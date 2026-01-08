// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs;

/// <summary>
/// Estimation by the <see cref="IJobManagement"/> how much additional effort
/// is required for a certain job
/// </summary>
public class JobEvaluation
{
    /// <summary>
    /// Possible errors in the workplan
    /// </summary>
    public IReadOnlyList<string> WorkplanErrors { get; set; }
}