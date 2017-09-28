using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Container;
using Marvin.Modules.Server;

namespace Marvin.Runtime.Diagnostics
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public IContainer Container { get; set; }

        public string ExportDescription(DescriptionExportFormat format)
        {
            switch (format)
            {
                case DescriptionExportFormat.Console: 
                    return ExportConsoleDescription(Container);
                case DescriptionExportFormat.Documentation: 
                    return ExportHtmlDescription();
                default:
                    return string.Empty;
            }
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            outputStream("Diagnostics does not support any console commands!");
        }

        private string ExportConsoleDescription(IContainer container)
        {
            var loadedModules = container?.GetRegisteredImplementations(typeof(IDiagnosticsPlugin)).Select(GetName).ToList() ?? new List<string>();
            var isOne = loadedModules.Count == 1;

            var manPage = $@"
     Diagnostics Plugin - Bundle SvcRuntime
     Version: {GetType().Assembly.GetName().Version}
-------------------------------------------------
The Diagnostics plugin is a host for diagnostics
modules. Each provides a specific functionality.

The following {(isOne ? "" : loadedModules.Count + " ")}module{(isOne ? "" : "s")} {(isOne ? "is" : "are")} currently registered:
";

            foreach (var loadedModule in loadedModules)
            {
                manPage += "* " + loadedModule + "\n";
            }
            return manPage;
        }

        private string ExportHtmlDescription()
        {
            return "Core plugin designed to host modular components for system and performance diagnosis. Can be easily extended by implementing" +
                   " the provided IDiagnosticsModule API";
        }

        private string GetName(Type arg)
        {
            var att = arg.GetCustomAttribute<PluginAttribute>();
            return att == null ? arg.Name : att.Name;
        }
    }
}
