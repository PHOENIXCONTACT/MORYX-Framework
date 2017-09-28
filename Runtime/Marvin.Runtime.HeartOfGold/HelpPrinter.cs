using System;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// Prints the help of the HoG.
    /// </summary>
    public static class HelpPrinter
    {
        /// <summary>
        /// Prints the help of the HoG.
        /// </summary>
        public static void Print()
        {
            const int padding = 18;

            Console.WriteLine("HeartOfGold - Version: {0}", RuntimePlatform.RuntimeVersion);
            Console.WriteLine("Available commands");
            Console.WriteLine("-c=<ConfigDir>".PadRight(padding) + "Sets the config directory. Default is \"Configs\"");
            Console.WriteLine("-d".PadRight(padding) + "Enforce developer mode. Used by default in Windows console");
            Console.WriteLine("-r=<Runtime>".PadRight(padding) + "Run custom runtime environment");
            Console.WriteLine("-e=<ModuleCount>".PadRight(padding) + "Define number of expected modules. Only used in test mode");
            Console.WriteLine("-f".PadRight(padding) + "Run full test. Only used in test mode");
            Console.WriteLine("-i=<TimeMs>".PadRight(padding) + "Timeout in milliseconds. Only used in test mode");
        }
    }
}
