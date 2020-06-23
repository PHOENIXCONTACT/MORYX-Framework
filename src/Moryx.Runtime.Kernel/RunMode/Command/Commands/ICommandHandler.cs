// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Interface for all commands supported by the emulator
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Check if this 
        /// </summary>
        bool CanHandle(string command);

        /// <summary>
        /// Handle the entered command
        /// </summary>
        void Handle(string[] fullCommand);

        /// <summary>
        /// Print all valid commands
        /// </summary>
        void ExportValidCommands(int pad);
    }
}
