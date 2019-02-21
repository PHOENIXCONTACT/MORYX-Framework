using System.IO;
using Marvin.TestTools.SystemTest;
using NUnit.Framework;

namespace Marvin.Runtime.SystemTests
{
    public class HogHelper
    {
#if !DEBUG
    const string Configuration = "Release";
#else
    const string Configuration = "Debug";
#endif
        public HeartOfGoldController HogController { get; set; }

        public static string RuntimeDir => TestContext.CurrentContext.TestDirectory + @"\..\..\..\..\Marvin.Runtime.Console\bin\" + Configuration;

        public static string ConfigDirParam => Path.Combine("Config", "SystemTests");

        public static string ConfigDir => Path.Combine(RuntimeDir, ConfigDirParam);

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