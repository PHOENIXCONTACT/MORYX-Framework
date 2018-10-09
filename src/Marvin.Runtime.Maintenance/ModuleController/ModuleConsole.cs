using System;
using System.Linq;
using System.Reflection;
using Marvin.Container;
using Marvin.Model;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Runtime.Modules;
using Marvin.Serialization;

namespace Marvin.Runtime.Maintenance
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
                    return ExportConsoleDescription();
                case DescriptionExportFormat.Documentation:
                    return ExportHtmlDescription();
            }
            return string.Empty;
        }

        private string ExportConsoleDescription()
        {
            if (Container == null)
                return "Man page only available if plugin is in state Ready or Running";

            var modules = Container.GetRegisteredImplementations(typeof(IMaintenancePlugin));
            var names = modules.Select(type => "* " + type.GetCustomAttribute<PluginAttribute>().Name);
            var manPage = $@"
   Maintenance Module - Bundle Maintenance
   Version: {GetType().Assembly.GetName().Version}
---------------------------------------------
This module provides maintenance access to all
module including itself. It is plugin based
and can be extended and customized. 

Available plugins:
{string.Join("\n", names)}
  
";
            return manPage;
        }

        private string ExportHtmlDescription()
        {
            return "Core module to maintain the application. It provides config and logging support by default. Database configuration can be " +
                   "included with an additional plugin as well as other extensions implementing IMaintenanceModule";
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            outputStream("Maintenance does not support any console commands!");
        }
    }
}
