// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Operators.Management.Model;
using Moryx.Operators.Skills;
using Moryx.Tools;
using Newtonsoft.Json;

namespace Moryx.Operators.Management;

[Component(LifeCycle.Singleton, typeof(ISkillStorage))]
internal class SkillStorage : ISkillStorage, ILoggingComponent
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public IModuleLogger Logger { get; set; }

    public IUnitOfWorkFactory<OperatorsContext> UnitOfWorkFactory { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Dictionary<string, Func<ICapabilities>> _capabilityConstructors;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    #region Lifecylce

    public void Start()
    {
        Logger.LogInformation("Starting {storage}. Register types of {capabilties}...", nameof(SkillStorage), nameof(ICapabilities));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        _capabilityConstructors = ReflectionTool.GetPublicClasses(typeof(ICapabilities))
            .Select(c =>
            {
                // Abstract classes and those without parameterles ctor will throw an exception
                if (!c.IsAbstract && c.GetConstructor(Type.EmptyTypes) is not null)
                    return c;

                Logger.LogInformation("Skipping {type}, because it is abstract or does not provide an empty constructor.", c.Name);
                return null;
            })
            .Where(c => c is not null)
            .ToDictionary(ct => ct.FullName ?? ct.ToString(), cc => ReflectionTool.ConstructorDelegate<ICapabilities>(cc));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        Logger.LogInformation("Registered {count} types of {capabilties}...", _capabilityConstructors.Count, nameof(ICapabilities));
    }

    public void Stop()
    {
    }

    #endregion

    #region ISkillStorage

    public IEnumerable<Skill> GetSkills(IReadOnlyList<SkillType> types, IReadOnlyList<OperatorData> operators)
    {
        using var uow = UnitOfWorkFactory.Create();
        var repo = uow.GetRepository<ISkillRepository>();
        var skillEntities = repo.Linq.Active()
            .AsEnumerable() //because of query translation error 
            .Where(se => se.Expiration >= DateOnly.FromDateTime(DateTime.UtcNow))
            .ToList();

        var skills = new List<Skill>(skillEntities.Count);
        foreach (var skillEntity in skillEntities)
        {
            var type = types.FirstOrDefault(t => t.Id == skillEntity.SkillTypeId);
            var operatorData = operators.FirstOrDefault(o => o.Identifier == skillEntity.OperatorIdentifier);

            if (type is null)
            {
                Logger.LogError("Skill -Id: {SkillId}- is referencing a SkillType Id: {SkillTypeId}- that was not loaded into memory.", skillEntity.Id, skillEntity.SkillTypeId);
                continue;
            }
            if(operatorData is null)
            {
                Logger.LogError("Skill -Id: {SkillId}- is referencing an Operator Identifier: {OperatorIdentifier}- that was not loaded into memory.", skillEntity.Id, skillEntity.OperatorIdentifier);
                continue;
            }

            var skill = new Skill(type, operatorData.Operator)
            {
                Id = skillEntity.Id,
                ObtainedOn = skillEntity.ObtainedOn

            };
            skills.Add(skill);
        }

        return skills;
    }

    public Skill Create(SkillCreationContext context)
    {
        using var uow = UnitOfWorkFactory.Create();
        var repo = uow.GetRepository<ISkillRepository>();

        var entity = repo.Create();
        entity.SkillTypeId = context.Type.Id;
        entity.OperatorIdentifier = context.Operator.Identifier;

        entity.ObtainedOn = context.ObtainedOn;
        entity.Expiration = context.ObtainedOn.AddDays(context.Type.Duration.Days);

        uow.SaveChanges();

        return new Skill(context.Type, context.Operator)
        {
            Id=entity.Id,
            ObtainedOn = entity.ObtainedOn
        };
    }

    public void Delete(Skill Skill)
    {
        using var uow = UnitOfWorkFactory.Create();
        var repo = uow.GetRepository<ISkillRepository>();
        var entity = repo.GetByKey(Skill.Id);
        repo.Remove(entity);
        uow.SaveChanges();
    }

    public IEnumerable<SkillType> GetTypes()
    {
        using var uow = UnitOfWorkFactory.Create();
        var repo = uow.GetRepository<ISkillTypeRepository>();

        // Load skill entities
        var skillTypeEntities = repo.Linq.Active().ToList();
        Logger.LogDebug("Retrieved {count} {name}s from the database.", skillTypeEntities.Count, nameof(SkillType));

        // Load skill object
        var skillTypes = new List<SkillType>(skillTypeEntities.Count);
        foreach (var typeEntity in skillTypeEntities)
        {
            var skillType = new SkillType(typeEntity.Name, DeserializeCapabilities(typeEntity))
            {
                Id = typeEntity.Id,
                Duration = typeEntity.Duration
            };

            skillTypes.Add(skillType);
        }

        return skillTypes;
    }

    private ICapabilities DeserializeCapabilities(SkillTypeEntity typeEntity)
    {
        var capabilityContructor = _capabilityConstructors.GetValueOrDefault(typeEntity.CapabilitiesType);
        if (capabilityContructor is null)
        {
            Logger.LogError("Capability of type {type} could not be constructed. No eligable constructor could be found.", typeEntity.CapabilitiesType);
            return new FaultyCapabilities() { OriginalType = typeEntity.CapabilitiesType };
        }

        var capabilityType = capabilityContructor().GetType();
        if (JsonConvert.DeserializeObject(typeEntity.CapabilitiesData, capabilityType) is ICapabilities capabilityData)
            return capabilityData;

        Logger.LogError("Capability of type {type} could not be constructed. Deserialization of capability failed.", typeEntity.CapabilitiesType);
        return new FaultyCapabilities() { OriginalType = typeEntity.CapabilitiesType };
    }

    public SkillType Create(SkillTypeCreationContext context)
    {
        using var uow = UnitOfWorkFactory.Create();
        var repo = uow.GetRepository<ISkillTypeRepository>();

        var typeEntity = repo.Create(context.Name, context.Duration);
        SerializeCapabilities(typeEntity, context.AcquiredCapabilities);
        uow.SaveChanges();

        return new SkillType(context.Name, context.AcquiredCapabilities)
        {
            Id = typeEntity.Id,
            Duration = context.Duration
        };
    }

    public void Update(SkillType type)
    {
        using var uow = UnitOfWorkFactory.Create();
        var repo = uow.GetRepository<ISkillTypeRepository>();

        var typeEntity = repo.GetByKey(type.Id);
        typeEntity.Name = type.Name;
        typeEntity.Duration = type.Duration;
        SerializeCapabilities(typeEntity, type.AcquiredCapabilities);

        uow.SaveChanges();
    }

    private static void SerializeCapabilities(SkillTypeEntity typeEntity, ICapabilities capabilities)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        typeEntity.CapabilitiesType = capabilities.GetType().FullName;
#pragma warning restore CS8601 // Possible null reference assignment.
        typeEntity.CapabilitiesData = JsonConvert.SerializeObject(capabilities);
    }

    public void Delete(SkillType type)
    {
        using var uow = UnitOfWorkFactory.Create();
        var skillsRepo = uow.GetRepository<ISkillRepository>();
        skillsRepo.Linq.Where(s => s.SkillTypeId == type.Id)
            .ForEach(e => skillsRepo.Remove(e));

        var repo = uow.GetRepository<ISkillTypeRepository>();
        var entity = repo.GetByKey(type.Id);
        repo.Remove(entity);
        uow.SaveChanges();
    }    

    #endregion
}
