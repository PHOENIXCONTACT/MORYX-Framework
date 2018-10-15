using Marvin.Threading;
using Marvin.Workflows.Compiler;
using Marvin.Workflows.Validation;

namespace Marvin.Workflows
{
    /// <summary>
    /// Workflow engine API access class with factory methods
    /// </summary>
    public static class Workflow
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
        /// Create engine instance from workplan and context. The default factory will be used to instantiate
        /// the workflow.
        /// </summary>
        public static IWorkflowEngine CreateEngine(IWorkplan workplan, IWorkplanContext context)
        {
            return CreateWorkflowEngine(WorkflowFactory.Instantiate(workplan, context), context);
        }

        /// <summary>
        /// Create the workflow engine for a given workflow instance
        /// </summary>
        public static IWorkflowEngine CreateEngine(IWorkflow workflow)
        {
            return CreateWorkflowEngine(workflow, new NullContext());
        }

        /// <summary>
        /// Create the workflow engine for a given workflow instance and add context
        /// </summary>
        private static WorkflowEngine CreateWorkflowEngine(IWorkflow workflow, IWorkplanContext context)
        {
            var engine = new WorkflowEngine { Context = context };
            engine.Initialize(workflow);
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
        /// Destroy a workflow engine instance
        /// </summary>
        public static void Destroy(IWorkflowEngine engine)
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
            return WorkflowValidation.Validate(workplan, aspects);
        }

        /// <summary>
        /// Compile a workplan
        /// </summary>
        public static CompiledWorkplan<TStep> Compile<TStep>(IWorkplan workplan, IWorkplanContext context, ICompiler<TStep> compiler)
            where TStep : CompiledTransition
        {
            return WorkplanCompiler.Compile(workplan, context, compiler);
        }
    }
}