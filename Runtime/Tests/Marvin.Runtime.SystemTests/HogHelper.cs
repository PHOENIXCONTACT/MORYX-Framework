using System;
using System.IO;
using Marvin.TestTools.SystemTest;
using NUnit.Framework;

namespace Marvin.Runtime.SystemTests
{
    public class HogHelper
    {
        public HeartOfGoldController HogController { get; set; }

        public static string BaseDir => TestContext.CurrentContext.TestDirectory + @"..\..\..\..\..\..\";

        public static string RuntimeDir => Path.Combine(BaseDir, "Build", "ServiceRuntime");

        public static string ConfigDirParam => Path.Combine("Config", "SystemTests");

        public static string ConfigDir => Path.Combine(RuntimeDir, ConfigDirParam);

        public static string TestModuleDir => Path.Combine(BaseDir, "Build", "SystemTests");

        public static void CopyTestModules()
        {
            CopyAssembly("Marvin.DependentTestModule.dll");
            CopyAssembly("Marvin.TestModule.dll");
        }

        public static void CopyAssembly(string module)
        {
            File.Copy(Path.Combine(TestModuleDir, module), Path.Combine(RuntimeDir, module), true);
        }

        public static void RemoveAssembly(string module)
        {
            File.Delete(Path.Combine(RuntimeDir, module));
        }

        public static void DeleteTestModules()
        {
            RemoveAssembly("Marvin.DependentTestModule.dll");
            RemoveAssembly("Marvin.TestModule.dll");
        }

        /// <summary>
        /// Starts the HeartOfGold executable
        /// </summary>
        public bool StartHeartOfGold(string service)
        {
            if (HogController.StartHeartOfGold())
            {
                return HogController.WaitForService(service);
            }

            return false;
        }
    }
}