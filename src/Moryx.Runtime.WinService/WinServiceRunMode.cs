// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using CommandLine;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Kernel;
using Moryx.Tools;

namespace Moryx.Runtime.WinService
{
    /// <summary>
    /// Environment wrapper for the windows service
    /// </summary>
    [RunMode(typeof(WinServiceOptions))]
    public class WinServiceRunMode : RunModeBase<WinServiceOptions>
    {
        private const string InstallLogPath = "install.log";
        private const string UninstallLogPath = "uninstall.log";

        /// <summary>
        /// Config manager instance.
        /// </summary>
        public IRuntimeConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Run environment
        /// </summary>
        /// <returns>0: All fine - 1: Warning - 2: Error</returns>
        public override RuntimeErrorCode Run()
        {
            if (Options.Install)
                return InstallService();

            if (Options.Uninstall)
                return UninstallService();

            var service = new HeartOfGoldService(ModuleManager, ConfigManager);
            service.Run();
            return RuntimeErrorCode.NoError;
        }

        /// <summary>
        /// Installs the service in the windows service registry
        /// </summary>
        private RuntimeErrorCode InstallService()
        {
            if (!IsPrivileged())
                return RuntimeErrorCode.Error;

            try
            {
                var spi = new ServiceProcessInstaller();
                var si = new ServiceInstaller();

                //# Service Account Information
                spi.Username = Options.User;
                spi.Password = Options.Password;
                spi.Account = Options.Account;

                //# Service Information
                si.Parent = spi;
                si.StartType = ServiceStartMode.Automatic;
                si.ServicesDependedOn = Options.Dependencies.ToArray();
                si.Description = Platform.Current.ProductDescription;

                //# This must be identical to the MoryxService.ServiceBase name
                //# set in the constructor of MoryxService.cs
                si.ServiceName = Platform.Current.ProductName;

                // Install context
                si.Context = new InstallContext(InstallLogPath, null);

                // Execution path with parameters
                var exec = Assembly.GetEntryAssembly();
                var cmdOptions = Parser.Default.FormatCommandLine(new WinServiceOptions
                {
                    ConfigDir = Options.ConfigDir
                });

                // ReSharper disable once PossibleNullReferenceException
                var serviceExecution = AppendPathParameter(exec.Location, cmdOptions);
                // ReSharper disable once StringLiteralTypo
                si.Context.Parameters["assemblypath"] = serviceExecution;

                // Install service
                IDictionary stateSaver = new Hashtable();
                si.Install(stateSaver);

                return RuntimeErrorCode.NoError;
            }
            catch (Exception e)
            {
                Console.WriteLine(ExceptionPrinter.Print(e));
                return RuntimeErrorCode.Error;
            }
        }

        /// <summary>
        /// Removes the service from the windows service registry
        /// </summary>
        private static RuntimeErrorCode UninstallService()
        {
            if (!IsPrivileged())
                return RuntimeErrorCode.Error;

            try
            {
                var si = new ServiceInstaller();
                var spi = new ServiceProcessInstaller();
                si.Parent = spi;
                si.ServiceName = Platform.Current.ProductName;

                // Install context
                si.Context = new InstallContext(UninstallLogPath, null);

                // Uninstall service
                si.Uninstall(null);

                return RuntimeErrorCode.NoError;
            }
            catch (Exception e)
            {
                Console.WriteLine(ExceptionPrinter.Print(e));
                return RuntimeErrorCode.Error;
            }
        }

        private static string AppendPathParameter(string path, string parameter)
        {
            if (path.Length > 0 && path[0] != '"')
                path = "\"" + path + "\"";

            path += " " + parameter;
            return path;
        }

        private static bool IsPrivileged()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                var isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);

                if (isElevated)
                    return true;

                Console.WriteLine("The current user {0} is not privileged. Run as administrator!",
                    identity.Name);

                return false;
            }
        }
    }
}
