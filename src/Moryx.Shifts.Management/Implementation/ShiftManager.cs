// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Tools;
using System.Runtime.CompilerServices;
using Moryx.Shifts.Management.Model;

[assembly: InternalsVisibleTo("Moryx.Shift.Management.IntegrationTests")]

namespace Moryx.Shifts.Management;

[Component(LifeCycle.Singleton, typeof(IShiftManager))]
internal class ShiftManager : IShiftManager, ILoggingComponent
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public IShiftStorage Storage { get; set; }

    public IModuleLogger Logger { get; set; }

    public ModuleConfig ModuleConfig { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    #region Lifecycle

    public void Start()
    {
        _types.AddRange(Storage.GetTypes());
        _shifts.AddRange(Storage.GetShifts(_types));
        _assignements.AddRange(Storage.GetAssignements(_shifts));
    }

    public void Stop()
    {
        _shifts.Clear();
        _types.Clear();
        _assignements.Clear();
    }

    #endregion

    #region IShiftManager
    private readonly List<Shift> _shifts = new();
    public IReadOnlyList<Shift> Shifts => _shifts;

    private readonly List<ShiftType> _types = new();
    public IReadOnlyList<ShiftType> ShiftTypes => _types;

    private readonly List<ShiftAssignement> _assignements = new();
    public IReadOnlyList<ShiftAssignement> ShiftAssignements => _assignements;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public event EventHandler<ShiftsChangedEventArgs> ShiftsChanged;
    public event EventHandler<ShiftTypesChangedEventArgs> ShiftTypesChanged;
    public event EventHandler<ShiftAssignementsChangedEventArgs> ShiftAssignementsChanged;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Shift CreateShift(ShiftCreationContext context)
    {
        Logger.Log(LogLevel.Debug, "Creating {shift} from {context}", nameof(Shift), nameof(ShiftCreationContext));
        var shift = Storage.Create(context);
        _shifts.Add(shift);
        ShiftsChanged.Invoke(this, new ShiftsChangedEventArgs(ShiftChange.Creation, shift));
        Logger.Log(LogLevel.Debug, "Created {shift} -Id: {id}- from {context}", nameof(Shift), shift.Id, nameof(ShiftCreationContext));
        return shift;
    }

    public ShiftAssignement CreateShiftAssignement(ShiftAssignementCreationContext context)
    {
        Logger.Log(LogLevel.Debug, "Creating {assignement} from {context}", nameof(ShiftAssignement), nameof(ShiftAssignementCreationContext));
        var assignement = Storage.Create(context);
        _assignements.Add(assignement);
        ShiftAssignementsChanged.Invoke(this, new ShiftAssignementsChangedEventArgs(ShiftAssignementChange.Creation, assignement));
        Logger.Log(LogLevel.Debug, "Created {assignement} -Id: {id}- from {context}", nameof(ShiftAssignement), assignement.Id, nameof(ShiftAssignementCreationContext));
        return assignement;
    }

    public ShiftType CreateShiftType(ShiftTypeCreationContext context)
    {
        Logger.Log(LogLevel.Debug, "Creating {type} from {context}", nameof(ShiftType), nameof(ShiftTypeCreationContext));
        var type = Storage.Create(context);
        _types.Add(type);
        ShiftTypesChanged.Invoke(this, new ShiftTypesChangedEventArgs(ShiftTypeChange.Creation, type));
        Logger.Log(LogLevel.Debug, "Created {type} -Id: {id}- from {context}", nameof(ShiftType), type.Id, nameof(ShiftTypeCreationContext));
        return type;
    }

    public void DeleteShift(Shift shift)
    {
        var referencingAssignements = _assignements.Where(a => a.Shift == shift).ToArray();
        Logger.Log(LogLevel.Debug, "Deleting {shift} -Id: {id}- and all {count} assignement that reference it",
            nameof(Shift), shift.Id, referencingAssignements.Length);
        Storage.Delete(shift);
        foreach (var assignement in referencingAssignements)
        {
            _assignements.Remove(assignement);
            ShiftAssignementsChanged.Invoke(this, new ShiftAssignementsChangedEventArgs(ShiftAssignementChange.Deletion, assignement));
        }

        _shifts.Remove(shift);
        ShiftsChanged.Invoke(this, new ShiftsChangedEventArgs(ShiftChange.Deletion, shift));
        Logger.Log(LogLevel.Debug, "Deleted {shift} -Id: {id}- and all {count} assignement that referenced it",
            nameof(Shift), shift.Id, referencingAssignements.Length);
    }

    public void DeleteShiftAssignement(ShiftAssignement assignement)
    {
        Logger.Log(LogLevel.Debug, "Deleting {assignement} -Id: {id}-", nameof(ShiftAssignement), assignement.Id);
        Storage.Delete(assignement);
        _assignements.Remove(assignement);
        ShiftAssignementsChanged.Invoke(this, new ShiftAssignementsChangedEventArgs(ShiftAssignementChange.Deletion, assignement));
        Logger.Log(LogLevel.Debug, "Deleted {assignement} -Id: {id}-", nameof(ShiftAssignement), assignement.Id);
    }

    public void DeleteShiftType(ShiftType type)
    {
        var referenceingShifts = _shifts.Where(s => s.Type == type).ToArray();
        Logger.Log(LogLevel.Debug, "Deleting {type} -Id: {id}- and marking all {count} shifts that reference it as {obsolete}",
            nameof(ShiftType), type.Id, referenceingShifts.Length, nameof(ShiftState.Obsolete));
        Storage.Delete(type);
        foreach (var shift in referenceingShifts)
        {
            _shifts.Remove(shift);
            ShiftsChanged.Invoke(this, new ShiftsChangedEventArgs(ShiftChange.Deletion, shift));
        }

        _types.Remove(type);
        ShiftTypesChanged.Invoke(this, new ShiftTypesChangedEventArgs(ShiftTypeChange.Deletion, type));
        Logger.Log(LogLevel.Debug, "Deleted {type} -Id: {id}- and marked all {count} shifts that reference it as {obsolete}",
            nameof(ShiftType), type.Id, referenceingShifts.Length, nameof(ShiftState.Obsolete));
    }

    public void UpdateShift(Shift shift)
    {
        Logger.Log(LogLevel.Debug, "Updating {shift} -Id: {id}-", nameof(Shift), shift.Id);
        Storage.Update(shift);
        _shifts.ReplaceItem(s => s.Id == shift.Id, shift);
        ShiftsChanged.Invoke(this, new ShiftsChangedEventArgs(ShiftChange.Update, shift));
        Logger.Log(LogLevel.Debug, "Updated {shift} -Id: {id}-", nameof(Shift), shift.Id);
    }

    public void UpdateShiftType(ShiftType type)
    {
        Logger.Log(LogLevel.Debug, "Updating {type} -Id: {id}-", nameof(ShiftType), type.Id);
        Storage.Update(type);
        _types.ReplaceItem(t => t.Id == type.Id, type);
        ShiftTypesChanged.Invoke(this, new ShiftTypesChangedEventArgs(ShiftTypeChange.Update, type));
        Logger.Log(LogLevel.Debug, "Updated {type} -Id: {id}-", nameof(ShiftType), type.Id);
    }

    public void UpdateShiftAssignement(ShiftAssignement assignement)
    {
        Logger.Log(LogLevel.Debug, "Updating {type} -Id: {id}-", nameof(ShiftAssignement), assignement.Id);
        Storage.Update(assignement);
        _assignements.ReplaceItem(a => a.Id == assignement.Id, assignement);
        ShiftAssignementsChanged.Invoke(this, new ShiftAssignementsChangedEventArgs(ShiftAssignementChange.Update, assignement));
        Logger.Log(LogLevel.Debug, "Updated {type} -Id: {id}-", nameof(ShiftAssignement), assignement.Id);
    }

    #endregion
}