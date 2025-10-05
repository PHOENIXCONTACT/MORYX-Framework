// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moryx.Bindings;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Class to resolve visual instruction bindings
    /// </summary>
    public class VisualInstructionBinder
    {
        private readonly InstructionResolver[] _instructionResolvers;

        /// <summary>
        /// Creates a new instance of <see cref="VisualInstructionBinder"/>
        /// </summary>
        public VisualInstructionBinder(IEnumerable<VisualInstruction> instructions, IBindingResolverFactory resolverFactory)
        {
            _instructionResolvers = instructions.Where(inst => inst.Type != InstructionContentType.Unknown).Select(inst => new InstructionResolver(inst, resolverFactory)).ToArray();
        }

        /// <summary>
        /// Will resolve the instructions for the given process
        /// </summary>
        public VisualInstruction[] ResolveInstructions(object source)
        {
            var instructions = new VisualInstruction[_instructionResolvers.Length];
            for (var index = 0; index < _instructionResolvers.Length; index++)
                instructions[index] = _instructionResolvers[index].Resolve(source);

            return instructions;
        }

        private class InstructionResolver
        {
            public InstructionResolver(VisualInstruction instruction, IBindingResolverFactory resolverFactory)
            {
                _instruction = instruction;

                _contentResolver = TextBindingResolverFactory.Create(instruction.Content ?? string.Empty, resolverFactory);
                if (!string.IsNullOrEmpty(instruction.Preview))
                    _previewResolver = TextBindingResolverFactory.Create(instruction.Preview, resolverFactory);
            }

            private readonly VisualInstruction _instruction;

            private readonly ITextBindingResolver _contentResolver;

            private readonly ITextBindingResolver _previewResolver;

            public VisualInstruction Resolve(object source)
            {
                return new VisualInstruction
                {
                    Type = _instruction.Type,
                    Content = _contentResolver.Resolve(source),
                    Preview = _previewResolver?.Resolve(source)
                };
            }
        }
    }
}
