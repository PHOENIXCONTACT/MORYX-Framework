// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;
using Moryx.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Moryx.Modules;
using Moryx.Shifts.Management.Localizations;

[assembly: InternalsVisibleTo("Moryx.Shifts.Management.IntegrationTests")]

namespace Moryx.Shifts.Management
{
    internal class ShiftManagementFacade : FacadeBase, IShiftManagement
    {
        #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IShiftManager ShiftManager { get; set; }

        public IParallelOperations ParallelOperations { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        #endregion

        #region IShiftManagement

        public IReadOnlyList<Shift> Shifts => ShiftManager.Shifts;
        public IReadOnlyList<ShiftType> ShiftTypes => ShiftManager.ShiftTypes;
        public IReadOnlyList<ShiftAssignement> ShiftAssignements => ShiftManager.ShiftAssignements;

        public event EventHandler<ShiftsChangedEventArgs>? ShiftsChanged;
        public event EventHandler<ShiftTypesChangedEventArgs>? TypesChanged;
        public event EventHandler<ShiftAssignementsChangedEventArgs>? AssignementsChanged;

        public Shift CreateShift(ShiftCreationContext shiftCreationContext)
        {
            ValidateHealthState();
            ArgumentNullException.ThrowIfNull(shiftCreationContext);
            return ShiftManager.CreateShift(Verified(shiftCreationContext));
        }

        private ShiftCreationContext Verified(ShiftCreationContext context)
        {
            context.Type = ShiftTypes.SingleOrDefault(t => t.Id == context.Type?.Id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(ShiftType), context.Type?.Id));

            return context;
        }

        public void UpdateShift(Shift shift)
        {
            ValidateHealthState();
            ArgumentNullException.ThrowIfNull(shift);
            ShiftManager.UpdateShift(Verified(shift));
        }

        private Shift Verified(Shift shift)
        {
            var verifiedShift = Shifts.SingleOrDefault(s => s.Id == shift.Id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(Shift), shift.Id));
            verifiedShift.Type = ShiftTypes.SingleOrDefault(t => t.Id == shift.Type?.Id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(ShiftType), shift.Type?.Id));

            return shift;
        }

        public void DeleteShift(long id)
        {
            ValidateHealthState();
            ShiftManager.DeleteShift(VerifiedShift(id));
        }

        private Shift VerifiedShift(long id)
        {
            var shift = Shifts.SingleOrDefault(s => s.Id == id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(Shift), id));
            return shift;
        }

        public ShiftType CreateShiftType(ShiftTypeCreationContext shiftTypeCreationContext)
        {
            ValidateHealthState();
            ArgumentNullException.ThrowIfNull(shiftTypeCreationContext);
            return ShiftManager.CreateShiftType(shiftTypeCreationContext);
        }

        public void UpdateShiftType(ShiftType shiftType)
        {
            ValidateHealthState();
            ArgumentNullException.ThrowIfNull(shiftType);
            ShiftManager.UpdateShiftType(Verified(shiftType));
        }

        private ShiftType Verified(ShiftType type)
        {
            var verifiedType = ShiftTypes.SingleOrDefault(t => t.Id == type.Id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(ShiftType), type.Id));

            return type;
        }

        public void DeleteShiftType(long id)
        {
            ValidateHealthState();
            ShiftManager.DeleteShiftType(VerifiedType(id));
        }

        private ShiftType VerifiedType(long id)
        {
            var type = ShiftTypes.SingleOrDefault(s => s.Id == id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(ShiftType), id));
            return type;
        }

        public ShiftAssignement CreateShiftAssignement(ShiftAssignementCreationContext shiftAssignementCreationContext)
        {
            ValidateHealthState();
            ArgumentNullException.ThrowIfNull(shiftAssignementCreationContext);
            return ShiftManager.CreateShiftAssignement(Verified(shiftAssignementCreationContext));
        }

        private ShiftAssignementCreationContext Verified(ShiftAssignementCreationContext context)
        {
            context.Shift = Shifts.SingleOrDefault(s => s.Id == context.Shift?.Id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(Shift), context.Shift?.Id));

            return context;
        }

        public void UpdateShiftAssignement(ShiftAssignement assignement)
        {
            ValidateHealthState();
            ArgumentNullException.ThrowIfNull(assignement);
            ShiftManager.UpdateShiftAssignement(Verified(assignement));
        }

        private ShiftAssignement Verified(ShiftAssignement assignement)
        {
            var verifiedShift = ShiftAssignements.SingleOrDefault(a => a.Id == assignement.Id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(ShiftAssignement), assignement.Id));
            verifiedShift.Shift = Shifts.SingleOrDefault(s => s.Id == verifiedShift.Shift?.Id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(Shift), verifiedShift.Shift?.Id));

            return assignement;
        }

        public void DeleteShiftAssignement(long id)
        {
            ValidateHealthState();
            ShiftManager.DeleteShiftAssignement(VerifiedAssignement(id));
        }

        private ShiftAssignement VerifiedAssignement(long id)
        {
            var assignement = ShiftAssignements.SingleOrDefault(a => a.Id == id) ??
                throw new ArgumentException(string.Format(Strings.ID_NOT_FOUND, nameof(ShiftAssignement), id));

            return assignement;
        }

        #endregion

        #region FacadeBase

        public override void Activate()
        {
            base.Activate();

            ShiftManager.ShiftsChanged += ParallelOperations.DecoupleListener<ShiftsChangedEventArgs>(OnShiftsChanged);
            ShiftManager.ShiftTypesChanged += ParallelOperations.DecoupleListener<ShiftTypesChangedEventArgs>(OnTypesChanged);
            ShiftManager.ShiftAssignementsChanged += ParallelOperations.DecoupleListener<ShiftAssignementsChangedEventArgs>(OnAssignementsChanged);
        }

        private void OnAssignementsChanged(object? sender, ShiftAssignementsChangedEventArgs e) => AssignementsChanged?.Invoke(this, e);
        private void OnTypesChanged(object? sender, ShiftTypesChangedEventArgs e) => TypesChanged?.Invoke(this, e);
        private void OnShiftsChanged(object? sender, ShiftsChangedEventArgs e) => ShiftsChanged?.Invoke(this, e);

        public override void Deactivate()
        {
            ShiftManager.ShiftsChanged -= ParallelOperations.RemoveListener<ShiftsChangedEventArgs>(OnShiftsChanged);
            ShiftManager.ShiftTypesChanged -= ParallelOperations.RemoveListener<ShiftTypesChangedEventArgs>(OnTypesChanged);
            ShiftManager.ShiftAssignementsChanged -= ParallelOperations.RemoveListener<ShiftAssignementsChangedEventArgs>(OnAssignementsChanged);

            base.Deactivate();
        }

        #endregion
    }
}

