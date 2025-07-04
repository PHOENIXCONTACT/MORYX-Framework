// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.IO;

namespace Moryx.Tools
{
    /// <summary>
    /// Static helper to attach to the unhandled exception event
    /// </summary>
    public static class CrashHandler
    {
        /// <summary>
        /// Handle an uncaught app domain exception by writing a last crash report
        /// </summary>
        public static void HandleCrash(object sender, UnhandledExceptionEventArgs e)
        {
            // Build crash Text
            string crashText;
            try
            {
                crashText = ExceptionPrinter.Print((Exception) e.ExceptionObject);
            }
            catch
            {
                var ex = e.ExceptionObject as Exception;
                crashText = "Someone actually threw an exception in the exception: \n" +
                            $"  Type of trojan exception: {e.ExceptionObject.GetType()}\n" +
                            $"  Original stack trace: \n{(ex == null ? "Exception does not inherit System.Exception" : ex.StackTrace)}";
            }

            // First dump to console
            Console.WriteLine(crashText);

            // Dump to file
            WriteErrorToFile(crashText);

            if (e.IsTerminating && !Debugger.IsAttached)
            {
                Console.ReadLine();
                Environment.Exit(2);
            }
        }

        /// <summary>
        /// Write an exception message to a file in the CrashLogs directory
        /// </summary>
        public static void WriteErrorToFile(string message)
        {
            var crashDir = Path.Combine(Directory.GetCurrentDirectory(), @"CrashLogs");
            var fileName = $@"{crashDir}\CrashLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

            if (!Directory.Exists(crashDir))
                Directory.CreateDirectory(crashDir);

            // Try to write to directory
            try
            {
                File.WriteAllText(fileName, message);
            }
            catch
            {
               // Kill corrupted directory
               Directory.Delete(crashDir, true);
               Directory.CreateDirectory(crashDir);

               // Now try again
               File.WriteAllText(fileName, message);
            }
        }
    }
}
