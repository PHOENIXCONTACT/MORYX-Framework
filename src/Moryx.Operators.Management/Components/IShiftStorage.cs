// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Operators.Skills;

namespace Moryx.Operators.Management;

internal interface ISkillStorage : IPlugin
{
    IEnumerable<SkillType> GetTypes();

    IEnumerable<Skill> GetSkills(IReadOnlyList<SkillType> types, IReadOnlyList<OperatorData> operators);

    Skill Create(SkillCreationContext context);

    void Delete(Skill skill);

    SkillType Create(SkillTypeCreationContext context);
    void Update(SkillType type);
    void Delete(SkillType type);
}
