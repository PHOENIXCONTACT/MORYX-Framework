using System;
using System.IO;
using System.Text;
using Marvin.Container;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class SetupCommand : ICommandHandler
    {
        public IRuntimeConfigManager ConfigManager { get; set; }

        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Check if this
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "setup";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            if (fullCommand.Length < 2)
            {
                Console.WriteLine("Insufficient number of arguments!");
                return;
            }

            switch (fullCommand[1])
            {
                case "-p":
                    PrintSetup();
                    break;
                case "-c":
                    SetConfigDirectory(fullCommand[2]);
                    break;
            }
        }

        private void PrintSetup()
        {
            // Build line
            var halfLine = new StringBuilder();
            for (var i = 0; i < (Console.WindowWidth - 15) / 2; i++)
                halfLine.Append("-");

            Console.WriteLine(halfLine + " Current setup " + halfLine);
            Console.WriteLine("Execution directory: " + Directory.GetCurrentDirectory());
            Console.WriteLine("ConfigDir: " + ConfigManager.ConfigDirectory);
        }

        private void SetConfigDirectory(string configDir)
        {
            // Stop all services
            ModuleManager.StopModules();

            // Save current configs
            ConfigManager.SaveAll();

            // Relocate and reload configs
            ConfigManager.ConfigDirectory = configDir;
            ConfigManager.ClearCache();

            // Restart all
            ModuleManager.StartModules();
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("setup -p".PadRight(pad) + "Print the current setup. ConfigDirectory, etc...");
            Console.WriteLine("      -c".PadRight(pad) + "Change the config directory.");
        }
    }
}
