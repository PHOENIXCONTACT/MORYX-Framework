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

        public IDocumentLoader DocumentLoader { get; set; }

        public IAdviceManager AdviceManager { get; set; }

        public ModuleConfig Config { get; set; }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            // --Initialize Assignment
            await ProductAssignment.InitializeAsync(Config.ProductAssignment, cancellationToken);
            await PartsAssignment.InitializeAsync(Config.PartsAssignment, cancellationToken);
            await RecipeAssignment.InitializeAsync(Config.RecipeAssignment, cancellationToken);
            await OperationValidation.InitializeAsync(Config.OperationValidation, cancellationToken);
            await DocumentLoader.InitializeAsync(Config.Documents.DocumentLoader, cancellationToken);

            // --Initialize dispatcher
            await OperationDispatcher.InitializeAsync(Config.OperationDispatcher, cancellationToken);

            // --Start Advice
            await AdviceManager.StartAsync(cancellationToken);

            // --Start Assignment
            await ProductAssignment.StartAsync(cancellationToken);
            await PartsAssignment.StartAsync(cancellationToken);
            await RecipeAssignment.StartAsync(cancellationToken);
            await OperationValidation.StartAsync(cancellationToken);
            await DocumentLoader.StartAsync(cancellationToken);
            OperationAssignment.Start();

            // --Start Job handler and dispatcher
            await OperationDispatcher.StartAsync(cancellationToken);
            JobHandler.Start();

            // --Start pool
            await OperationDataPool.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            // --Stop Job handler and dispatcher
            JobHandler.Stop();
            await OperationDispatcher.StopAsync(cancellationToken);

            // --Stop Assignment
            OperationAssignment.Stop();
            await DocumentLoader.StopAsync(cancellationToken);
            await OperationValidation.StopAsync(cancellationToken);
            await RecipeAssignment.StopAsync(cancellationToken);
            await PartsAssignment.StopAsync(cancellationToken);
            await ProductAssignment.StopAsync(cancellationToken);

            // --Stop Advice
            await AdviceManager.StopAsync(cancellationToken);

            // --Stop pool
            await OperationDataPool.StopAsync(cancellationToken);
        }
    }
}

