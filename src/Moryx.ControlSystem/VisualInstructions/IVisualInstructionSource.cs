// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Source of visual instructions within the resource graph
    /// </summary>
    public interface IVisualInstructionSource : IResource
    {
        /// <summary>
        /// Identifier of this instruction source
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Instructions on this source
        /// </summary>
        IReadOnlyList<ActiveInstruction> Instructions { get; }

        /// <summary>
        /// An instruction was completed
        /// </summary>
        void Completed(ActiveInstructionResponse response);

        /// <summary>
        /// Instruction was added
        /// </summary>
        event EventHandler<ActiveInstruction> Added;

        /// <summary>
        /// Instruction was cleared
        /// </summary>
        event EventHandler<ActiveInstruction> Cleared;
    }
}