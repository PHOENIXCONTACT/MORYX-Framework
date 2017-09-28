using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// Helper class to build argument object from argument strings
    /// </summary>
    public class RuntimeArguments
    {
        private readonly Dictionary<string, string> _arguments;

        private RuntimeArguments(Dictionary<string, string> arguments)
        {
            _arguments = arguments;
        }

        /// <summary>
        /// Find out if there is an argument in the fragments.
        /// </summary>
        /// <param name="fragments">An amount of fragments which should be checked.</param>
        /// <returns>True when an argument could be found in the fragments.</returns>
        public bool HasArgument(params string[] fragments)
        {
            return fragments.Any(fragment => _arguments.ContainsKey(fragment));
        }

        /// <summary>
        /// Determine if the key exists and has a non null or empty value.
        /// </summary>
        /// <param name="key">The key which should be checked.</param>
        /// <returns>True when the key exists and has a non null or empty value.</returns>
        public bool HasValue(string key)
        {
            return _arguments.ContainsKey(key) && !string.IsNullOrEmpty(_arguments[key]);
        }

        /// <summary>
        /// extension to enable the get of a value to a certain key.
        /// </summary>
        /// <param name="key">The key for which the value should be get.</param>
        /// <returns>The value or null if the key is not present in the dictionary.s</returns>
        public string this[string key]
        {
            get { return _arguments.ContainsKey(key) ? _arguments[key] : null; }
        }

        /// <summary>
        /// Creates a dictionary from the arg input for all arguments which are supported.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>A new runtime arguments instance with the arguments from the params.</returns>
        public static RuntimeArguments BuildArgumentDict(IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            var argumentRegex = new Regex(@"-{1,2}(?<key>\w+)=?(?<value>\S+)?");
            foreach (var argument in args)
            {
                var match = argumentRegex.Match(argument);
                if (!match.Success)
                    continue;

                bool hasValue = match.Groups["value"].Success;
                result[match.Groups["key"].Value] = hasValue ? match.Groups["value"].Value : null;
            }

            return new RuntimeArguments(result);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var argument in _arguments)
            {
                builder.AppendFormat("{0}={1}", argument.Key, argument.Value).AppendLine();
            }
            return builder.ToString();
        }
    }
}