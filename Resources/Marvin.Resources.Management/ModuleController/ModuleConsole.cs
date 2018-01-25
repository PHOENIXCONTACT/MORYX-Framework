using System;
using Marvin.Runtime.Modules;

namespace Marvin.Resources.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public ResourceTypeController TypeController { get; set; }

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
        private string ExportConsoleDescription()
        {
            var manPage =
@"

";
            return manPage;
        }

        // Export your desription for the supervisor or maintenance
        // This should be a static explaination of the plugin
        private string ExportHtmlDescription()
        {
            var manPage =
@"

";
            return manPage;
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
        }
    }
}
