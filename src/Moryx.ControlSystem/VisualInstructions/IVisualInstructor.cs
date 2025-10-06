// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.VisualInstructions
{
    /// <summary>
    /// Instructor that represents a specific screen/client of the application. 
    /// </summary>
    public interface IVisualInstructor : IResource
    {
        /// <summary>
        /// Only display these instructions
        /// Has to be cleared with the <see cref="Clear"/> method
        /// </summary>
        /// <returns>Instruction id to clear the instruction</returns>
        long Display(ActiveInstruction instruction);

        /// <summary>
        /// Only display these instructions
        /// Instruction will be cleared automatically after the given time
        /// </summary>
        void Display(ActiveInstruction instruction, int autoClearMs);

        /// <summary>
        /// Execute the instructions and display possbile results and inputs. The callback contains the selected option
        /// and possible user input.
        /// </summary>
        /// <returns>Id of the instruction to clear manually</returns>
        long Execute(ActiveInstruction instruction, Action<ActiveInstructionResponse> callback);

        /// <summary>
        /// Clears specified Instruction from UI.
        /// </summary>
        void Clear(long instructionId);
    }
}
