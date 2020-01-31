// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Runtime.Kernel;
using Moryx.Runtime.Maintenance.Plugins.Modules;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.TestModule;
using Moryx.TestTools.SystemTest;
using Moryx.Tools.Wcf;
using NUnit.Framework;

namespace Moryx.Runtime.SystemTests
{
    /// <summary>
    /// These tests shall check two aspects: They shall verify the HoG functionality but also wether the HeartOfGoldController is working as expected.
    /// </summary>
    [TestFixture]
    public class BasicConfigTests : IDisposable
    {
        private const int OrgIntegerValue = 3;
        private const long OrgLongValue = 424242424242;
        private const bool OrgBoolValue = false;
        private const double OrgDoubleValue = 2718;
        private const string OrgStringValue = "World";
        private const ConfigEnumeration OrgEnumValue = ConfigEnumeration.Value1;

        private const int NewIntegerValue = 7;
        private const long NewLongValue = 42424242424242;
        private const bool NewBoolValue = true;
        private const double NewDoubleValue = 1414;
        private const string NewStringValue = "Hello World";
        private const ConfigEnumeration NewEnumValue = ConfigEnumeration.Value5;

        private HeartOfGoldController _hogController;
        private ConfigManager _configManager;

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory(HogHelper.ConfigDir);

            _configManager = new RuntimeConfigManager
            {
                ConfigDirectory = HogHelper.ConfigDir
            };

            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 60
            };

            ModuleConfig config = new ModuleConfig
            {
                SleepTime = 0,

                Config = new PortConfig(),

                IntegerValue = OrgIntegerValue,
                DoubleValue = OrgDoubleValue,
                StringValue = OrgStringValue,
                LongValue = OrgLongValue,
                BoolValue = OrgBoolValue,
                EnumValue = OrgEnumValue,
                TestPlugin = new TestPluginConfig
                {
                    PluginBoolValue = !OrgBoolValue,
                    PluginDoubleValue = OrgDoubleValue * 100,
                    PluginIntegerValue = OrgIntegerValue * 100,
                    PluginLongValue = OrgLongValue * 100,
                    PluginStringValue = OrgStringValue + " asdf",
                    PluginEnumValue = OrgEnumValue + 1
                },
                Plugins = new List<TestPluginConfig>
                {
                    new TestPluginConfig1
                    {
                        PluginBoolValue = OrgBoolValue,
                        PluginDoubleValue = OrgDoubleValue * 10,
                        PluginIntegerValue = OrgIntegerValue * 10,
                        PluginLongValue = OrgLongValue * 10,
                        PluginStringValue = OrgStringValue + " abc",
                        PluginEnumValue = OrgEnumValue + 2
                    },
                    new TestPluginConfig2
                    {
                        PluginBoolValue = ! OrgBoolValue,
                        PluginDoubleValue = OrgDoubleValue * -1,
                        PluginIntegerValue = OrgIntegerValue * -1,
                        PluginLongValue = OrgLongValue * -1,
                        PluginStringValue = OrgStringValue + " xyz",
                        PluginEnumValue = OrgEnumValue + 3
                    }
                }

            };

            _configManager.SaveConfiguration(config);

            Console.WriteLine("Starting HeartOfGold");

            bool started = _hogController.StartHeartOfGold();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            _hogController.CreateClients();

            bool result = _hogController.WaitForService("TestModule", ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running'");
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(HogHelper.ConfigDir, true);

            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Killing HeartOfGold");
                _hogController.KillHeartOfGold();

                Thread.Sleep(1000);

                Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
            }
        }

        public void Dispose()
        {
            if (_hogController != null)
            {
                _hogController.Dispose();
                _hogController = null;
            }
        }

        [Test]
        public void BasicConfigTest()
        {
            var moduleName = "TestModule";
            Config config = _hogController.GetConfig(moduleName);

            Entry integerEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "IntegerValue");
            Entry doubleEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "DoubleValue");
            Entry stringEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "StringValue");
            Entry longEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "LongValue");
            Entry boolEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "BoolValue");
            Entry enumEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "EnumValue");
            Entry testPluginEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "TestPlugin");
            Entry pluginsEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "Plugins");

            Assert.IsNotNull(integerEntry, "Can't get property 'IntegerValue' from config.");
            Assert.IsNotNull(doubleEntry, "Can't get property 'DoubleValue' from config.");
            Assert.IsNotNull(stringEntry, "Can't get property 'StringValue' from config.");
            Assert.IsNotNull(longEntry, "Can't get property 'LongValue' from config.");
            Assert.IsNotNull(boolEntry, "Can't get property 'BoolValue' from config.");
            Assert.IsNotNull(enumEntry, "Can't get property 'EnumValue' from config.");
            Assert.IsNotNull(testPluginEntry, "Can't get property 'TestPlugin' from config.");
            Assert.IsNotNull(pluginsEntry, "Can't get property 'Plugins' from config.");

            Assert.AreEqual(2, pluginsEntry.SubEntries.Count, "Number of 'Plugins' does not match.");

            Entry pluginEntry0 = pluginsEntry.SubEntries[0];
            Entry pluginEntry1 = pluginsEntry.SubEntries[1];

            Entry pluginIntegerEntry = testPluginEntry.SubEntries.FirstOrDefault(e => e.Identifier == "PluginIntegerValue");
            Entry pluginDoubleEntry = testPluginEntry.SubEntries.FirstOrDefault(e => e.Identifier == "PluginDoubleValue");
            Entry pluginStringEntry = testPluginEntry.SubEntries.FirstOrDefault(e => e.Identifier == "PluginStringValue");
            Entry pluginLongEntry = testPluginEntry.SubEntries.FirstOrDefault(e => e.Identifier == "PluginLongValue");
            Entry pluginBoolEntry = testPluginEntry.SubEntries.FirstOrDefault(e => e.Identifier == "PluginBoolValue");
            Entry pluginEnumEntry = testPluginEntry.SubEntries.FirstOrDefault(e => e.Identifier == "PluginEnumValue");

            Entry plugin0IntegerEntry = pluginEntry0.SubEntries.FirstOrDefault(e => e.Identifier == "PluginIntegerValue");
            Entry plugin0DoubleEntry = pluginEntry0.SubEntries.FirstOrDefault(e => e.Identifier == "PluginDoubleValue");
            Entry plugin0StringEntry = pluginEntry0.SubEntries.FirstOrDefault(e => e.Identifier == "PluginStringValue");
            Entry plugin0LongEntry = pluginEntry0.SubEntries.FirstOrDefault(e => e.Identifier == "PluginLongValue");
            Entry plugin0BoolEntry = pluginEntry0.SubEntries.FirstOrDefault(e => e.Identifier == "PluginBoolValue");
            Entry plugin0EnumEntry = pluginEntry0.SubEntries.FirstOrDefault(e => e.Identifier == "PluginEnumValue");

            Entry plugin1IntegerEntry = pluginEntry1.SubEntries.FirstOrDefault(e => e.Identifier == "PluginIntegerValue");
            Entry plugin1DoubleEntry = pluginEntry1.SubEntries.FirstOrDefault(e => e.Identifier == "PluginDoubleValue");
            Entry plugin1StringEntry = pluginEntry1.SubEntries.FirstOrDefault(e => e.Identifier == "PluginStringValue");
            Entry plugin1LongEntry = pluginEntry1.SubEntries.FirstOrDefault(e => e.Identifier == "PluginLongValue");
            Entry plugin1BoolEntry = pluginEntry1.SubEntries.FirstOrDefault(e => e.Identifier == "PluginBoolValue");
            Entry plugin1EnumEntry = pluginEntry1.SubEntries.FirstOrDefault(e => e.Identifier == "PluginEnumValue");

            Assert.IsNotNull(pluginIntegerEntry, "Can't get property 'PluginIntegerValue' from config.");
            Assert.IsNotNull(pluginDoubleEntry, "Can't get property 'PluginDoubleValue' from config.");
            Assert.IsNotNull(pluginStringEntry, "Can't get property 'PluginStringValue' from config.");
            Assert.IsNotNull(pluginLongEntry, "Can't get property 'PluginLongValue' from config.");
            Assert.IsNotNull(pluginBoolEntry, "Can't get property 'PluginBoolValue' from config.");
            Assert.IsNotNull(pluginEnumEntry, "Can't get property 'PluginEnumValue' from config.");

            Assert.IsNotNull(plugin0IntegerEntry, "Can't get property 'PluginIntegerValue' from Plugins[0] config.");
            Assert.IsNotNull(plugin0DoubleEntry, "Can't get property 'PluginDoubleValue' from Plugins[0] config.");
            Assert.IsNotNull(plugin0StringEntry, "Can't get property 'PluginStringValue' from Plugins[0] config.");
            Assert.IsNotNull(plugin0LongEntry, "Can't get property 'PluginLongValue' from Plugins[0] config.");
            Assert.IsNotNull(plugin0BoolEntry, "Can't get property 'PluginBoolValue' from Plugins[0] config.");
            Assert.IsNotNull(plugin0EnumEntry, "Can't get property 'PluginEnumValue' from Plugins[0] config.");

            Assert.IsNotNull(plugin1IntegerEntry, "Can't get property 'PluginIntegerValue' from Plugins[1] config.");
            Assert.IsNotNull(plugin1DoubleEntry, "Can't get property 'PluginDoubleValue' from Plugins[1] config.");
            Assert.IsNotNull(plugin1StringEntry, "Can't get property 'PluginStringValue' from Plugins[1] config.");
            Assert.IsNotNull(plugin1LongEntry, "Can't get property 'PluginLongValue' from Plugins[1] config.");
            Assert.IsNotNull(plugin1BoolEntry, "Can't get property 'PluginBoolValue' from Plugins[1] config.");
            Assert.IsNotNull(plugin1EnumEntry, "Can't get property 'PluginEnumValue' from Plugins[1] config.");

            // ReSharper disable SpecifyACultureInStringConversionExplicitly
            Assert.AreEqual(OrgIntegerValue.ToString(), integerEntry.Value.Current, "Property 'IntegerValue' contains unexpected value.");
            Assert.AreEqual(OrgDoubleValue.ToString(), doubleEntry.Value.Current, "Property 'DoubleValue' contains unexpected value.");
            Assert.AreEqual(OrgStringValue, stringEntry.Value.Current, "Property 'StringValue' contains unexpected value.");
            Assert.AreEqual(OrgLongValue.ToString(), longEntry.Value.Current, "Property 'LongValue' contains unexpected value.");
            Assert.AreEqual(OrgBoolValue.ToString(), boolEntry.Value.Current, "Property 'BoolValue' contains unexpected value.");
            Assert.AreEqual(OrgEnumValue.ToString(), enumEntry.Value.Current, "Property 'EnumValue' contains unexpected value.");

            Assert.AreEqual((OrgIntegerValue * 100).ToString(), pluginIntegerEntry.Value.Current, "Property 'PluginIntegerValue' contains unexpected value.");
            Assert.AreEqual((OrgDoubleValue * 100).ToString(), pluginDoubleEntry.Value.Current, "Property 'PluginDoubleValue' contains unexpected value.");
            Assert.AreEqual((OrgStringValue + " asdf"), pluginStringEntry.Value.Current, "Property 'PluginStringValue' contains unexpected value.");
            Assert.AreEqual((OrgLongValue * 100).ToString(), pluginLongEntry.Value.Current, "Property 'PluginLongValue' contains unexpected value.");
            Assert.AreEqual((!OrgBoolValue).ToString(), pluginBoolEntry.Value.Current, "Property 'PluginBoolValue' contains unexpected value.");
            Assert.AreEqual((OrgEnumValue + 1).ToString(), pluginEnumEntry.Value.Current, "Property 'PluginEnumValue' contains unexpected value.");

            Assert.AreEqual((OrgIntegerValue * 10).ToString(), plugin0IntegerEntry.Value.Current, "Property 'PluginIntegerValue' from Plugins[0] contains unexpected value.");
            Assert.AreEqual((OrgDoubleValue * 10).ToString(), plugin0DoubleEntry.Value.Current, "Property 'PluginDoubleValue' from Plugins[0] contains unexpected value.");
            Assert.AreEqual((OrgStringValue + " abc"), plugin0StringEntry.Value.Current, "Property 'PluginStringValue' from Plugins[0] contains unexpected value.");
            Assert.AreEqual((OrgLongValue * 10).ToString(), plugin0LongEntry.Value.Current, "Property 'PluginLongValue' from Plugins[0] contains unexpected value.");
            Assert.AreEqual(OrgBoolValue.ToString(), plugin0BoolEntry.Value.Current, "Property 'PluginBoolValue' from Plugins[0] contains unexpected value.");
            Assert.AreEqual((OrgEnumValue + 2).ToString(), plugin0EnumEntry.Value.Current, "Property 'PluginEnumValue' from Plugins[0] contains unexpected value.");

            Assert.AreEqual((OrgIntegerValue * -1).ToString(), plugin1IntegerEntry.Value.Current, "Property 'PluginIntegerValue' from Plugins[1] contains unexpected value.");
            Assert.AreEqual((OrgDoubleValue * -1).ToString(), plugin1DoubleEntry.Value.Current, "Property 'PluginDoubleValue' from Plugins[1] contains unexpected value.");
            Assert.AreEqual((OrgStringValue + " xyz"), plugin1StringEntry.Value.Current, "Property 'PluginStringValue' from Plugins[1] contains unexpected value.");
            Assert.AreEqual((OrgLongValue * -1).ToString(), plugin1LongEntry.Value.Current, "Property 'PluginLongValue' from Plugins[1] contains unexpected value.");
            Assert.AreEqual((!OrgBoolValue).ToString(), plugin1BoolEntry.Value.Current, "Property 'PluginBoolValue' from Plugins[1] contains unexpected value.");
            Assert.AreEqual((OrgEnumValue + 3).ToString(), plugin1EnumEntry.Value.Current, "Property 'PluginEnumValue' from Plugins[1] contains unexpected value.");

            integerEntry.Value.Current = NewIntegerValue.ToString();
            doubleEntry.Value.Current = NewDoubleValue.ToString();
            stringEntry.Value.Current = NewStringValue;
            longEntry.Value.Current = NewLongValue.ToString();
            boolEntry.Value.Current = NewBoolValue.ToString();
            enumEntry.Value.Current = NewEnumValue.ToString();

            pluginIntegerEntry.Value.Current = (NewIntegerValue * 1000).ToString();
            pluginDoubleEntry.Value.Current = (NewDoubleValue * 1000).ToString();
            pluginStringEntry.Value.Current = NewStringValue + " qwertz";
            pluginLongEntry.Value.Current = (NewLongValue * 1000).ToString();
            pluginBoolEntry.Value.Current = (!NewBoolValue).ToString();
            pluginEnumEntry.Value.Current = (NewEnumValue + 1).ToString();

            plugin0IntegerEntry.Value.Current = (NewIntegerValue * 20).ToString();
            plugin0DoubleEntry.Value.Current = (NewDoubleValue * 20).ToString();
            plugin0StringEntry.Value.Current = NewStringValue + " ABC";
            plugin0LongEntry.Value.Current = (NewLongValue * 20).ToString();
            plugin0BoolEntry.Value.Current = NewBoolValue.ToString();
            plugin0EnumEntry.Value.Current = (NewEnumValue + 2).ToString();

            plugin1IntegerEntry.Value.Current = (NewIntegerValue * -2).ToString();
            plugin1DoubleEntry.Value.Current = (NewDoubleValue * -2).ToString();
            plugin1StringEntry.Value.Current = NewStringValue + " XYZ";
            plugin1LongEntry.Value.Current = (NewLongValue * -2).ToString();
            plugin1BoolEntry.Value.Current = (!NewBoolValue).ToString();
            plugin1EnumEntry.Value.Current = (NewEnumValue + 3).ToString();
            // ReSharper restore SpecifyACultureInStringConversionExplicitly

            _hogController.SetConfig(config, moduleName);

            Thread.Sleep(1000);

            ModuleConfig testConfig = _configManager.GetConfiguration<ModuleConfig>(true);

            Assert.AreEqual(NewIntegerValue, testConfig.IntegerValue, "Property 'IntegerValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewDoubleValue, testConfig.DoubleValue, "Property 'DoubleValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewStringValue, testConfig.StringValue, "Property 'StringValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewLongValue, testConfig.LongValue, "Property 'LongValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewBoolValue, testConfig.BoolValue, "Property 'BoolValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewEnumValue, testConfig.EnumValue, "Property 'EnumValue' from loaded file contains unexpected value.");

            TestPluginConfig testPlugin = testConfig.TestPlugin;

            Assert.IsNotNull(testPlugin, "Property 'TestPlugin' from loaded file is null.");

            Assert.AreEqual(NewIntegerValue * 1000, testPlugin.PluginIntegerValue, "Property 'PluginIntegerValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewDoubleValue * 1000, testPlugin.PluginDoubleValue, "Property 'PluginDoubleValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewStringValue + " qwertz", testPlugin.PluginStringValue, "Property 'PluginStringValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewLongValue * 1000, testPlugin.PluginLongValue, "Property 'PluginLongValue' from loaded file contains unexpected value.");
            Assert.AreEqual(!NewBoolValue, testPlugin.PluginBoolValue, "Property 'PluginBoolValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewEnumValue + 1, testPlugin.PluginEnumValue, "Property 'PluginEnumValue' from loaded file contains unexpected value.");

            Assert.AreEqual(2, testConfig.Plugins.Count, "Number of 'Plugins' from loaded file does not match.");

            TestPluginConfig plugin0 = testConfig.Plugins[0];
            TestPluginConfig plugin1 = testConfig.Plugins[1];

            Assert.AreEqual(NewIntegerValue * 20, plugin0.PluginIntegerValue, "Property 'plugin0.PluginIntegerValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewDoubleValue * 20, plugin0.PluginDoubleValue, "Property 'plugin0.PluginDoubleValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewStringValue + " ABC", plugin0.PluginStringValue, "Property 'plugin0.PluginStringValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewLongValue * 20, plugin0.PluginLongValue, "Property 'plugin0.PluginLongValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewBoolValue, plugin0.PluginBoolValue, "Property 'plugin0.PluginBoolValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewEnumValue + 2, plugin0.PluginEnumValue, "Property 'plugin0.PluginEnumValue' from loaded file contains unexpected value.");

            Assert.AreEqual(NewIntegerValue * -2, plugin1.PluginIntegerValue, "Property 'plugin1.PluginIntegerValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewDoubleValue * -2, plugin1.PluginDoubleValue, "Property 'plugin1.PluginDoubleValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewStringValue + " XYZ", plugin1.PluginStringValue, "Property 'plugin1.PluginStringValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewLongValue * -2, plugin1.PluginLongValue, "Property 'plugin1.PluginLongValue' from loaded file contains unexpected value.");
            Assert.AreEqual(!NewBoolValue, plugin1.PluginBoolValue, "Property 'plugin1.PluginBoolValue' from loaded file contains unexpected value.");
            Assert.AreEqual(NewEnumValue + 3, plugin1.PluginEnumValue, "Property 'plugin1.PluginEnumValue' from loaded file contains unexpected value.");
        }
    }
}
