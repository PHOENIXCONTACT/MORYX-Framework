// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Orders.Assignment;
using Moryx.Orders.Dispatcher;
using Moryx.Orders.Management.Advice;
using Moryx.Orders.Management.Assignment;

namespace Moryx.Orders.Management
{
    [Component(LifeCycle.Singleton)]
    internal class ComponentOrchestration
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

        public void Start()
        {
            // --Initialize Assignment
            ProductAssignment.Initialize(Config.ProductAssignment);
            PartsAssignment.Initialize(Config.PartsAssignment);
            RecipeAssignment.Initialize(Config.RecipeAssignment);
            OperationValidation.Initialize(Config.OperationValidation);
            DocumentLoader.Initialize(Config.Documents.DocumentLoader);

            // --Initialize dispatcher
            OperationDispatcher.Initialize(Config.OperationDispatcher);

            // --Start Advice
            AdviceManager.Start();

            // --Start Assignment
            ProductAssignment.Start();
            PartsAssignment.Start();
            RecipeAssignment.Start();
            OperationValidation.Start();
            DocumentLoader.Start();
            OperationAssignment.Start();

            // --Start Job handler and dispatcher
            OperationDispatcher.Start();
            JobHandler.Start();

            // --Start pool
            OperationDataPool.Start();

            // --Start Effort Calculator
            EffortCalculator.Start();
        }

        public void Stop()
        {
            // --Stop Effort Calculator
            EffortCalculator.Stop();

            // --Stop Job handler and dispatcher
            JobHandler.Stop();
            OperationDispatcher.Stop();

            // --Stop Assignment
            OperationAssignment.Stop();
            OperationValidation.Stop();
            RecipeAssignment.Stop();
            PartsAssignment.Stop();
            ProductAssignment.Stop();

            // --Stop Advice
            AdviceManager.Stop();

            // --Stop pool
            OperationDataPool.Stop();
        }
    }
}

