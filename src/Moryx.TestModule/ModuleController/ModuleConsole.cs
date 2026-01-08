// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Container;
using IContainer = Moryx.Container.IContainer;

namespace Moryx.TestModule
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

        [EntrySerialize, Description("Returns the string that was sent.")]
        public string Echo(string message)
        {
            return $"Echo: {message}";
        }

        [EntrySerialize, Description("Some short information about the server.")]
        public ShortServerInfo ServerInfo()
        {
            return new ShortServerInfo().Fill();
        }

        [EntrySerialize, Description("Echos short information about the server.")]
        public ShortServerInfo EchoServerInfo(ShortServerInfo shortServerInfo)
        {
            return shortServerInfo;
        }

        [EntrySerialize, Description("Returns the current module config.")]
        public ModuleConfig CurrentModuleConfig()
        {
            return Container.Resolve<ModuleConfig>();
        }

        [EntrySerialize, Description("Echos a module config.")]
        public ModuleConfig EchoModuleConfig(ModuleConfig moduleConfig)
        {
            return moduleConfig;
        }

        [EntrySerialize, Description("Executes the json test")]
        public void JsonTest()
        {
            var jsonTest = Container.Resolve<JsonTest>();
            jsonTest.Start();
        }

        [EntrySerialize, Description("Executes the async method")]
        public Task<string> TestAsync()
        {
            return Task.FromResult("Test");
        }
    }
}
