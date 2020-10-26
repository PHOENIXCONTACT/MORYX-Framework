// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using Moryx.TestTools.SystemTest;
using NUnit.Framework;

namespace Moryx.Runtime.SystemTests
{
    public class HogHelper
    {
#if !DEBUG
    const string Configuration = "Release";
#else
    const string Configuration = "Debug";
#endif
        public HeartOfGoldController HogController { get; set; }

        public static string RuntimeDir => TestContext.CurrentContext.TestDirectory + @"\..\..\..\..\StartProject\bin\" + Configuration;

        public static string ConfigDirParam => Path.Combine("Config", "SystemTests");

        public static string ConfigDir => Path.Combine(RuntimeDir, ConfigDirParam);

        /// <summary>
        /// Starts the HeartOfGold executable
        /// </summary>
        public bool StartHeartOfGold(string service)
        {
            if (HogController.StartApplication())
            {
                return HogController.WaitForService(service);
            }

            return false;
        }
    }
}
