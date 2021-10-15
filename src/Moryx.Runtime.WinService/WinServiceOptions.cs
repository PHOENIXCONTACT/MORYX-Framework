// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using CommandLine;
using Moryx.Runtime.Kernel;

#if NETFRAMEWORK
using System.Collections.Generic;
using System.ServiceProcess;
using CommandLine.Text;
#endif

namespace Moryx.Runtime.WinService
{
    /// <summary>
    /// Option class for the <see cref="WinServiceRunMode"/>
    /// </summary>
    [Verb("service", HelpText = "Starts the runtime in service mode. Used for Windows Services.")]
    public class WinServiceOptions : RuntimeOptions
    {
#if NETFRAMEWORK
        /// <summary>
        /// If set, the install process will be started
        /// </summary>
        [Option('i', "install", HelpText = "Installs this application as windows service")]
        public bool Install { get; set; }

        /// <summary>
        /// If set, the uninstall process will be started
        /// </summary>
        [Option('u', "uninstall", HelpText = "Uninstalls this application as windows service.")]
        public bool Uninstall { get; set; }

        /// <summary>
        /// Specifies a service's security context
        /// </summary>
        [Option('s', "security", Default = ServiceAccount.User, HelpText = "Specifies a service's security context (LocalService, NetworkService, LocalSystem, User)")]
        public ServiceAccount Account { get; set; }

        /// <summary>
        /// The user account under which the service application will run.
        /// </summary>
        [Option("user", HelpText = "The user account under which the service application will run.")]
        public string User { get; set; }

        /// <summary>
        /// The password associated with the user account under which the service application runs.
        /// </summary>
        [Option("password", HelpText = "The password associated with the user account under which the service application runs.")]
        public string Password { get; set; }

        /// <summary>
        /// Space separated list of services that must be running for this service to run.
        /// </summary>
        [Option('d', "dependencies", HelpText = "Space separated list of services that must be running for this service to run.")]
        public IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// Examples for the help output
        /// </summary>
        [Usage]
        public static IEnumerable<Example> Examples =>
            new List<Example> {
                new Example("Installs the service in the Windows Service Registry", new WinServiceOptions
                {
                    Install = true,
                    Account = ServiceAccount.User,
                    User = "domain\\JohnDoe",
                    Password = "secret",
                    ConfigDir = @"C:\YourApp\Config",
                    Dependencies = new[] { "postgresql-x64-11" }
                }),
                new Example("Uninstalls the service in the Windows Service Registry", new WinServiceOptions { Uninstall = true}),
            };
#endif
    }
}
