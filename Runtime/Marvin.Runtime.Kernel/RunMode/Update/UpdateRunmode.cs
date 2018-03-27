using System;
using Marvin.Model;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel.Update
{
    /// <summary>
    /// Runmode used to update all models
    /// </summary>
    [Runmode(RunModeName)]
    public class UpdateRunmode : IRunmode
    {
        /// <summary>
        /// Const name of the RunMode. 
        /// </summary>
        public const string RunModeName = "DbUpdate";

        /// <summary>
        /// Service manager instance
        /// </summary>
        public IModuleManager ModuleManager { get; set; }

        /// <summary>
        /// Update each model
        /// </summary>
        public IModelConfigurator[] Configurators { get; set; }

        /// <summary>
        /// Setup the environment by passing the command line arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public void Setup(RuntimeArguments args)
        {
        }

        /// <summary>
        /// Run environment
        /// </summary>
        /// <returns>0: All fine - 1: Warning - 2: Error</returns>
        public RuntimeErrorCode Run()
        {
            Console.WriteLine("Updating databases...");
            foreach (var configurator in Configurators)
            {
                try
                {
                    var summary = configurator.MigrateDatabase(configurator.Config);
                    if (!summary.WasUpdated)
                    {
                        Console.WriteLine("No updates for {0}", configurator.TargetModel);
                        continue;
                    }

                    // Display update summary
                    Console.WriteLine("Update summary for {0}:", configurator.TargetModel);
                    foreach (var update in summary.ExecutedUpdates)
                    {
                        Console.WriteLine("{0}->{1}: {2}", update.From, update.To, update.Description);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Update for {0} failed with exception:\n  {1}", configurator.TargetModel, ex.Message);
                }
            }
            Console.WriteLine("Update complete");
            return RuntimeErrorCode.NoError;
        }
    }
}