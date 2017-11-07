using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.HeartOfGold
{
    internal static class CommandHelper
    {
        internal static IServerModule GetByName(IModuleManager moduleManager, string name)
        {
            return moduleManager.AllModules.FirstOrDefault(item => String.Equals(item.Name.Replace(" ", string.Empty), name,
                                                                                 StringComparison.CurrentCultureIgnoreCase));
        }

        internal static ICollection<string> MatchingNames(IModuleManager moduleManager, string fragment)
        {
            return moduleManager.AllModules.Select(m => m.Name)
                                .Where(name => name.StartsWith(fragment, StringComparison.CurrentCultureIgnoreCase))
                                .ToList();
        }

        private static readonly InputLink _headInput = new InputLink(string.Empty);

        internal static string ReadCommand(IModuleManager moduleManager)
        {
            var index = 0;
            var input = string.Empty;
            var autoComplete = string.Empty;

            var currentLink = _headInput;

            var reading = true;
            while (reading)
            {
                var newChar = char.MinValue;
                var pressedKey = Console.ReadKey(true);
                switch (pressedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        RemoveChars(input.Length);
                        currentLink = currentLink.Previous;
                        input = currentLink.Input;
                        Console.Write(input);
                        break;
                    case ConsoleKey.DownArrow:
                        RemoveChars(input.Length);
                        currentLink = currentLink.Next;
                        input = currentLink.Input;
                        Console.Write(input);
                        break;
                    case ConsoleKey.Backspace:
                        if (string.IsNullOrEmpty(input))
                            break;
                        if (string.IsNullOrEmpty(autoComplete))
                        {
                            RemoveChars(1);
                            input = input.Substring(0, input.Length - 1);
                        }
                        else
                        {
                            RemoveChars(autoComplete.Length);
                            autoComplete = string.Empty;
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        input += autoComplete;
                        autoComplete = string.Empty;
                        newChar = ' ';
                        break;
                    case ConsoleKey.Tab:
                        // Get matches for current input
                        var current = input.Split(' ').Last();
                        var matches = MatchingNames(moduleManager, current);
                        if (!matches.Any())
                            break;

                        // Iterate through matches
                        if (index >= matches.Count)
                            index = 0;
                        var match = matches.ElementAt(index++);

                        // Move cursor to real position
                        RemoveChars(autoComplete.Length);
                        autoComplete = match.Remove(0, current.Length);
                        Console.Write(autoComplete);
                        break;
                    case ConsoleKey.Enter:
                        if (!string.IsNullOrEmpty(autoComplete))
                            input += autoComplete;
                        reading = false;
                        break;
                    default:
                        newChar = pressedKey.KeyChar;
                        index = 0;
                        break;
                }
                if (newChar == char.MinValue)
                    continue;

                input += newChar;
                Console.Write(newChar);
            }
            Console.WriteLine();

            if (string.IsNullOrEmpty(input) || _headInput.Previous.Input == input)
                return input;

            // Insert link into circular linked list
            var currentPrevious = _headInput.Previous;
            var newLink = new InputLink(input)
            {
                Previous = currentPrevious,
                Next = _headInput
            };
            currentPrevious.Next = newLink;
            _headInput.Previous = newLink;
            return input;
        }

        private static void RemoveChars(int count)
        {
            if (count == 0)
                return;

            Console.CursorLeft = Console.CursorLeft - count;
            Console.Write(string.Empty.PadRight(count));
            Console.CursorLeft = Console.CursorLeft - count;
        }

        internal static void PrintState(ServerModuleState state, bool line = false, int padRight = 0)
        {
            switch (state)
            {
                case ServerModuleState.Running:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ServerModuleState.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ServerModuleState.Failure:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ServerModuleState.Starting:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case ServerModuleState.Initializing:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case ServerModuleState.Ready:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case ServerModuleState.Stopped:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case ServerModuleState.Stopping:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                default:
                    Console.ResetColor();
                    break;

            }

            if (line)
                Console.WriteLine(state.ToString().PadRight(padRight));
            else
                Console.Write(state.ToString().PadRight(padRight));

            Console.ResetColor();
        }

        private class InputLink
        {
            public InputLink(string input)
            {
                Input = input;
                // Per default each link defines its own circular chain
                Previous = Next = this;
            }

            /// <summary>
            /// Previous link in the circular chain
            /// </summary>
            public InputLink Previous { get; set; }

            /// <summary>
            /// Input represented by this link
            /// </summary>
            public string Input { get; }

            /// <summary>
            /// Next link represented by this link
            /// </summary>
            public InputLink Next { get; set; }

            public override string ToString()
            {
                return $"{Previous.Input}->{Input}->{Next.Input}";
            }
        }
    }
}
