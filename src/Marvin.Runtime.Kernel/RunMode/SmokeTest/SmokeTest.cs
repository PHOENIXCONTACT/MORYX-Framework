using System;
using System.Linq;
using System.Threading;
using System.Timers;
using Marvin.Configuration;
using Marvin.Model;
using Marvin.Notifications;
using Marvin.Runtime.Modules;
using Marvin.Tools;
using Marvin.Tools.Wcf;
using Timer = System.Timers.Timer;

namespace Marvin.Runtime.Kernel.SmokeTest
{
    /// <summary>
    /// Smoke test will start the runtime and try to load the modules, start them and shut them down again.
    /// </summary>
    [RunMode(typeof(SmokeTestOptions))]
    public class SmokeTest : RunModeBase<SmokeTestOptions>
    {
        /// <summary>
        /// ConfigManager to receive the <see cref="WcfConfig"/>
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// All database model factories
        /// </summary>
        public IUnitOfWorkFactory[] ModelFactories { get; set; }

        #region Fields

        private TestStep _currentStep = TestStep.StartAll;

        private readonly ManualResetEvent _awaitStep = new ManualResetEvent(false);
        private Timer _noChangeTimer;

        private bool _failed;
        private bool _timedOut;

        private int _expectedModules;
        private bool _fullTest;

        private int _digits;
        private int _modulesCount;

        #endregion

        /// <summary>
        /// Setup the smoke test with some optional arguments.
        /// </summary>
        /// <param name="args">The argument list for the smoke test.</param>
        public override void Setup(RuntimeOptions args)
        {
            base.Setup(args);

            // Read arguments
            _expectedModules = Options.ExpectedModules;
            _fullTest = Options.FullTest;

            // Override configs port value
            var wcfConfig = ConfigManager.GetConfiguration<WcfConfig>(false);
            wcfConfig.HttpPort += Options.PortIncrement;
            wcfConfig.NetTcpPort += Options.PortIncrement;

            _noChangeTimer = new Timer(Options.NoChangeInterval);
            _noChangeTimer.Elapsed += OperationTimedOut;
        }

        #region Testing

        /// <summary>
        /// Prepare the smoke test. Create all databases for the needed modules, count the modules, delete databases after run of tests.
        /// </summary>
        /// <returns>The result of the tests.</returns>
        public override RuntimeErrorCode Run()
        {
            // Check if all modules were found
            var modulesCount = ModuleManager.AllModules.Count();
            if (_expectedModules > modulesCount)
            {
                // Return error if insufficient number of modules was found
                Console.WriteLine("Number of modules doesn't match! Expected {0} - found {1}", _expectedModules, modulesCount);
                return RuntimeErrorCode.Error;
            }

            // Load model configurators
            var modelConfigurators = ModelFactories.Select(m => ((IModelConfiguratorFactory)m).GetConfigurator()).ToArray();

            // Create all databases
            foreach (var modelConfigurator in modelConfigurators)
            {
                try
                {
                    // Add database config prefix
                    modelConfigurator.Config.Database = $"smokeTest-{modelConfigurator.TargetModel}";
                    modelConfigurator.CreateDatabase(modelConfigurator.Config);
                }
                catch
                {
                    Console.WriteLine("Failed to create data model {0}", modelConfigurator.TargetModel);
                }
            }

            // Register to changed events
            ModuleManager.ModuleStateChanged += OnModuleStateChanged;

            // Determine number of all modules once to avoid constant call to ModuleManager
            // For optical purposes we get the number of digits it takes to display the number of all modules
            _modulesCount = ModuleManager.AllModules.Count();
            _digits = (int)Math.Floor(Math.Log10(_modulesCount)) + 1;

            // Run all tests
            var result = RunTests();

            // Delete all databases
            foreach (var modelConfigurator in modelConfigurators)
            {
                try
                {
                    modelConfigurator.DeleteDatabase(modelConfigurator.Config);
                }
                catch
                {
                    Console.WriteLine("Failed to delete data model {0}", modelConfigurator.TargetModel);
                }
            }

            return result;
        }

        private RuntimeErrorCode RunTests()
        {
            // Start all and await result
            var result = RunTestCase(TestStep.StartAll, state => ModuleManager.StartModules());
            if (!result)
                return RuntimeErrorCode.Error;

            // Start manual modules if any
            var manualStart = ManualStartCallbacks();
            result = !manualStart.Any() || RunTestCase(TestStep.StartManuals, manualStart);
            if (!result)
                return RuntimeErrorCode.Error;

            // Only reincarnate all modules for a full test
            if (_fullTest)
            {
                result = RunTestCase(TestStep.ReincarnateSingle, ModuleManager.AllModules.Select(SingleReincarnate).ToArray());
                if (!result)
                    return RuntimeErrorCode.Error;
            }

            // Stop all modules
            result = RunTestCase(TestStep.StopAll, state => ModuleManager.StopModules());
            if (!result)
                return RuntimeErrorCode.Error;

            Console.WriteLine("System passed all tests");
            if (_expectedModules == _modulesCount)
                return RuntimeErrorCode.NoError;

            Console.WriteLine("Found more modules than expected!");
            return RuntimeErrorCode.Warning;
        }

        private WaitCallback[] ManualStartCallbacks()
        {
            var modules = ModuleManager.AllModules.Where(item => ModuleManager.BehaviourAccess<ModuleStartBehaviour>(item).Behaviour != ModuleStartBehaviour.Auto
                                                                && item.State != ServerModuleState.Running);
            var waitCallbacks = modules.Select(plugin => new WaitCallback(state => ModuleManager.StartModule(plugin)));
            return waitCallbacks.ToArray();
        }

        private WaitCallback SingleReincarnate(IServerModule plugin)
        {
            var callback = new WaitCallback(state =>
            {
                Console.WriteLine("Restarting " + plugin.Name);
                ModuleManager.ReincarnateModule(plugin);
            });
            return callback;
        }

        private bool RunTestCase(TestStep testStep, params WaitCallback[] testRuns)
        {
            if (!TestStepRequired(testStep))
                return true;

            _currentStep = testStep;
            Console.WriteLine("Test step: {0}..", testStep);
            foreach (var testRun in testRuns)
            {
                // Prepare necessary async operations
                _awaitStep.Reset();
                _noChangeTimer.Start();

                // Execute test case
                ThreadPool.QueueUserWorkItem(testRun);
                _awaitStep.WaitOne();

                // Clean up and evaluate result
                Console.WriteLine();
                _noChangeTimer.Stop();
                if (!_failed && !_timedOut)
                    continue;

                if (_timedOut)
                    Console.WriteLine("Test {0} did not report any changes within the last {1}s", testStep, _noChangeTimer.Interval / 1000);
                break;
            }
            if (_failed | _timedOut)
            {
                Console.WriteLine("Test result: failed!");
                Console.WriteLine("Final system state:");
                foreach (var service in ModuleManager.AllModules)
                {
                    Console.WriteLine("Module {0} is in state {1}", service.Name, service.State);
                }
            }
            else
                Console.WriteLine("Test result: success!");
            Console.WriteLine();
            return !_failed && !_timedOut;
        }

        private bool TestStepRequired(TestStep testStep)
        {
            switch (testStep)
            {
                case TestStep.StartAll:
                    return ModuleManager.AllModules.Any(service => ModuleManager.BehaviourAccess<ModuleStartBehaviour>(service).Behaviour == ModuleStartBehaviour.Auto);
                case TestStep.StartManuals:
                    return ModuleManager.AllModules.Any(service => ModuleManager.BehaviourAccess<ModuleStartBehaviour>(service).Behaviour == ModuleStartBehaviour.Manual);
                default:
                    return true;
            }
        }
        #endregion

        #region Event Handler

        private void OperationTimedOut(object sender, ElapsedEventArgs e)
        {
            lock (ModuleManager)
            {
                _timedOut = true;
                _awaitStep.Set();
            }
        }

        private void OnModuleStateChanged(object sender, ModuleStateChangedEventArgs stateChanged)
        {
            // Reset NoChangeStep
            _noChangeTimer.Stop();
            _noChangeTimer.Start();

            // Ignore temporary states
            if (stateChanged.NewState.HasFlag(ServerModuleState.Initializing))
                return;

            // Output state change
            if (stateChanged.NewState == ServerModuleState.Failure)
            {
                var module = (IServerModule)sender;
                Console.WriteLine("Module {0} failed with Exception:\n", module.Name);
                Console.WriteLine(ExceptionPrinter.Print(module.Notifications.OrderBy(n => n.Timestamp).Last(n => n.Severity == Severity.Error).Exception));
                _failed = true;
                _awaitStep.Set();
            }

            lock (ModuleManager)
            {
                // If we allready failed skip output
                if (_failed | _timedOut)
                    return;

                var stepDone = false;
                switch (_currentStep)
                {
                    case TestStep.StartAll:
                        // Check if all auto start modules are on their feet
                        var autoPlugin = ModuleManager.AllModules.Where(item => ModuleManager.BehaviourAccess<ModuleStartBehaviour>(item).Behaviour == ModuleStartBehaviour.Auto).ToArray();
                        var running = autoPlugin.Count(item => item.State == ServerModuleState.Running);
                        var allAuto = autoPlugin.Length;

                        if (running == allAuto)
                            stepDone = true;
                        Console.WriteLine("{0} of {1} auto started modules are running", running.ToString("D").PadLeft(_digits), allAuto);
                        break;
                    case TestStep.StartManuals:
                    case TestStep.ReincarnateSingle:
                        // Check if target service is back running
                        running = ModuleManager.AllModules.Count(item => item.State == ServerModuleState.Running);
                        if (running == _modulesCount)
                            stepDone = true;
                        Console.WriteLine("{0} of {1} modules are running", running.ToString("D").PadLeft(_digits), _modulesCount);
                        break;
                    case TestStep.StopAll:
                        // Check if all auto start modules are stopped
                        var stopped = ModuleManager.AllModules.Count(item => item.State == ServerModuleState.Stopped);
                        if (stopped == _modulesCount)
                            stepDone = true;
                        Console.WriteLine("{0} of {1} modules stopped", stopped.ToString("D").PadLeft(_digits), _modulesCount);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (stepDone)
                    _awaitStep.Set();
            }
        }

        #endregion
    }

    internal enum TestStep
    {
        StartAll,
        StartManuals,
        ReincarnateSingle,
        StopAll
    }
}
