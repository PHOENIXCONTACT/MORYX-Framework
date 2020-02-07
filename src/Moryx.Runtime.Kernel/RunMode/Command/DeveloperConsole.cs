// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.InteropServices;
using System.Text;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Console for the developer to debug and monitor the HoG while developing.
    /// </summary>
    [RunMode(typeof(DeveloperConsoleOptions))]
    public class DeveloperConsole : CommandRunMode<DeveloperConsoleOptions>
    {
        /// <summary>
        /// Register necessary controls and initialize the module.
        /// </summary>
        protected override void Boot()
        {
            // Register console control to capture close event on windows machines
            // based on http://stackoverflow.com/questions/474679/capture-console-exit-c-sharp
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Disable close button. The user have to use the 'exit' command
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

                _handler = Handler;
                SetConsoleCtrlHandler(_handler, true);
            }

            // Register to service manager event
            ModuleManager.ModuleStateChanged += OnModuleStateChanged;

            // Welcome message and start
            DrawTitle();
            base.Boot();
        }

        /// <summary>
        /// Runs the text environment. Handle the flow when entered commands.
        /// </summary>
        protected override void RunTextEnvironment()
        {
            // Enter shell mode
            var command = string.Empty;
            while (command != "exit")
            {
                WriteBashPostString();
                command = CommandHelper.ReadCommand(ModuleManager);
                if (string.IsNullOrEmpty(command))
                    continue;

                var parts = command.Split(' ');
                switch (parts[0])
                {
                    case "help": PrintGeneralHelp();
                        break;
                    case "clear": ClearConsole();
                        break;
                    case "exit":
                        break;
                    default: ExecuteCommand(command);
                        break;
                }
            }
        }

        /// <summary>
        /// Shut down application, stops modules, save config.
        /// </summary>
        protected override void ShutDown()
        {
            base.ShutDown();

            ModuleManager.ModuleStateChanged -= OnModuleStateChanged;

            ConfigLoader.SaveAll();
        }

        /// <summary>
        /// Print the text which is given in the params.
        /// </summary>
        /// <param name="lines">The lines which should be printed.</param>
        protected override void PrintText(params string[] lines)
        {
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Print the state change of the module in the console.
        /// </summary>
        /// <param name="sender">Sender object as IServerModule.</param>
        /// <param name="eventArgs">EventArgs with new state.</param>
        protected void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs eventArgs)
        {
            lock (this)
            {
                ClearCurrentConsoleLine();
                var serverModule = (IServerModule)sender;
                Console.Write(serverModule.Name.PadRight(35) + " changed state to".PadRight(30));
                CommandHelper.PrintState(eventArgs.NewState, true);
                WriteBashPostString();
            }
        }

        private void PrintGeneralHelp()
        {
            const int pad = 22;
            Console.WriteLine("Command".PadRight(pad) + "Action");
            foreach (var handler in CommandHandler)
            {
                handler.ExportValidCommands(pad);
                Console.WriteLine();
            }
            Console.WriteLine("clear".PadRight(pad) + "Clears the console and prints the state of all modules");
            Console.WriteLine();
            Console.WriteLine("help".PadRight(pad) + "Prints this page");
            Console.WriteLine();
            Console.WriteLine("exit".PadRight(pad) + "Stops all modules and closes the application");
            Console.WriteLine();
        }

        #region Unmanaged exit code

        #region Console Window Handling

        // ReSharper disable once InconsistentNaming
        private const int MF_BYCOMMAND = 0x00000000;

        // ReSharper disable once InconsistentNaming
        private const int SC_CLOSE = 0xF060;

        /// <summary>
        /// Deletes an item from the specified menu.
        /// If the menu item opens a menu or submenu, this function destroys the
        /// handle to the menu or submenu and frees the memory used by the menu or submenu.
        /// https://msdn.microsoft.com/en-us/library/ms647629(VS.85).aspx
        /// </summary>
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        /// <summary>
        /// Enables the application to access the window menu (also known as the system menu or the control menu)
        /// for copying and modifying.
        /// https://msdn.microsoft.com/en-us/library/ms647985(v=vs.85).aspx
        /// </summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// Retrieves the window handle used by the console associated with the calling process.
        /// https://msdn.microsoft.com/en-us/library/ms683175(v=vs.85).aspx
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        #endregion

        #region Exit Signals

        // Code taken from http://stackoverflow.com/questions/474679/capture-console-exit-c-sharp

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private EventHandler _handler;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private bool Handler(CtrlType sig)
        {
            ConfigLoader.SaveAll();
            ModuleManager.StopModules();
            return false;
        }

        #endregion

        #endregion

        #region Style Methods

        /// <summary>
        /// Draws the title.
        /// </summary>
        private static void DrawTitle()
        {
            DrawStarLine();

            var title = "MORYX Runtime " + RuntimePlatform.RuntimeVersion + " Emulator: " + Platform.Current.ProductName;
            Console.WriteLine("{0," + ((Console.WindowWidth / 2) + title.Length / 2) + "}", title);
            Console.WriteLine();

            const string welcomeMessage = "Welcome to the Runtime emulator.";
            Console.WriteLine("{0," + ((Console.WindowWidth / 2) + welcomeMessage.Length / 2) + "}", welcomeMessage);

            const string helpInfo = "Enter \"help\" for more information!";
            Console.WriteLine("{0," + ((Console.WindowWidth / 2) + helpInfo.Length / 2) + "}", helpInfo);

            DrawStarLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Writes the bash post string.
        /// </summary>
        private static void WriteBashPostString()
        {
            Console.Write("Runtime > ");
        }

        /// <summary>
        /// Clears the console, draws the title and prints the services
        /// </summary>
        private void ClearConsole()
        {
            Console.Clear();
            DrawTitle();
            ExecuteCommand("print");
        }

        /// <summary>
        /// Draws a star line.
        /// </summary>
        private static void DrawStarLine()
        {
            var starLine = new StringBuilder();
            for (var i = 0; i < Console.WindowWidth; i++)
                starLine.Append("*");
            Console.WriteLine(starLine.ToString());
        }

        /// <summary>
        /// Clears the current console line.
        /// </summary>
        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(string.Empty.PadRight(Console.WindowWidth - 1));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        #endregion
    }
}
