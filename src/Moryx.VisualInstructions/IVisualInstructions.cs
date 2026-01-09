// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.VisualInstructions;

/// <summary>
/// Facade for worker support module
/// </summary>
public interface IVisualInstructions
{
    /// <summary>
    /// Get all instructions for a given client identifier
    /// </summary>
    IReadOnlyList<ActiveInstruction> GetInstructions(string identifier);

    /// <summary>
    /// Add an instruction for an identifier
    /// </summary>
    void AddInstruction(string identifier, ActiveInstruction instruction);

    /// <summary>
    /// Clear an instruction on screen
    /// </summary>
    void ClearInstruction(string identifier, ActiveInstruction instruction);

    /// <summary>
    /// Complete an instruction with the response received from the client
    /// </summary>
    void CompleteInstruction(string identifier, ActiveInstructionResponse response);

    /// <summary>
    /// Event raised when an instruction was added
    /// </summary>
    event EventHandler<InstructionEventArgs> InstructionAdded;

    /// <summary>
    /// Event raised when an instruction was removed
    /// </summary>
    event EventHandler<InstructionEventArgs> InstructionCleared;

    /// <summary>
    /// Get a list of all available instructors
    /// </summary>
    public IReadOnlyList<string> GetInstructors();
}