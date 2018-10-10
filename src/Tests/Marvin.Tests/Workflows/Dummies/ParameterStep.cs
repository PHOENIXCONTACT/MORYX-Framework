using Marvin.Serialization;
using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Tests.Workflows
{
    internal class ParameterStep : WorkplanStepBase, IParameterHolder
    {
        /// 
        public override string Name
        {
            get { return "ParameterStep"; }
        }
        
        [EditorVisible]
        public DummyParameters Parameters { get; set; }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new DummyTransition();
        }

        public DummyParameters Export()
        {
            return Parameters;
        }
    }

    internal class ParameterConstructorStep : WorkplanStepBase, IParameterHolder
    {
        private readonly DummyParameters _parameters;

        /// 
        public override string Name
        {
            get { return "ParameterConstructorStep"; }
        }

        public ParameterConstructorStep(DummyParameters parameters)
        {
            _parameters = parameters;
        }

        ///
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new DummyTransition();
        }

        public DummyParameters Export()
        {
            return _parameters;
        }
    }

    internal interface IParameterHolder
    {
        DummyParameters Export();
    }

    internal class DummyParameters
    {
        [EditorVisible]
        public int Number { get; set; }

        [EditorVisible]
        public string Name { get; set; }
    }
}