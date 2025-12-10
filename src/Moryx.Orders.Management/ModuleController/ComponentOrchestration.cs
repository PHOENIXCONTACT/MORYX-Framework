// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.Orders.Assignment;
using Moryx.Orders.Dispatcher;
using Moryx.Orders.Management.Advice;
using Moryx.Orders.Management.Assignment;

namespace Moryx.Orders.Management
{
    [Component(LifeCycle.Singleton)]
    internal class ComponentOrchestration : IAsyncPlugin
    {
        #region Dependencies

        public IProductAssignment ProductAssignment { get; set; }

        public IPartsAssignment PartsAssignment { get; set; }

        public IRecipeAssignment RecipeAssignment { get; set; }

        public IOperationDispatcher OperationDispatcher { get; set; }

        public IJobHandler JobHandler { get; set; }

        public IOperationValidation OperationValidation { get; set; }

        public IOperationAssignment OperationAssignment { get; set; }

        public IOperationDataPool OperationDataPool { get; set; }

        public IEffortCalculator EffortCalculator { get; set; }

        public IDocumentLoader DocumentLoader { get; set; }

        public IAdviceManager AdviceManager { get; set; }

        public ModuleConfig Config { get; set; }

        #endregion

        public async Task StartAsync()
        {
            // --Initialize Assignment
            await ProductAssignment.InitializeAsync(Config.ProductAssignment);
            await PartsAssignment.InitializeAsync(Config.PartsAssignment);
            await RecipeAssignment.InitializeAsync(Config.RecipeAssignment);
            await OperationValidation.InitializeAsync(Config.OperationValidation);
            await DocumentLoader.InitializeAsync(Config.Documents.DocumentLoader);

            // --Initialize dispatcher
            await OperationDispatcher.InitializeAsync(Config.OperationDispatcher);

            // --Start Advice
            await AdviceManager.StartAsync();

            // --Start Assignment
            await ProductAssignment.StartAsync();
            await PartsAssignment.StartAsync();
            await RecipeAssignment.StartAsync();
            await OperationValidation.StartAsync();
            await DocumentLoader.StartAsync();
            OperationAssignment.Start();

            // --Start Job handler and dispatcher
            await OperationDispatcher.StartAsync();
            JobHandler.Start();

            // --Start pool
            await OperationDataPool.StartAsync();

            // --Start Effort Calculator
            EffortCalculator.Start();
        }

        public async Task StopAsync()
        {
            // --Stop Effort Calculator
            EffortCalculator.Stop();

            // --Stop Job handler and dispatcher
            JobHandler.Stop();
            await OperationDispatcher.StopAsync();

            // --Stop Assignment
            OperationAssignment.Stop();
            await DocumentLoader.StopAsync();
            await OperationValidation.StopAsync();
            await RecipeAssignment.StopAsync();
            await PartsAssignment.StopAsync();
            await ProductAssignment.StopAsync();

            // --Stop Advice
            await AdviceManager.StopAsync();

            // --Stop pool
            await OperationDataPool.StopAsync();
        }
    }
}

