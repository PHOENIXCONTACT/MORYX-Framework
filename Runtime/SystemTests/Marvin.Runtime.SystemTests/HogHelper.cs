using System;
using System.IO;
using Marvin.TestTools.SystemTest;

namespace Marvin.Runtime.SystemTests
{
    public class HogHelper
    {
        public HeartOfGoldController HogController { get; set; }

        /// <summary>
        /// Gets the base dir.
        /// </summary>
        /// <value>
        /// The base dir.
        /// </value>
        public static string BaseDir
        {
            // The "WORKSPACE"-Variable is defined on the Jenkins buildserver to find the path of the runtime executable.
            // This will use the variable if it is defined or it uses the relative path from this solution to the "Build"-Directory.
            get { return Environment.GetEnvironmentVariable("WORKSPACE") ?? @"..\..\..\..\..\"; }
        }

        public static string RuntimeDir
        {
            get { return Path.Combine(BaseDir, "Runtime", "Build", "ServiceRuntime"); }
        }

        public static string ModulesDir
        {
            get { return Path.Combine(RuntimeDir, ""); }
        }

        public static string ModelsDir
        {
            get { return Path.Combine(RuntimeDir, ""); }
        }

        public static string ConfigDirParam
        {
            get { return Path.Combine("Config", "SystemTests"); }
        }

        public static string ConfigDir
        {
            get { return Path.Combine(RuntimeDir, ConfigDirParam); }
        }

        public static string TestModuleDir
        {
            get { return Path.Combine(BaseDir, "Runtime", "Build", "SystemTests"); }
        }

        public static string TestModelDir
        {
            get { return Path.Combine(BaseDir, "Toolkit", "Build", ""); }
        }

        public static void CopyTestModules()
        {
            CopyTestModule("Marvin.DependentTestModule.dll");
            CopyTestModule("Marvin.TestModule.dll");
        }

        public static void CopyTestModule(string module)
        {
            File.Copy(Path.Combine(TestModuleDir, module), Path.Combine(ModulesDir, module), true);
        }

        public static void CopyTestModels()
        {
            CopyTestModel("Marvin.TestTools.Test.Model.dll");
        }

        public static void CopyTestModel(string module)
        {
            File.Copy(Path.Combine(TestModelDir, module), Path.Combine(ModelsDir, module), true);
        }

        public static void DeleteTestModules()
        {
            DeleteTestModule("Marvin.DependentTestModule.dll");
            DeleteTestModule("Marvin.TestModule.dll");
        }

        public static void DeleteTestModule(string module)
        {
            File.Delete(Path.Combine(ModulesDir, module));
        }

        public static void DeleteTestModels()
        {
            DeleteTestModel("Marvin.Test.Model.dll");
        }

        public static void DeleteTestModel(string module)
        {
            File.Delete(Path.Combine(ModelsDir, module));
        }

        /// <summary>
        /// Initializes the current instance.
        /// </summary>
        /// <param name="configDir"></param>
        public void Initialize(string configDir)
        {
            HogController = new HeartOfGoldController
            {
                RuntimeDir = RuntimeDir,
                ConfigDir = configDir
            };
        }

        /// <summary>
        /// Starts the HeartOfGold executable
        /// </summary>
        public bool StartHeartOfGold(String service)
        {
            if (HogController.StartHeartOfGold())
            {
                return HogController.WaitForService(service);
            }

            return false;
        }

        /// <summary>
        /// Starts the HeartOfGold executable and waits for the maintenance service.
        /// </summary>
        public bool StartHeartOfGold()
        {
            return StartHeartOfGold("Maintenance");
        }

    }
}