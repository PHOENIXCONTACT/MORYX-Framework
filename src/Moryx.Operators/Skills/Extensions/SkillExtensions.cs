// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Operators.Skills;

public static class SkillExtensions
{
    /// <summary>
    /// Returns the acquired capabilities of the skill
    /// </summary>
    /// <param name="skill">skill of the operator</param>
    /// <returns></returns>
    public static ICapabilities AcquiredCapabilities(this Skill skill) =>
        skill.Type.AcquiredCapabilities;
}