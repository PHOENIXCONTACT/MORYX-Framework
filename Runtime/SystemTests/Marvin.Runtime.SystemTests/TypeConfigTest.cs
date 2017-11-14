using System;
using System.Linq;
using System.Threading;
using Marvin.Configuration;
using Marvin.Runtime.Kernel;
using Marvin.Runtime.Modules;
using Marvin.Serialization;
using Marvin.TestModule;
using Marvin.TestTools.SystemTest;
using Marvin.TestTools.SystemTest.Maintenance;
using Marvin.Tools.Wcf;
using NUnit.Framework;

namespace Marvin.Runtime.SystemTests
{
    /// <summary>
    /// These tests shall check two aspects: They shall verify the HoG functionality but also wether the HeartOfGoldController is working as expected.
    /// </summary>
    [TestFixture]
    public class TypeConfigTest : IDisposable
    {
        private ConfigManager _configManager;
        private HeartOfGoldController _hogController;

        private readonly Type[] _possibleTypes =
        {
            typeof(TestPluginConfig), 
            typeof(TestPluginConfig1), 
            typeof(TestPluginConfig2)
        };

        private readonly Type[] _possibleSubTypes =
        {
            typeof(TestSubPluginConfig), 
            typeof(TestSubPluginConfig1), 
            typeof(TestSubPluginConfig2)
        };

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            HogHelper.CopyTestModule("Marvin.TestModule.dll");

            _configManager = new RuntimeConfigManager
            {
                ConfigDirectory = HogHelper.ConfigDir
            };

            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 120
            };

            ModuleConfig config = new ModuleConfig
            {
                Config = new WcfConfig()
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

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Killing HeartOfGold");
                _hogController.Process.Kill();

                Thread.Sleep(1000);

                Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
            }

            HogHelper.DeleteTestModules();
        }

        public void Dispose()
        {
            if (_hogController == null) 
                return;

            _hogController.Dispose();
            _hogController = null;
        }

        [Test]
        public void TestProvidedTypes()
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry testPluginEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "TestPlugin");
            Entry pluginsEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "Plugins");

            Assert.IsNotNull(testPluginEntry, "Can't get property 'TestPlugin' from config.");
            Assert.IsNotNull(pluginsEntry, "Can't get property 'Plugins' from config.");

            Assert.AreEqual(3, testPluginEntry.Value.Possible.Length, "Number of possible types does not match.");
            Assert.AreEqual(3, pluginsEntry.Value.Possible.Length, "Number of possible types does not match.");

            foreach (Type type in _possibleTypes)
            {
                Assert.NotNull(testPluginEntry.Value.Possible.FirstOrDefault(p => p == type.Name), "Type '{0}' not found in 'TestPlugin.Value.Possible'.", type.Name);
                Assert.NotNull(pluginsEntry.Value.Possible.FirstOrDefault(p => p == type.Name), "Type '{0}' not found in 'Plugins.Value.Possible'.", type.Name);
            }
        }

        [Test]
        public void TestProvidedSubTypes()
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry pluginEntry = config.Entries.First(e => e.Key.Identifier == "TestPlugin");

            Entry subPluginEntry  = pluginEntry.SubEntries.FirstOrDefault(e => e.Key.Identifier == "SubPlugin");
            Entry subPluginsEntry = pluginEntry.SubEntries.FirstOrDefault(e => e.Key.Identifier == "SubPlugins");

            Assert.IsNotNull(subPluginEntry, "Can't get property 'TestPlugin' from config.");
            Assert.IsNotNull(subPluginsEntry, "Can't get property 'Plugins' from config.");

            Assert.AreEqual(3, subPluginEntry.Value.Possible.Length, "Number of possible types does not match.");
            Assert.AreEqual(3, subPluginsEntry.Value.Possible.Length, "Number of possible types does not match.");

            foreach (Type type in _possibleSubTypes)
            {
                Assert.NotNull(subPluginEntry.Value.Possible.FirstOrDefault(p => p == type.Name), "Type '{0}' not found in 'TestPlugin.Value.Possible'.", type.Name);
                Assert.NotNull(subPluginsEntry.Value.Possible.FirstOrDefault(p => p == type.Name), "Type '{0}' not found in 'Plugins.Value.Possible'.", type.Name);
            }
        }

        [TestCase(typeof(TestPluginConfig))]
        [TestCase(typeof(TestPluginConfig1))]
        [TestCase(typeof(TestPluginConfig2))]
        public void TestPluginConfiguration(Type configType)
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry testPluginEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "TestPlugin");

            Assert.IsNotNull(testPluginEntry, "Can't get property 'TestPlugin' from config.");

            Entry replacement = _hogController.ReplaceEntry("TestModule", testPluginEntry, configType.Name);

            Assert.IsNotNull(replacement, "Can't get new config from server.");
            Assert.AreEqual(configType.Name, replacement.Value.Current, "Type of new config does not match.");

            config.Entries.Remove(testPluginEntry);
            config.Entries.Add(replacement);

            _hogController.SetConfig(config);

            Thread.Sleep(1000);

            ModuleConfig testConfig = _configManager.GetConfiguration<ModuleConfig>(true);

            Assert.AreEqual(configType, testConfig.TestPlugin.GetType(), "Type of TestPlugin config does not match expected type '{0}'", configType);
        }

        [TestCase(typeof(TestSubPluginConfig))]
        [TestCase(typeof(TestSubPluginConfig1))]
        [TestCase(typeof(TestSubPluginConfig2))]
        public void TestSubPluginConfiguration(Type configType)
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry pluginEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "TestPlugin");

            Assert.IsNotNull(pluginEntry, "Can't get property 'TestPlugin' from config.");

            Entry subPluginEntry = pluginEntry.SubEntries.FirstOrDefault(e => e.Key.Identifier == "SubPlugin");

            Assert.IsNotNull(subPluginEntry, "Can't get property 'SubPlugin' from config.");

            Entry replacement = _hogController.ReplaceEntry("TestModule", subPluginEntry, configType.Name);

            Assert.IsNotNull(replacement, "Can't get new config from server.");
            Assert.AreEqual(configType.Name, replacement.Value.Current, "Type of new config does not match.");

            pluginEntry.SubEntries.Remove(subPluginEntry);
            pluginEntry.SubEntries.Add(replacement);

            _hogController.SetConfig(config);

            Thread.Sleep(1000);

            ModuleConfig testConfig = _configManager.GetConfiguration<ModuleConfig>(true);
            TestPluginConfig pluginConfig = testConfig.TestPlugin;

            Assert.AreEqual(configType, pluginConfig.SubPlugin.GetType(), "Type of SubPlugin config does not match expected type '{0}'", configType);
        }

        [TestCase(typeof(TestPluginConfig))]
        [TestCase(typeof(TestPluginConfig1))]
        [TestCase(typeof(TestPluginConfig2))]
        public void TestReplacePluginsConfiguration(Type configType)
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry pluginsEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "Plugins");

            Assert.IsNotNull(pluginsEntry, "Can't get property 'Plugins' from config.");

            Entry requestedConfig = _hogController.RequestEntry("TestModule", pluginsEntry, configType.Name);

            Assert.IsNotNull(requestedConfig, "Can't get new config from server.");
            Assert.AreEqual(configType.Name, requestedConfig.Value.Current, "Type of new config does not match.");

            pluginsEntry.SubEntries.Clear();

            pluginsEntry.SubEntries.Add(requestedConfig);

            _hogController.SetConfig(config);

            Thread.Sleep(2000);

            ModuleConfig testConfig = _configManager.GetConfiguration<ModuleConfig>(true);

            Assert.AreEqual(1, testConfig.Plugins.Count, "Number of of Plugin configs does not match");

            Assert.AreEqual(configType, testConfig.Plugins[0].GetType(), "Type of Plugin config does not match expected type '{0}'", configType);
        }

        [TestCase(typeof(TestSubPluginConfig))]
        [TestCase(typeof(TestSubPluginConfig1))]
        [TestCase(typeof(TestSubPluginConfig2))]
        public void TestReplaceSubPluginsConfiguration(Type configType)
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry pluginEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "TestPlugin");

            Assert.IsNotNull(pluginEntry, "Can't get property 'TestPlugin' from config.");

            Entry subPluginsEntry = pluginEntry.SubEntries.FirstOrDefault(e => e.Key.Identifier == "SubPlugins");

            Assert.IsNotNull(subPluginsEntry, "Can't get property 'Plugins' from config.");

            Entry requestedConfig = _hogController.RequestEntry("TestModule", subPluginsEntry, configType.Name);

            Assert.IsNotNull(requestedConfig, "Can't get new config from server.");
            Assert.AreEqual(configType.Name, requestedConfig.Value.Current, "Type of new config does not match.");

            subPluginsEntry.SubEntries.Clear();

            subPluginsEntry.SubEntries.Add(requestedConfig);

            _hogController.SetConfig(config);

            Thread.Sleep(2000);

            ModuleConfig testConfig = _configManager.GetConfiguration<ModuleConfig>(true);
            TestPluginConfig pluginConfig = testConfig.TestPlugin;

            Assert.AreEqual(1, pluginConfig.SubPlugins.Count, "Number of of SubPlugin configs does not match");

            Assert.AreEqual(configType, pluginConfig.SubPlugins[0].GetType(), "Type of SubPlugin config does not match expected type '{0}'", configType);
        }

        [TestCase(typeof(TestPluginConfig))]
        [TestCase(typeof(TestPluginConfig1))]
        [TestCase(typeof(TestPluginConfig2))]
        public void TestAddPluginsConfiguration(Type configType)
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry pluginsEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "Plugins");

            Assert.IsNotNull(pluginsEntry, "Can't get property 'SubPlugins' from config.");

            int oldCount = pluginsEntry.SubEntries.Count;

            Entry requestedConfig = _hogController.RequestEntry("TestModule", pluginsEntry, configType.Name);

            Assert.IsNotNull(requestedConfig, "Can't get new config from server.");
            Assert.AreEqual(configType.Name, requestedConfig.Value.Current, "Type of new config does not match.");

            pluginsEntry.SubEntries.Add(requestedConfig);

            _hogController.SetConfig(config);

            Thread.Sleep(2000);

            ModuleConfig testConfig = _configManager.GetConfiguration<ModuleConfig>(true);

            Assert.AreEqual(oldCount + 1, testConfig.Plugins.Count, "Number of of SubPlugin configs does not match");

            TestPluginConfig newConfig = testConfig.Plugins.FirstOrDefault(c => c.GetType() == configType);

            Assert.NotNull(newConfig, "New config of type '{0}' not found.", configType);
        }

        [TestCase(typeof(TestSubPluginConfig))]
        [TestCase(typeof(TestSubPluginConfig1))]
        [TestCase(typeof(TestSubPluginConfig2))]
        public void TestAddSubPluginsConfiguration(Type configType)
        {
            Config config = _hogController.GetConfig("TestModule");

            Entry pluginEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "TestPlugin");

            Assert.IsNotNull(pluginEntry, "Can't get property 'TestPlugin' from config.");

            Entry subPluginsEntry = pluginEntry.SubEntries.FirstOrDefault(e => e.Key.Identifier == "SubPlugins");

            Assert.IsNotNull(subPluginsEntry, "Can't get property 'SubPlugins' from config.");

            int oldCount = subPluginsEntry.SubEntries.Count;

            Entry requestedConfig = _hogController.RequestEntry("TestModule", subPluginsEntry, configType.Name);

            Assert.IsNotNull(requestedConfig, "Can't get new config from server.");
            Assert.AreEqual(configType.Name, requestedConfig.Value.Current, "Type of new config does not match.");

            subPluginsEntry.SubEntries.Add(requestedConfig);

            _hogController.SetConfig(config);

            Thread.Sleep(2000);

            ModuleConfig testConfig = _configManager.GetConfiguration<ModuleConfig>(true);
            TestPluginConfig pluginConfig = testConfig.TestPlugin;

            Assert.AreEqual(oldCount + 1, pluginConfig.SubPlugins.Count, "Number of of SubPlugin configs does not match");

            TestSubPluginConfig newConfig = pluginConfig.SubPlugins.FirstOrDefault(c => c.GetType() == configType);

            Assert.NotNull(newConfig, "New config of type '{0}' not found.", configType);
        }

        /// <summary>
        /// This test was implemented because of a reported error that is allready fixed.
        /// To test error detection replace the GetDiveTypes method within the ConfigTransformer utils
        /// with the old faulty code.
        /// </summary>
        /// <example>
        /// var diveTypes = new List<!--Type-->();
        /// if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
        /// {
        ///     var subCompAtt = prop.GetCustomAttribute<!--PossibleConfigValuesAttribute-->();
        ///     if (subCompAtt == null)
        ///         diveTypes.Add(prop.PropertyType.GenericTypeArguments.First());
        ///     else
        ///     {
        ///         var configNames = _transformationProvider.ResolvePossibleValues(subCompAtt);
        ///         diveTypes.AddRange(configNames.Select(confName => _transformationProvider.ConvertValue(subCompAtt, confName).GetType()));
        ///     }
        /// }
        /// else
        ///     diveTypes.Add(prop.PropertyType);
        ///
        /// return diveTypes;
        /// </example>
        [Test]
        public void TestCollectionOnReplacedType()
        {
            // Copied code from replacement
            // TODO: Clean up this messy system test
            Config config = _hogController.GetConfig("TestModule");

            Entry pluginEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "AnotherPlugin");

            Assert.IsNotNull(pluginEntry, "Can't get property 'TestPlugin' from config.");

            Entry replacement = _hogController.ReplaceEntry("TestModule", pluginEntry, typeof(AnotherPluginConfig2).Name);

            Assert.IsNotNull(replacement, "Can't get new config from server.");
            Assert.AreEqual(typeof(AnotherPluginConfig2).Name, replacement.Value.Current, "Type of new config does not match.");

            config.Entries.Remove(pluginEntry);
            config.Entries.Add(replacement);

            _hogController.SetConfig(config);

            Thread.Sleep(1000);

            // Try to get collection for replaced entry
            config = _hogController.GetConfig("TestModule");

            pluginEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "AnotherPlugin");

            Assert.IsNotNull(pluginEntry, "Can't get property 'TestPlugin' from config.");

            Entry subConfigs = pluginEntry.SubEntries.FirstOrDefault(e => e.Key.Identifier == "SubConfigs");

            Assert.IsNotNull(subConfigs, "Can't get order source property on config.");

            replacement = _hogController.RequestEntry("TestModule", subConfigs, typeof(AnotherSubConfig2).Name);

            Assert.IsNotNull(replacement, "Failed to get collection element for derived config instance");
        }
    }
}