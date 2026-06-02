// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Skills;

namespace Moryx.Operators.Attendances;

public interface IOperatorAssignable : IResource
{
    /// <summary>
    /// Skill required for an operator to work on this machine
    /// </summary>
    ICapabilities RequiredSkills { get; }

    /// <summary>
    /// Inform the assignable about the currently assigned operators and their skills
    /// </summary>
    /// <param name="attandances"></param>
    void AttendanceChanged(IReadOnlyList<AttendanceData> attandances);
}

public record AttendanceData(Operator Operator, IReadOnlyList<Skill> Skills);
