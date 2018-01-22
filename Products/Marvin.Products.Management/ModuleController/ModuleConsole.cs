using System;
using System.Linq;
using Marvin.Modules.Server;

namespace Marvin.Products.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public string ExportDescription(DescriptionExportFormat format)
        {
            switch (format)
            {
                case DescriptionExportFormat.Console:
                    return ExportConsoleDescription();
                case DescriptionExportFormat.Documentation:
                    return ExportHtmlDescription();
            }
            return string.Empty;
        }

        // Export your desription for the developer console here
        // This should represent the current state
        private static string ExportConsoleDescription()
        {
            return "";
        }

        // Export your desription for the supervisor or maintenance
        // This should be a static explaination of the plugin
        private static string ExportHtmlDescription()
        {
            return "";
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            if (!args.Any())
                outputStream("ProductManagement console requires arguments");
        }
    }
}
