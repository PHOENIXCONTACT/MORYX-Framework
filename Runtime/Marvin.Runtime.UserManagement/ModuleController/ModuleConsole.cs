using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Marvin.Container;
using Marvin.Runtime.UserManagement.UserAuthentication;
using Marvin.Runtime.UserManagement.UserAuthenticator;

namespace Marvin.Runtime.UserManagement
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        #region Dependency Injection

        public IContainer Container { get; set; }

        public IUserAuthentication Authentication { get; set; }

        #endregion

        public string ExportDescription(DescriptionExportFormat format)
        {
            switch (format)
            {
                case DescriptionExportFormat.Console:
                    return ExportConsoleDescription();
                case DescriptionExportFormat.Documentation:
                    return ExportHtmlDescription();
                default:
                    return string.Empty;
            }
        }

        private string ExportHtmlDescription()
        {
            return "Module handeling user authorization and operation accesses based on windows user groups.";
        }

        private string ExportConsoleDescription()
        {
            var strategies = Container.GetRegisteredImplementations(typeof(IUserAuthenticator))
                                      .Select(auth => "* " + auth.GetCustomAttribute<PluginAttribute>().Name);
            var stratString = string.Join("\n", strategies.ToArray());
            var manPage = string.Format(@"
  UserManager Module - Bundle Marvin.Runtime
  Version: {0}
---------------------------------------------
This module provides user group based access
and UI customization. For each application a
different authenticator strategy can be used.

Installed strategies:
{1}
  
", GetType().Assembly.GetName().Version, stratString);
            return manPage;
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            switch (args[0])
            {
                case "user":
                    var um = Authentication.GetUserInfos(WindowsIdentity.GetCurrent());
                    Console.WriteLine("User: {0}", um.Username);
                    Console.WriteLine("Groups:");
                    foreach (var group in um.Groups)
                    {
                        Console.WriteLine("  {0}", group);
                    }
                    break;
            }
        }
    }
}
