using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Default implementation of IWorkplan
    /// </summary>
    [DataContract]
    public class Workplan : IWorkplan, IPersistentObject
    {
        /// <summary>
        /// Create a new workplan instance
        /// </summary>
        public Workplan() : this(new List<IConnector>(), new List<IWorkplanStep>())
        {
        }

        /// <summary>
        /// Private constructor used for new and restored workplans
        /// </summary>
        private Workplan(List<IConnector> connectors, List<IWorkplanStep> steps)
        {
            _connectors = connectors;
            _steps = steps;
        }

        /// <see cref="IWorkplan"/>
        public long Id { get; set; }

        ///<see cref="IWorkplan"/>
        public string Name { get; set; }

        ///<see cref="IWorkplan"/>
        public int Version { get; set; }

        ///<see cref="IWorkplan"/>
        public WorkplanState State { get; set; }

        /// <summary>
        /// Current biggest id in the workplan
        /// </summary>
        public int MaxElementId { get; set; }

        /// <summary>
        /// Editable list of connectors
        /// </summary>
        [DataMember]
        private List<IConnector> _connectors;

        /// <see cref="IWorkplan"/>
        public IEnumerable<IConnector> Connectors { get { return _connectors; } }

        /// <summary>
        /// Add a range of connectors to the workplan
        /// </summary>
        public void Add(params IConnector[] connectors)
        {
            foreach (var connector in connectors)
            {
                connector.Id = ++MaxElementId;
                _connectors.Add(connector);
            }
        }
        /// <summary>
        /// Add a range of connectors to the workplan
        /// </summary>
        public bool Remove(IConnector connector)
        {
            return _connectors.Remove(connector);
        }

        /// <summary>
        /// Editable list of steps
        /// </summary>
        [DataMember]
        private List<IWorkplanStep> _steps;

        /// <see cref="IWorkplan"/>
        public IEnumerable<IWorkplanStep> Steps { get { return _steps; } }

        /// <summary>
        /// Add a range of steps to the workplan
        /// </summary>
        public void Add(params IWorkplanStep[] steps)
        {
            foreach (var step in steps)
            {
                step.Id = ++MaxElementId;
                _steps.Add(step);
            }
        }
        /// <summary>
        /// Add a range of connectors to the workplan
        /// </summary>
        public bool Remove(IWorkplanStep step)
        {
            return _steps.Remove(step);
        }

        /// <summary>
        /// Create an empty plan with start and end
        /// </summary>
        internal static Workplan EmptyPlan()
        {
            var emptyPlan = new Workplan();
            emptyPlan.Add(new Connector {Name = "Start", Classification = NodeClassification.Start});
            emptyPlan.Add(new Connector { Name = "End", Classification = NodeClassification.End });

            return emptyPlan;
        }

        /// <summary>
        /// Restore a workplan with a list of connectors and steps
        /// </summary>
        public static Workplan Restore(List<IConnector> connectors, List<IWorkplanStep> steps)
        {
            return new Workplan(connectors, steps);
        }
    }
}