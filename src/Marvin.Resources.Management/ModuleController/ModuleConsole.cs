using System;
using System.ComponentModel;
using System.Linq;
using Marvin.AbstractionLayer.Resources;
using Marvin.Runtime.Modules;
using Marvin.Serialization;
using Marvin.Tools;

namespace Marvin.Resources.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        #region Dependencies

        /// <summary>
        /// Factory to create the resource initializers
        /// </summary>
        public IResourceInitializerFactory InitializerFactory { get; set; }

        /// <summary>
        /// Config to load all configured initializers
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }

        /// <summary>
        /// Resource manager to execute initializers
        /// </summary>
        public IResourceManager ResourceManager { get; set; }

        #endregion

        #region Fields

        private IResourceInitializer[] _initializers;

        #endregion

        public string ExportDescription(DescriptionExportFormat format)
        {
            return $"{Environment.NewLine}Possible ResourceManager commands: {Environment.NewLine}" +
                   $"- initialize {Environment.NewLine}";
        }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            if (args.Length <= 0)
            {
                outputStream(ExportDescription(DescriptionExportFormat.Console));
                return;
            }

            switch (args[0].ToLower())
            {
                case "initialize":
                    Initialize(args.Skip(1).ToArray(), outputStream);
                    break;
            }
        }

        private void Initialize(string[] args, Action<string> outputStream)
        {
            // If not executed, create instances of the initializers
            if (_initializers == null)
                _initializers = ModuleConfig.Initializers.Select(config => InitializerFactory.Create(config)).ToArray();

            // If no additional arguments, list initializers
            if (!args.Any())
            {
                ListResourceInitializers(outputStream);
                return;
            }

            // If argument is 'list', list initializers
            if (args[0].ToLower() == "list")
            {
                ListResourceInitializers(outputStream);
                return;
            }

            // Else, parse argument
            outputStream(string.Empty);
            int initializerPos;
            if (int.TryParse(args[0], out initializerPos))
            {
                var index = initializerPos - 1;
                if (index >= _initializers.Length)
                {
                    outputStream($"ResourceInitializer with position {initializerPos} does not exists");
                    ListResourceInitializers(outputStream);
                }

                var initializer = _initializers[initializerPos - 1];
                ExecuteInitializer(initializer, outputStream);
            }
            else
            {
                var name = args[0];
                var initializer = _initializers.SingleOrDefault(i => i.Name.Replace(" ", "-").ToLower().Equals(name.ToLower()));

                if (initializer == null)
                {
                    outputStream($"ResourceInitializer with name '{name}' does not exists");
                    ListResourceInitializers(outputStream);
                }
                else
                {
                    ExecuteInitializer(initializer, outputStream);
                }
            }
        }

        private void ExecuteInitializer(IResourceInitializer initializer, Action<string> outputStream)
        {
            try
            {
                outputStream($"Executing initializer '{initializer.Name}' ...");
                ResourceManager.ExecuteInitializer(initializer);
                outputStream("Successful! Restart the module to load the changes!");
                outputStream(string.Empty);
            }
            catch (Exception e)
            {
                outputStream(ExceptionPrinter.Print(e));
                outputStream("... initialization failed!");
            }
        }

        private void ListResourceInitializers(Action<string> outputStream)
        {
            var maxName = _initializers.Max(i => i.Name.Length) + 2;

            outputStream($"|{"Id",-5}|{"Name".PadRight(maxName)}|Description");
            outputStream($"|-----|{"-".PadRight(maxName, '-')}|----------------");

            for (var index = 0; index < _initializers.Length; index++)
            {
                var initializer = _initializers[index];
                var name = initializer.Name.Replace(" ", "-");

                var line = $"|{index + 1,-5}|{name.PadRight(maxName)}|{initializer.Description}";

                outputStream(line);
            }

            outputStream($"{Environment.NewLine}Execute initializer with: {Environment.NewLine}" +
                         $"- initialize <Id> {Environment.NewLine}" +
                         $"- initialize <Name>{Environment.NewLine}");
        }

        [EditorVisible, DisplayName("Initialize Resource"), Description("Calls the configured resource initializer")]
        public string CallResourceInitializer([PluginConfigs(typeof(IResourceInitializer), true)] ResourceInitializerConfig[] configs)
        {
            foreach (var config in configs)
            {
                try
                {
                    var initializer = InitializerFactory.Create(config);
                    ResourceManager.ExecuteInitializer(initializer);
                    InitializerFactory.Destroy(initializer);
                }
                catch (Exception e)
                {
                    return $"{config.PluginName} failed to run: {e.Message}";
                }
            }

            return "Success";
        }

    }
}