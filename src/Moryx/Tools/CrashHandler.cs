// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools
{
    /// <summary>
    /// Static helper to attach to the unhandled exception event
    /// </summary>
    internal static class CrashHandler
    {
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
