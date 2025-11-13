// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.Operators.Extensions;
using Moryx.Operators.Skills;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Operators.Endpoints;

internal static class Converter
{
    private static readonly Dictionary<Type, Entry> _capabilitiyPrototypes;

    static Converter()
    {
        _capabilitiyPrototypes = ReflectionTool.GetPublicClasses(typeof(ICapabilities))
            // Abstract classes and those without parameterles ctor will throw an exception
            .Where(c => !c.IsAbstract && c.GetConstructor(Type.EmptyTypes) is not null)
            .ToDictionary(t => t, t => EntryConvert.Prototype(new EntryPrototype(t.Name, Activator.CreateInstance(t))));
    }

    internal static SkillTypeCreationContext ToContext(this SkillTypeCreationContextModel model)
        => new(VerifyNotNull(model.Name), CreateInstance(VerifyNotNull(model.Capabilities)))
        {
            Duration = model.Duration,
        };

    internal static SkillCreationContext ToContext(this SkillCreationContextModel model,
        IOperatorManagement operators,
        ISkillManagement skills) =>
        new(operators.GetOperator(VerifyNotNull(model.OperatorIdentifier)) ?? throw new KeyNotFoundException($"No operator with identifier {model.OperatorIdentifier} could be found."),
            skills.GetSkillType(model.TypeId) ?? throw new KeyNotFoundException($"No skill with id {model.OperatorIdentifier} could be found."))
        { ObtainedOn = model.ObtainedOn };

    internal static ExtendedOperatorModel ToModel(this AssignableOperator @operator) => new()
    {
        FirstName = @operator.FirstName,
        LastName = @operator.LastName,
        Pseudonym = @operator.Pseudonym,
        Identifier = @operator.Identifier,
        AssignedResources = @operator.AssignedResources.Select(ToModel)
    };

    internal static SkillTypeModel ToModel(this SkillType type, bool full)
    {
        var model = new SkillTypeModel { Id = type.Id, Name = type.Name, Duration = type.Duration };
        var capabilitySerialize = EntryConvert.EncodeObject(type.AcquiredCapabilities);
        model.Capabilities = capabilitySerialize;

        if (!full) return model;

        // Only serialize prototypes for detail view
        model.Capabilities.Prototypes = [.. _capabilitiyPrototypes.Values];
        model.Capabilities.Value.Possible = [.. _capabilitiyPrototypes.Keys.Select(t => t.Name)];
        return model;
    }

    internal static AttendableResourceModel ToModel(IOperatorAssignable resource) => new()
    {
        Id = resource.Id,
        Name = resource.Name
    };

    internal static SkillModel ToModel(this Skill skill) => new()
    {
        Id = skill.Id,
        TypeId = skill.Type.Id,
        OperatorIdentifier = skill.Operator.Identifier,
        ObtainedOn = skill.ObtainedOn,
        IsExpired = skill.IsExpired,
        ExpiresOn = skill.ObtainedOn.AddDays((int)skill.Type.Duration.TotalDays)
    };

    internal static Operator ToType(this OperatorModel @operator) => new(VerifyNotNull(@operator.Identifier))
    {
        FirstName = @operator.FirstName,
        LastName = @operator.LastName,
        Pseudonym = @operator.Pseudonym,
    };

    internal static SkillType ToType(this SkillTypeModel model) => new(VerifyNotNull(model.Name), CreateInstance(VerifyNotNull(model.Capabilities)))
    {
        Id = model.Id,
        Duration = model.Duration,
    };

    private static ICapabilities CreateInstance(Entry capabilityEntry)
    {
        var type = _capabilitiyPrototypes.Keys.FirstOrDefault(type => type.Name == capabilityEntry.Value.Current);
        try
        {
            return (ICapabilities)EntryConvert.CreateInstance(type, capabilityEntry);
        }
        catch (Exception e)
        {
            return new FaultyCapabilities()
            {
                OriginalType = capabilityEntry.Value.Current,
                Exception = e.ToString()
            };
        }

    }

    private static Entry VerifyNotNull(Entry? entry) => entry ?? throw new ArgumentNullException(nameof(entry));

    private static string VerifyNotNull(string? name) => name ?? throw new ArgumentNullException(nameof(name));

}

