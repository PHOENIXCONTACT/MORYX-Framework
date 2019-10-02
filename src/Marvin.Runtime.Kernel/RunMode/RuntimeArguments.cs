using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Class to build argument object from argument strings
    /// </summary>
    public class RuntimeArguments
    {
        private static IReadOnlyList<string> _allArguments;

        /// <summary>
        /// Directory where configs are saved.
        /// </summary>
        [Option('c', "configDir", Required = false, Default = "Config", HelpText = "Directory where configs are saved.")]
        public string ConfigDir { get; set; }

        /// <summary>
        /// Specify the RunMode to use.
        /// </summary>
        [Option('r', "runMode", Required = false, HelpText = "Specify the RunMode to use.")]
        public string RunMode { get; set; }

        /// <summary>
        /// Argument for starting the database update run mode
        /// </summary>
        [Option('u', "dbUpdate", Required = false, HelpText = "Update all databases with outdated versions.")]
        public bool DbUpdate { get; set; }

        /// <summary>
        /// Starts runtime in developer console
        /// </summary>
        [Option('d', "dev", Required = false, HelpText = "Starts runtime in developer console")]
        public bool DeveloperConsole { get; set; }

        /// <summary>
        /// Creates a dictionary from the arg input for all arguments which are supported.
        /// </summary>
        public static IReadOnlyList<Error> Build(IReadOnlyList<string> args, out RuntimeArguments parsed)
        {
            _allArguments = args;
            return Parse(out parsed, delegate(ParserSettings settings)
            {
                settings.AutoHelp = true;
                settings.HelpWriter = Console.Out;
                settings.AutoVersion = true;
            });
        }

        /// <summary>
        /// Parses a list of arguments to the given options class
        /// </summary>
        public static IEnumerable<Error> Parse<T>(out T parsed) where T : class, new()
        {
            return Parse(out parsed, null);
        }

        /// <summary>
        /// Private parse function
        /// </summary>
        private static IReadOnlyList<Error> Parse<T>(out T parsed, Action<ParserSettings> configuration) where T : class, new()
        {
            var parser = new Parser(delegate(ParserSettings settings)
            {
                settings.AutoVersion = false;
                settings.AutoHelp = false;
                settings.HelpWriter = null;
                configuration?.Invoke(settings);

                settings.IgnoreUnknownArguments = true;
            });

            IEnumerable<Error> parseErrors = null;
            T parsedOptions = null;

            parser.ParseArguments<T>(_allArguments)
                .WithParsed(options => parsedOptions = options)
                .WithNotParsed(errors => parseErrors = errors);

            parser.Dispose();

            parsed = parsedOptions;
            return parseErrors != null ? parseErrors.ToArray() : new Error[0];
        }
    }
}