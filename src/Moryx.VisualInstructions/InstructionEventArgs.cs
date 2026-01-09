// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.VisualInstructions;

/// <summary>
/// Event args for instruction events
/// </summary>
public class InstructionEventArgs : EventArgs
{
    /// <summary>
    /// Create new instance
    /// </summary>
    public InstructionEventArgs(string identifier, ActiveInstruction instruction)
    {
        Identifier = identifier;
        Instruction = instruction;
    }

    /// <summary>
    /// Identifier of source
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Referenced instruction
    /// </summary>
    public ActiveInstruction Instruction { get; }
}