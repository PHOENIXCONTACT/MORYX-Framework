// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using System;

namespace Moryx.ControlSystem.VisualInstructions.Endpoints.Tests
{
    internal class WorkerSupportMock : IWorkerSupport
    {
        private Dictionary<string, List<ActiveInstruction>> _instructions;

        public string CompleteResult { get; private set; }

        public event EventHandler<InstructionEventArgs> InstructionAdded;
        public event EventHandler<InstructionEventArgs> InstructionCleared;

        public WorkerSupportMock()
        {
            _instructions = new Dictionary<string, List<ActiveInstruction>>()
            {
                ["Instructor"] = new List<ActiveInstruction> { new ActiveInstruction() },
                ["Instructor with spaces"] = new List<ActiveInstruction> { new ActiveInstruction() },
                ["Ümlaut"] = new List<ActiveInstruction> { new ActiveInstruction() },
            };
        }

        public void AddInstruction(string identifier, ActiveInstruction instruction)
        {
            _instructions[identifier].Add(instruction);
        }

        public void ClearInstruction(string identifier, ActiveInstruction instruction)
        {
            _instructions[identifier].RemoveAll(i => i.Id == instruction.Id);
        }

        public void CompleteInstruction(string identifier, long instructionId, string result)
        {
            ClearInstruction(identifier, _instructions[identifier].FirstOrDefault(i => i.Id == instructionId));
            CompleteResult = result;
        }

        public IReadOnlyList<ActiveInstruction> GetInstructions(string identifier)
            => _instructions.ContainsKey(identifier)
                ? _instructions[identifier]
                : Array.Empty<ActiveInstruction>();

        public IReadOnlyList<string> GetInstructors()
        {
            return _instructions.Keys.ToArray();
        }

        public void CompleteInstruction(string identifier, ActiveInstructionResponse response)
        {
            ClearInstruction(identifier, _instructions[identifier].FirstOrDefault(i => i.Id == response.Id));
            CompleteResult = response.SelectedResult.Key;
        }
    }
}
