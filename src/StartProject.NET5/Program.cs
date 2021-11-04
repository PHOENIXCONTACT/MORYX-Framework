// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.ClientFramework.Kernel;

namespace StartProject
{
    /// <summary>
    /// Main kernel class to load the overall client framework.
    /// Will instantiate the container and will start the whole lifecycle.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main start point of this application
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            var hol = new HeartOfLead(args);
            hol.Initialize();

            // Start Application
            hol.Start();
        }
    }
}
