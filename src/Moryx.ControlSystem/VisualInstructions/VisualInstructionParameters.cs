// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer;
using Moryx.Serialization;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Provides parameters with visual instructions
    /// </summary>
    [DataContract]
    public class VisualInstructionParameters : Parameters
    {
        /// <summary>
        /// All instructions for this activity, if it used as a visual activity
        /// </summary>
        [EntrySerialize, DataMember]
        public VisualInstruction[] Instructions { get; set; }

        /// <summary>
        /// Inputs for this activity.
        /// </summary>
        public object Inputs { get; set; }

        /// <summary>
        /// Binder to resolve visual instruction bindings
        /// </summary>
        protected VisualInstructionBinder InstructionBinder { get; private set; }

        /// <inheritdoc />
        protected override void Populate(IProcess process, Parameters instance)
        {
            var parameters = (VisualInstructionParameters)instance;

            // No instructions, no binding!
            if (Instructions == null || Instructions.Length == 0)
                return;

            // Create binder for our instructions
            if (InstructionBinder == null)
                InstructionBinder = new VisualInstructionBinder(Instructions, ResolverFactory);

            // Resolve instructions
            parameters.Instructions = InstructionBinder.ResolveInstructions(process) ?? Instructions;
        }
    }
}
