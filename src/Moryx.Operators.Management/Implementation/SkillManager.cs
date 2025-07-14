// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Moryx.Operators.Skills;
using Microsoft.Extensions.Logging;
using Moryx.Tools;
using Moryx.Logging;

[assembly: InternalsVisibleTo("Moryx.Operators.Management.IntegrationTests")]

namespace Moryx.Operators.Management;

[Component(LifeCycle.Singleton, typeof(ISkillManager))]
internal class SkillManager : ISkillManager
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ISkillStorage Storage { get; set; }

    public IModuleLogger Logger { get; set; }

    public IOperatorManager Operators { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    #endregion

    #region ISkillManager
    private readonly List<Skill> _skills = [];
    public IReadOnlyList<Skill> Skills => _skills;

    private readonly List<SkillType> _types = [];
    public IReadOnlyList<SkillType> SkillTypes => _types;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public event EventHandler<SkillChangedEventArgs> SkillChanged;
    public event EventHandler<SkillTypeChangedEventArgs> SkillTypeChanged;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Skill CreateSkill(SkillCreationContext context)
    {
        Logger.Log(LogLevel.Debug, "Creating {Skill} from {context}", nameof(Skill), nameof(SkillCreationContext));
        var skill = Storage.Create(context);
        _skills.Add(skill);
        SkillChanged.Invoke(this, new SkillChangedEventArgs(SkillChange.Creation, skill));
        Logger.Log(LogLevel.Debug, "Created {Skill} -Id: {id}- from {context}", nameof(skill), skill.Id, nameof(SkillCreationContext));
        return skill;
    }

    public SkillType CreateSkillType(SkillTypeCreationContext context)
    {
        Logger.Log(LogLevel.Debug, "Creating {type} from {context}", nameof(SkillType), nameof(SkillTypeCreationContext));
        var type = Storage.Create(context);
        _types.Add(type);
        SkillTypeChanged.Invoke(this, new SkillTypeChangedEventArgs(SkillTypeChange.Creation, type));
        Logger.Log(LogLevel.Debug, "Created {type} -Id: {id}- from {context}", nameof(SkillType), type.Id, nameof(SkillTypeCreationContext));
        return type;
    }

    public void DeleteSkill(Skill skill)
    {
        Storage.Delete(skill);
        _skills.Remove(skill);
        SkillChanged.Invoke(this, new SkillChangedEventArgs(SkillChange.Deletion, skill));
        Logger.Log(LogLevel.Debug, "Deleted {Skill} -Id: {id}-.", nameof(skill), skill.Id);
    }

    public void DeleteSkillType(SkillType type)
    {
        var referenceingSkills = _skills.Where(s => s.Type == type).ToArray();
        Logger.Log(LogLevel.Debug, "Deleting {type} -Id: {id}- and all {count} skills that referenced it",
            nameof(SkillType), type.Id, referenceingSkills.Length);
        Storage.Delete(type);
        foreach (var Skill in referenceingSkills)
        {
            _skills.Remove(Skill);
            SkillChanged.Invoke(this, new SkillChangedEventArgs(SkillChange.Deletion, Skill));
        }

        _types.Remove(type);
        SkillTypeChanged.Invoke(this, new SkillTypeChangedEventArgs(SkillTypeChange.Deletion, type));
        Logger.Log(LogLevel.Debug, "Deleted {type} -Id: {id}- and all {count} Skills that referenced it",
            nameof(SkillType), type.Id, referenceingSkills.Length);
    }

    public void UpdateSkillType(SkillType type)
    {
        Logger.Log(LogLevel.Debug, "Updating {type} -Id: {id}-", nameof(SkillType), type.Id);
        Storage.Update(type);
        _types.ReplaceItem(t => t.Id == type.Id, type);
        SkillTypeChanged.Invoke(this, new SkillTypeChangedEventArgs(SkillTypeChange.Update, type));
        Logger.Log(LogLevel.Debug, "Updated {type} -Id: {id}-", nameof(SkillType), type.Id);
    }

    #endregion

    #region IPlugin

    public void Start()
    {
        _types.AddRange(Storage.GetTypes());
        _skills.AddRange(Storage.GetSkills(_types, Operators.Operators));
    }

    public void Stop()
    {
        _skills.Clear();
        _types.Clear();
    }

    #endregion
}
