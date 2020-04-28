// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using Marvin.Runtime;
using Marvin.Runtime.Modules;
using Marvin.Serialization;
using IContainer = Marvin.Container.IContainer;

namespace Marvin.TestModule
{
    internal class ShortServerInfo
    {
        public string ServerTime { get; set; }

        public class AssemblyInfo
        {
            public string AssemblyProduct { get; set; }
            public string AssemblyVersion { get; set; }
            public string AssemblyInformationalVersion { get; set; }
            public string AssemblyDescription { get; set; }

            public void Fill()
            {
                var currentPlatform = Platform.Current;
                AssemblyProduct = currentPlatform.ProductName;
                AssemblyVersion = currentPlatform.ProductVersion.ToString(3);
                AssemblyInformationalVersion = currentPlatform.ProductVersion.ToString(3);
                AssemblyDescription = currentPlatform.ProductDescription;
            }
        }

        public class HostInfo
        {
            public string MachineName { get; set; }
            public string OSInformation { get; set; }
            public long UpTime { get; set; }

            public void Fill()
            {
                MachineName = Environment.MachineName;
                OSInformation = Environment.OSVersion.ToString();
                UpTime = Environment.TickCount;
            }
        }

        public AssemblyInfo Assembly { get; set; } = new AssemblyInfo();
        public HostInfo Host { get; set; } = new HostInfo();

        public ShortServerInfo Fill()
        {
            ServerTime = DateTime.Now.ToLongDateString();
            Assembly.Fill();
            Host.Fill();
            return this;
        }
    }

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

        // Export your desription for the developer console here
        // This should represent the current state
        private string ExportConsoleDescription()
        {
            var manPage =
@"
Test module for System tests
";
            return manPage;
        }

        // Export your desription for the supervisor or maintenance
        // This should be a static explaination of the plugin
        private string ExportHtmlDescription()
        {
            var manPage =
@"
Test module for System tests
";
            return manPage;
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            outputStream("The TestModule does not provide any commands!");
        }


        [EditorBrowsable, Description("Shows the description of this module.")]
        public string ModuleDescription()
        {
            return ExportHtmlDescription();
        }

        [EditorBrowsable, Description("Returns the string that was sent.")]
        public string Echo(string message)
        {
            return $"Echo: {message}";
        }

        [EditorBrowsable, Description("Some short information about the server.")]
        public ShortServerInfo ServerInfo()
        {
            return new ShortServerInfo().Fill();
        }

        [EditorBrowsable, Description("Echos short information about the server.")]
        public ShortServerInfo EchoServerInfo(ShortServerInfo shortServerInfo)
        {
            return shortServerInfo;
        }

        [EditorBrowsable, Description("Returns the current module config.")]
        public ModuleConfig CurrentModuleConfig()
        {
            return Container.Resolve<ModuleConfig>();
        }

        [EditorBrowsable, Description("Echos a module config.")]
        public ModuleConfig EchoModuleConfig(ModuleConfig moduleConfig)
        {
            return moduleConfig;
        }
    }
}
