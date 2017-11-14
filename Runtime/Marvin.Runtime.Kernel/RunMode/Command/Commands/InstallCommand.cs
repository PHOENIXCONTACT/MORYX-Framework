using System;
using System.Configuration.Install;
using System.Reflection;
using Marvin.Container;

namespace Marvin.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class InstallCommand : ICommandHandler
    {
        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "install" | command == "uninstall";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            ManagedInstallerClass.InstallHelper(fullCommand[0] == "install"
                ? new[] { Assembly.GetExecutingAssembly().Location }
                : new[] { "/u", Assembly.GetExecutingAssembly().Location });
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("install".PadRight(pad) + "Installs this application as a windows service");
            Console.WriteLine("uninstall".PadRight(pad) + "Uninstalls this application as windows service");
        }
    }
}
