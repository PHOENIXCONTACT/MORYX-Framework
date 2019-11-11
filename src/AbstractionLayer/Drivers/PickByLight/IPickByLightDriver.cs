using System;

namespace Marvin.AbstractionLayer.Drivers.PickByLight
{
    /// <summary>
    /// Interface for the pick by light driver
    /// </summary>
    public interface IPickByLightDriver : IDriver
    {
        /// <summary>
        /// Activate instruction for this address
        /// </summary>
        void ActivateInstruction(string address, LightInstructions instruction);

        /// <summary>
        /// Deactivate an instruction
        /// </summary>
        void DeactivateInstruction(string address);

        /// <summary>
        /// Instruction was confirmed
        /// </summary>
        event EventHandler<InstructionConfirmation> InstructionConfirmed;
    }
}