﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Threading;

namespace Moryx.ControlSystem.WorkerSupport
{
    internal class WorkerSupportFacade : IWorkerSupport, IFacadeControl
    {
        public Action ValidateHealthState { get; set; }
        public WorkerSupportFacade()
        {
            ValidateHealthState = InitialValidateHealthState;
        }

        #region Dependencies

        public IModuleLogger Logger { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public IWorkerSupportController Controller { get; set; }

        #endregion

        /// <inheritdoc cref="IFacadeControl"/>
        public void Activate()
        {
            Controller.InstructionAdded += ParallelOperations.DecoupleListener<InstructionEventArgs>(OnInstructionAdded);
            Controller.InstructionCleared += ParallelOperations.DecoupleListener<InstructionEventArgs>(OnInstructionCleared);
        }

        /// <inheritdoc cref="IFacadeControl"/>
        public void Deactivate()
        {
            Controller.InstructionCleared -= ParallelOperations.RemoveListener<InstructionEventArgs>(OnInstructionCleared);
            Controller.InstructionAdded -= ParallelOperations.RemoveListener<InstructionEventArgs>(OnInstructionAdded);
        }

        private void OnInstructionAdded(object sender, InstructionEventArgs e)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            InstructionAdded?.Invoke(this, e);
        }

        private void OnInstructionCleared(object sender, InstructionEventArgs e)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            InstructionCleared?.Invoke(this, e);
        }

        public IReadOnlyList<ActiveInstruction> GetInstructions(string identifier)
        {
            ValidateHealthState();
            return Controller.GetInstructions(identifier);
        }

        public void AddInstruction(string identifier, ActiveInstruction instruction)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            ValidateHealthState();
            Controller.AddInstruction(identifier, instruction);
        }

        public void ClearInstruction(string identifier, ActiveInstruction instruction)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            ValidateHealthState();
            Controller.ClearInstruction(identifier, instruction);
        }

        public void CompleteInstruction(string identifier, ActiveInstructionResponse response)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            ValidateHealthState();
            Controller.CompleteInstruction(identifier, response);
        }

        public IReadOnlyList<string> GetInstructors()
        {
            ValidateHealthState();
            return Controller.GetInstructors();
        }

        private void InitialValidateHealthState()
        {
            throw new HealthStateException(ServerModuleState.Stopped);
        }

        public event EventHandler<InstructionEventArgs> InstructionAdded;

        public event EventHandler<InstructionEventArgs> InstructionCleared;
    }
}
