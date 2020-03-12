// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Runtime.Kernel.Additionals
{
    /// <summary>
    /// Determine if a defined stage of commands occured. 
    /// </summary>
    public class StageCommand
    {
        private int _stage = 0;
        private readonly string[] _stageCommands;

        /// <summary>
        /// Constructor for the stage of commands.
        /// </summary>
        /// <param name="stageCommands">an ammount of commands which must occure.</param>
        public StageCommand(params string[] stageCommands)
        {
            _stageCommands = stageCommands;
        }

        /// <summary>
        /// Handle the input command. Determine if the right order and amount of commands is reached.
        /// </summary>
        /// <param name="command">The current command which should be checked.</param>
        /// <returns>true when the right order and amount of commands is reached.</returns>
        public bool HandleChain(string command)
        {
            var currentStageCommand = _stageCommands[_stage];
            if (command != currentStageCommand)
            {
                _stage = 0;
                return false;
            }

            // Next stage
            _stage++;
            if (_stage < _stageCommands.Length)
                return false;

            // Last stage reached
            _stage = 0;
            return true;
        }
    }
}
