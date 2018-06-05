using System;
using Marvin.Container;

namespace Marvin.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class ProductCommand : ICommandHandler
    {
        /// <summary>
        /// Check if this 
        /// </summary>
        public bool CanHandle(string command)
        {
            return command == "mname";
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            Console.WriteLine();
            Console.WriteLine("Deployed product:");
            Console.WriteLine("Product: " + Platform.Current.ProductName);
            Console.WriteLine("Version: " + Platform.Current.ProductVersion);
            Console.WriteLine();
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
            Console.WriteLine("mname".PadRight(pad) + "Prints the currently executed product");
        }
    }
}
