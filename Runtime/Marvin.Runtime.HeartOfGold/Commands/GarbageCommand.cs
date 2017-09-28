using System;
using Marvin.Container;

namespace Marvin.Runtime.HeartOfGold
{
    [Plugin(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class GarbageCommand : ICommandHandler
    {
        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "gc";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length < 2)
                return;

            switch (fullCommand[1])
            {
                case "collect":
                    GC.Collect();
                    break;
            }
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
        }
    }
}