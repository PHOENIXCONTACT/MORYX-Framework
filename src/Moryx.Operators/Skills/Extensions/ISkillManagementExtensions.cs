// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Operators.Skills;

public static class ISkillManagementExtensions
{
    extension(ISkillManagement source)
    {
        /// <summary>
        /// Returns the skill with the given id.
        /// </summary>
        /// <param name="id">Id of the skill</param>
        public Skill? GetSkill(long id) =>
            source.Skills.SingleOrDefault(s => s.Id == id);

        /// <summary>
        /// Returns the all skills of an operator
        /// </summary>
        /// <param name="@operator">Operator to retrieve the skills for</param>
        public IEnumerable<Skill> GetSkills(Operator @operator) =>
            source.Skills.Where(s => s.Operator.Identifier == @operator.Identifier);

        /// <summary>
        /// Returns the acquired capabilities of an operator
        /// </summary>
        /// <param name="@operator">Operator to retrieve the acquired capabilities for</param>
        public ICapabilities GetAcquiredCapabilities(Operator @operator) =>
            new CombinedCapabilities(GetSkills(source, @operator)
                .Select(x => x.AcquiredCapabilities()).ToList());

        /// <summary>
        /// Returns the skill type with the given id.
        /// </summary>
        /// <param name="id">Id of the skill type</param>
        public SkillType? GetSkillType(long id) =>
            source.SkillTypes.SingleOrDefault(s => s.Id == id);
    }
}
