// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Immutable;
using Moryx.AbstractionLayer.Resources;
using Moryx.Operators.Attendances;
using Moryx.Operators.Exceptions;
using Moryx.Operators.Management.Properties;
using Moryx.Operators.Skills;
using Moryx.Runtime.Modules;
using Moryx.Threading;
using Moryx.Users;

namespace Moryx.Operators.Management;

internal class OperatorManagementFacade : FacadeBase, IOperatorManagement, IAttendanceManagement, ISkillManagement, IUserManagement
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public IOperatorManager OperatorManager { get; set; }

    public IAttendanceManager AttendanceManager { get; set; }

    public ISkillManager SkillManager { get; set; }

    public IParallelOperations ParallelOperations { get; set; }

    public IResourceManagement ResourceManagement { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    #region IOperatorManagement

    public IReadOnlyList<Operator> Operators
    {
        get
        {
            ValidateHealthState();
            return OperatorManager.Operators.Select(o => o.Operator).ToImmutableArray();
        }
    }

    public void AddOperator(Operator @operator)
    {
        OperatorManager.Add(VerifiedNew(@operator));
    }

    private Operator VerifiedNew(Operator @operator)
    {
        ArgumentNullException.ThrowIfNull(@operator);
        if (string.IsNullOrEmpty(@operator.Identifier))
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_NotNullExceptionMessage, nameof(Operator.Identifier)));
        if (Operators.Any(o => o.Identifier == @operator.Identifier))
            throw new AlreadyExistsException(@operator.Identifier);
        return @operator;
    }

    public void DeleteOperator(string identifier)
    {
        OperatorManager.Delete(VerifiedExisting(identifier));
    }

    private string VerifiedExisting(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_NotNullExceptionMessage, nameof(Operator.Identifier)));
        if (!Operators.Any(o => o.Identifier == identifier))
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(Operator), identifier));
        return identifier;
    }

    public void UpdateOperator(Operator @operator)
        => OperatorManager.Update(VerifiedKnown(@operator));

    private Operator VerifiedKnown(Operator @operator) => Operators.Any(o => o.Identifier == @operator.Identifier) ? @operator :
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(Operator), @operator.Identifier));

    public event EventHandler<OperatorChangedEventArgs>? OperatorChanged;

    #endregion

    #region IAttandanceManagement

    IReadOnlyList<AssignableOperator> IAttendanceManagement.Operators
    {
        get
        {
            ValidateHealthState();
            return OperatorManager.Operators.Select(o => o.Operator).ToArray();
        }
    }

    public AssignableOperator? DefaultOperator
    {
        get
        {
            ValidateHealthState();
            return AttendanceManager.DefaultOperator?.Operator;
        }
    }

    public void SignIn(AssignableOperator @operator, IOperatorAssignable resource)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(@operator);
        ArgumentNullException.ThrowIfNull(resource);
        AttendanceManager.SignIn(VerifiedExisting(@operator), resource);
    }

    private OperatorData VerifiedExisting(AssignableOperator @operator)
        => OperatorManager.Operators.SingleOrDefault(o => o.Identifier == @operator.Identifier) ??
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(Operator), @operator.Identifier));

    public void SignOut(AssignableOperator @operator, IOperatorAssignable resource)
    {
        ValidateHealthState();
        AttendanceManager.SignOut(VerifiedExisting(@operator), resource);
    }

    public event EventHandler<AssignableOperator>? OperatorSignedIn;

    public event EventHandler<AssignableOperator>? OperatorSignedOut;

    #endregion

    #region ISkillManagement

    public IReadOnlyList<Skill> Skills
    {
        get
        {
            ValidateHealthState();
            return SkillManager.Skills;
        }
    }

    public IReadOnlyList<SkillType> SkillTypes
    {
        get
        {
            ValidateHealthState();
            return SkillManager.SkillTypes;
        }
    }

    public Skill CreateSkill(SkillCreationContext skill)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(skill);
        return SkillManager.CreateSkill(Verified(skill));
    }

    private SkillCreationContext Verified(SkillCreationContext context)
    {
        context.Type = SkillTypes.SingleOrDefault(t => t.Id == context.Type?.Id) ??
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(SkillType), context.Type.Id));
        return context;
    }

    public void DeleteSkill(long id)
    {
        ValidateHealthState();
        SkillManager.DeleteSkill(VerifiedSkill(id));
    }

    private Skill VerifiedSkill(long id) => SkillManager.Skills.SingleOrDefault(s => s.Id == id) ??
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(Skill), id));

    public SkillType CreateSkillType(SkillTypeCreationContext context)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(context);
        return SkillManager.CreateSkillType(context);
    }

    public void UpdateSkillType(SkillType type)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(type);
        SkillManager.UpdateSkillType(Verified(type));
    }

    private SkillType Verified(SkillType type)
    {
        if (SkillTypes.SingleOrDefault(t => t.Id == type.Id) is null)
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(SkillType), type.Id));
        return type;
    }

    public void DeleteSkillType(long id)
    {
        ValidateHealthState();
        SkillManager.DeleteSkillType(VerifiedType(id));
    }

    private SkillType VerifiedType(long id) => SkillManager.SkillTypes.SingleOrDefault(t => t.Id == id) ??
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(SkillType), id));

    public event EventHandler<SkillChangedEventArgs>? SkillChanged;

    public event EventHandler<SkillTypeChangedEventArgs>? SkillTypeChanged;

    #endregion

    #region IUserManagement

    /// <inheritdoc/>
    public IReadOnlyList<User> Users
    {
        get
        {
            ValidateHealthState();
            return OperatorManager.Operators.Select(o => o.User).ToImmutableArray();
        }
    }

    /// <inheritdoc/>
    public User? DefaultUser
    {
        get
        {
            ValidateHealthState();
            return AttendanceManager.DefaultOperator?.User;
        }
    }

    /// <inheritdoc/>
    public void SignIn(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        AttendanceManager.SignIn(VerifiedExisting(user), UserResource.Instance);
    }

    private OperatorData VerifiedExisting(User user)
        => OperatorManager.Operators.SingleOrDefault(o => o.Identifier == user.Identifier) ??
            throw new ArgumentException(string.Format(Strings.OperatorManagementFacade_ReferenceNotFoundExceptionMessage, nameof(Operator), user.Identifier));

    /// <inheritdoc/>
    public void SignOut(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        AttendanceManager.SignOut(VerifiedExisting(user), UserResource.Instance);
    }

    /// <inheritdoc/>
    public User? GetUser(string identifier)
        => GetUser(identifier, false);

    /// <inheritdoc/>
    public User? GetUser(string identifier, bool fallbackDefault)
    {
        ValidateHealthState();
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        return OperatorManager.Operators.SingleOrDefault(o => o.Identifier == identifier)?.User ?? (fallbackDefault ? DefaultUser : null);
    }

    /// <inheritdoc/>
    public event EventHandler<User>? UserSignedIn;

    /// <inheritdoc/>
    public event EventHandler<User>? UserSignedOut;

    #endregion

    #region FacadeBase

    public override void Activate()
    {
        base.Activate();

        AttendanceManager.OperatorSignedIn += ParallelOperations.DecoupleListener<OperatorData>(OnOperatorSignedIn);
        AttendanceManager.OperatorSignedOut += ParallelOperations.DecoupleListener<OperatorData>(OnOperatorSignedOut);
        OperatorManager.OperatorChanged += ParallelOperations.DecoupleListener<OperatorChangedEventArgs>(OnOperatorChanged);
        SkillManager.SkillChanged += ParallelOperations.DecoupleListener<SkillChangedEventArgs>(OnSkillChanged);
        SkillManager.SkillTypeChanged += ParallelOperations.DecoupleListener<SkillTypeChangedEventArgs>(OnSkillTypeChanged);
    }

    private void OnSkillTypeChanged(object? sender, SkillTypeChangedEventArgs e) => SkillTypeChanged?.Invoke(sender, e);

    private void OnSkillChanged(object? sender, SkillChangedEventArgs e) => SkillChanged?.Invoke(this, e);

    private void OnOperatorSignedIn(object? sender, OperatorData operatorData)
    {
        OperatorSignedIn?.Invoke(this, operatorData.Operator);
        UserSignedIn?.Invoke(this, operatorData.User);
    }

    private void OnOperatorSignedOut(object? sender, OperatorData operatorData)
    {
        OperatorSignedOut?.Invoke(this, operatorData.Operator);
        UserSignedOut?.Invoke(this, operatorData.User);
    }

    private void OnOperatorChanged(object? sender, OperatorChangedEventArgs eventArgs) => OperatorChanged?.Invoke(this, eventArgs);

    public override void Deactivate()
    {
        AttendanceManager.OperatorSignedIn -= ParallelOperations.RemoveListener<OperatorData>(OnOperatorSignedIn);
        AttendanceManager.OperatorSignedOut -= ParallelOperations.RemoveListener<OperatorData>(OnOperatorSignedOut);
        OperatorManager.OperatorChanged -= ParallelOperations.RemoveListener<OperatorChangedEventArgs>(OnOperatorChanged);
        SkillManager.SkillChanged -= ParallelOperations.RemoveListener<SkillChangedEventArgs>(OnSkillChanged);
        SkillManager.SkillTypeChanged -= ParallelOperations.RemoveListener<SkillTypeChangedEventArgs>(OnSkillTypeChanged);

        base.Deactivate();
    }

    #endregion
}

