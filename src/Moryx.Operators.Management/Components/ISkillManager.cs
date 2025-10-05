// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.Operators.Skills;

namespace Moryx.Operators.Management;

internal interface ISkillManager : IPlugin
{
    IReadOnlyList<Skill> Skills { get; }

    Skill CreateSkill(SkillCreationContext skill);

    void DeleteSkill(Skill skill);

    IReadOnlyList<SkillType> SkillTypes { get; }

    SkillType CreateSkillType(SkillTypeCreationContext type);

    void UpdateSkillType(SkillType type);

    void DeleteSkillType(SkillType type);

    event EventHandler<SkillChangedEventArgs> SkillChanged;

    event EventHandler<SkillTypeChangedEventArgs> SkillTypeChanged;
}
