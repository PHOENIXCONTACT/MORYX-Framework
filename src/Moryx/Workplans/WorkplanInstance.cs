// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Threading;
using Moryx.Workplans.Validation;

namespace Moryx.Workplans
{
    /// <summary>
    /// API access class with factory methods for <see cref="IConnector"/>, <see cref="IWorkplanEngine"/>, 
    /// <see cref="IPathPredictor"/> and <see cref="ITransitionMapper"/>
    /// </summary>
    public static class WorkplanInstance
    {
        /// <summary>
        /// Create a named connector instance of classification intermediate
        /// </summary>
        public static IConnector CreateConnector(string name)
        {
            return CreateConnector(name, NodeClassification.Intermediate);
        }

        /// <summary>
        /// Create a named connector instance
        /// </summary>
        public static IConnector CreateConnector(string name, NodeClassification classification)
        {
            return new Connector { Name = name, Classification = classification };
        }

        /// <summary>
        /// Create the <see cref="WorkplanEngine"/> for a workplan instance and add a <paramref name="context"/>. 
        /// The <see cref="WorkplanInstanceFactory"/> will be used to instantiate the <paramref name="workplan"/>.
        /// </summary>
        public static IWorkplanEngine CreateEngine(IWorkplan workplan, IWorkplanContext context)
        {
            return CreateWorkplanEngine(WorkplanInstanceFactory.Instantiate(workplan, context), context);
        }

        /// <summary>
        /// Create the <see cref="IWorkplanEngine"/> for a given <paramref name="workplanInstance"/>
        /// </summary>
        public static IWorkplanEngine CreateEngine(IWorkplanInstance workplanInstance)
        {
            return CreateWorkplanEngine(workplanInstance, new NullContext());
        }

        /// <summary>
        /// Create the <see cref="WorkplanEngine"/> for a given <paramref name="workplanInstance"/> and add a <paramref name="context"/>
        /// </summary>
        private static WorkplanEngine CreateWorkplanEngine(IWorkplanInstance workplanInstance, IWorkplanContext context)
        {
            var engine = new WorkplanEngine { Context = context };
            engine.Initialize(workplanInstance);
            return engine;
        }

        /// <summary>
        /// Create a path predictor for this workplan that can be used
        /// to monitor instances of the workplan.
        /// </summary>
        public static IPathPredictor PathPrediction(IWorkplan workplan)
        {
            return new PathPredictor(workplan);
        }

        /// <summary>
        /// Destroy a <see cref="IWorkplanEngine"/>
        /// </summary>
        public static void Destroy(IWorkplanEngine engine)
        {
            engine.Dispose();
        }

        /// <summary>
        /// Factory method to create a transition mapper
        /// </summary>
        public static ITransitionMapper TransitionMapper()
        {
            return new TransitionMapper();
        }

        /// <summary>
        /// Factory method to create a transition mapper that executes transitions on new thread
        /// </summary>
        public static ITransitionMapper TransitionMapper(IParallelOperations parallelOperations)
        {
            return new TransitionMapper(parallelOperations);
        }

        /// <summary>
        /// Validate the workplan under different aspects. Aspects can be combined using '|' operator.
        /// </summary>
        /// <param name="workplan">Workplan to validate</param>
        /// <param name="aspects">Enum flag aspects to validate</param>
        /// <returns><remarks>True</remarks> if validation succeeded. Otherwise <remarks>false</remarks>.</returns>
        public static ValidationResult Validate(IWorkplan workplan, ValidationAspect aspects)
        {
            return WorkplanValidation.Validate(workplan, aspects);
        }
    }
}
