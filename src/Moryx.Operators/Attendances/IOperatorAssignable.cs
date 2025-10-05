// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Skills;

namespace Moryx.Operators;

public interface IOperatorAssignable: IResource
{
    /// <summary>
    /// Skill required for an operator to work on this machine
    /// </summary>
    ICapabilities RequiredSkills { get; }

    void AttendanceChanged(IReadOnlyList<AttendanceChangedArgs> attandances);
}

public record AttendanceChangedArgs(Operator Operator, IReadOnlyList<Skill> Skills);
