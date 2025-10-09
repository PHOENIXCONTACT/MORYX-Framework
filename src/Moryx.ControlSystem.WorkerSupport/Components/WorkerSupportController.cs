// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Logging;
using Moryx.Tools;

namespace Moryx.ControlSystem.WorkerSupport
{
    [Component(LifeCycle.Singleton, typeof(IWorkerSupportController))]
    internal class WorkerSupportController : IWorkerSupportController
    {
        private Dictionary<string, IVisualInstructionSource> _instructionSources;

        private Dictionary<string, List<ActiveInstruction>> _facadeInstructions = new();

        public ModuleConfig Config { get; set; }

        public ResourceManagement ResourceManagement { get; set; }

        public IModuleLogger Logger { get; set; }

        public void Start()
        {
            _instructionSources = ResourceManagement.GetResources<IVisualInstructionSource>()
                .ToDictionary(vs => vs.Identifier, vs => vs);

            foreach (var instructionSource in _instructionSources.Values)
            {
                instructionSource.Added += OnInstructionAdded;
                instructionSource.Cleared += OnInstructionCleared;
                instructionSource.Instructions?.ForEach(i => OnInstructionAdded(instructionSource, i));
            }

            ResourceManagement.ResourceAdded += OnResourceAdded;
            ResourceManagement.ResourceRemoved += OnResourceRemoved;

            Logger.Log(LogLevel.Information, "Started worker support controller");
        }

        private void OnResourceRemoved(object sender, IResource resource)
        {
            if (resource is IVisualInstructionSource instructionSource)
            {
                instructionSource.Added -= OnInstructionAdded;
                instructionSource.Cleared -= OnInstructionCleared;
                _instructionSources.Remove(instructionSource.Identifier);
            }
        }

        private void OnResourceAdded(object sender, IResource resource)
        {
            if (resource is IVisualInstructionSource instructionSource)
            {
                _instructionSources.Add(instructionSource.Identifier, instructionSource);
                instructionSource.Added += OnInstructionAdded;
                instructionSource.Cleared += OnInstructionCleared;
            }

        }

        public void Stop()
        {
            foreach (var instructionSource in _instructionSources.Values)
            {
                instructionSource.Cleared -= OnInstructionCleared;
                instructionSource.Added -= OnInstructionAdded;
            }

            ResourceManagement.ResourceAdded -= OnResourceAdded;
            ResourceManagement.ResourceRemoved -= OnResourceRemoved;

            Logger?.Log(LogLevel.Information, "Stopped worker support controller");
        }

        public void Dispose()
        {

        }

        private void OnInstructionAdded(object sender, ActiveInstruction instruction)
        {
            var identifier = ((IVisualInstructionSource)sender).Identifier;
            ProcessInstructionItems(instruction);

            InstructionAdded?.Invoke(this, new InstructionEventArgs(identifier, instruction));
        }

        private void OnInstructionCleared(object sender, ActiveInstruction instruction)
        {
            var identifier = ((IVisualInstructionSource)sender).Identifier;

            InstructionCleared?.Invoke(this, new InstructionEventArgs(identifier, instruction));
        }

        public IReadOnlyList<ActiveInstruction> GetInstructions(string identifier)
        {
            var instructions = new List<ActiveInstruction>();

            // Load instructions from sources
            if (_instructionSources.ContainsKey(identifier))
                instructions.AddRange(_instructionSources[identifier].Instructions);

            // Include external instructions
            if (_facadeInstructions.ContainsKey(identifier))
                instructions.AddRange(_facadeInstructions[identifier]);

            return instructions;
        }

        public void AddInstruction(string identifier, ActiveInstruction instruction)
        {
            if (!_facadeInstructions.ContainsKey(identifier))
                _facadeInstructions[identifier] = new List<ActiveInstruction>();

            _facadeInstructions[identifier].Add(instruction);

            ProcessInstructionItems(instruction);

            InstructionAdded(this, new InstructionEventArgs(identifier, instruction));
        }

        public void ClearInstruction(string identifier, ActiveInstruction instruction)
        {
            if (_facadeInstructions.ContainsKey(identifier))
                _facadeInstructions[identifier].Remove(instruction);

            InstructionCleared(this, new InstructionEventArgs(identifier, instruction));
        }

        public void CompleteInstruction(string identifier, ActiveInstructionResponse response)
        {
            if (_facadeInstructions.ContainsKey(identifier))
            {
                _facadeInstructions[identifier].RemoveAll(i => i.Id == response.Id);
            }
            else if (_instructionSources.ContainsKey(identifier) && _instructionSources[identifier].Instructions.Any(a => a.Id == response.Id))
            {
                _instructionSources[identifier].Completed(response);
            }

            InstructionCleared(this, new InstructionEventArgs(identifier, new ActiveInstruction { Id = response.Id }));
        }

        private void ProcessInstructionItems(ActiveInstruction instruction)
        {
            // Apply processor to each item
            foreach (var processorConfig in Config.ProcessorConfigs)
            {
                var processor = new InstructionProcessor(processorConfig);
                foreach (var visualInstruction in instruction.Instructions)
                {
                    processor.ProcessItem(visualInstruction);
                }
            }
        }

        public IReadOnlyList<string> GetInstructors()
        {
            return _instructionSources.Keys
                .Union(_facadeInstructions.Keys)
                .Distinct()
                .ToList();
        }

        public event EventHandler<InstructionEventArgs> InstructionAdded;

        public event EventHandler<InstructionEventArgs> InstructionCleared;
    }
}
