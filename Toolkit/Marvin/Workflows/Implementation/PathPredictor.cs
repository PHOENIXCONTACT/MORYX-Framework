using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Marvin.Tools;

namespace Marvin.Workflows
{
    /// <summary>
    /// Implementation of <see cref="IPathPredictor"/>
    /// </summary>
    internal class PathPredictor : IPathPredictor
    {
        /// <summary>
        /// The analyzed workplans with the possible results for each path
        /// </summary>
        private readonly Dictionary<long, PossibleResults> _workplanAnalysis;

        /// <summary>
        /// Engines monitored by this path predictor
        /// </summary>
        private readonly ICollection<IMonitoredEngine> _monitoredEngines = new List<IMonitoredEngine>();

        /// <inheritdoc />
        public int MonitoredEngines => _monitoredEngines.Count;

        public PathPredictor(IWorkplan workplan)
        {
            // All inputs for each output to traverse the workplan in reverse
            var reversedRelations = ReverseRelations(workplan);
            _workplanAnalysis = reversedRelations.Keys.ToDictionary(key => key, key => new PossibleResults(key));

            foreach (var connector in workplan.Connectors.Where(c => c.Classification.HasFlag(NodeClassification.Exit)))
            {
                Analyze(connector, connector, reversedRelations);
            }
        }

        /// <summary>
        /// Reverse the relations from outputs to inputs to traverse the workplan
        /// in reverse and identify all possible path leading to a certain result
        /// </summary>
        /// <param name="workplan"></param>
        /// <returns></returns>
        private static IDictionary<long, HashSet<IConnector>> ReverseRelations(IWorkplan workplan)
        {
            // Create dictionary of connector ids and possible inputs
            var reversedConnections = workplan.Connectors.ToDictionary(c => c.Id, c => new HashSet<IConnector>());

            foreach (var step in workplan.Steps)
            {
                foreach (var output in step.Outputs)
                {
                    reversedConnections[output.Id].AddRange(step.Inputs);
                }
            }

            return reversedConnections;
        }

        /// <summary>
        /// Analyze the current node and build the internal path prediction dictionary
        /// </summary>
        private void Analyze(IConnector currentNode, IConnector resultNode, IDictionary<long, HashSet<IConnector>> reversedRelations)
        {
            // Try to add the result to this node. if this fails we already visited this node
            // and have a reached a recursion
            if (_workplanAnalysis[currentNode.Id].AddResult(resultNode))
            {
                // Continue recursively
                foreach (var input in reversedRelations[currentNode.Id])
                    Analyze(input, resultNode, reversedRelations);
            }
        }

        public void Monitor(IWorkflowEngine instance)
        {
            lock (_monitoredEngines)
            {
                var monitoredEngine = (IMonitoredEngine)instance;
                _monitoredEngines.Add(monitoredEngine);

                monitoredEngine.PlaceReached += OnPlaceReached;
                monitoredEngine.Completed += EngineCompleted;
            }
        }

        public bool Remove(IWorkflowEngine instance)
        {
            lock (_monitoredEngines)
            {
                var monitoredEngine = (IMonitoredEngine)instance;

                var wasMonitored = _monitoredEngines.Remove(monitoredEngine);
                if (wasMonitored)
                    UnregisterEvents(monitoredEngine);

                return wasMonitored;
            }
        }

        private void OnPlaceReached(object sender, IPlace place)
        {
            // Determine the result classification from this place forward
            var classification = _workplanAnalysis[place.Id].AggregatedClassification;

            // Publish the result if it is one of the final ones
            if (classification == NodeClassification.End || classification == NodeClassification.Failed)
            {
                var eventArgs = new PathPredictionEventArgs((IWorkflowEngine)sender, classification);
                PathPrediction(this, eventArgs);

                // As far as we are concerne this engine is complete
                Remove((IWorkflowEngine)sender);
            }
        }

        private void EngineCompleted(object sender, IPlace e)
        {
            Remove((IWorkflowEngine)sender);
        }


        public void Dispose()
        {
            lock (_monitoredEngines)
            {
                foreach (var engine in _monitoredEngines)
                    UnregisterEvents(engine);

                _monitoredEngines.Clear();
            }
        }

        private void UnregisterEvents(IMonitoredEngine engine)
        {
            engine.PlaceReached -= OnPlaceReached;
            engine.Completed -= EngineCompleted;
        }

        public event EventHandler<PathPredictionEventArgs> PathPrediction;
    }

    /// <summary>
    /// Possible results that can be reached from an individual <see cref="IPlace"/>
    /// </summary>
    internal class PossibleResults
    {
        /// <summary>
        /// Place represented by this instance
        /// </summary>
        private readonly long _targetId;

        /// <summary>
        /// All end places reachable from 
        /// </summary>
        private readonly List<IConnector> _results = new List<IConnector>();

        public PossibleResults(long targetId)
        {
            _targetId = targetId;
        }

        /// <summary>
        /// The aggregated <see cref="NodeClassification"/> of the possible results. This can be useful if more than one result is possible,
        /// but they share the same classification.
        /// </summary>
        public NodeClassification AggregatedClassification
        {
            get
            {
                return _results.Aggregate(int.MaxValue, // Seed 
                    (current, connector) => current & (int)connector.Classification,
                    result => (NodeClassification)result);
            }
        }

        /// <summary>
        /// Add a possible result that can be reached from the represented place
        /// </summary>
        /// <returns><value>True</value> if the result could be added to the instance, <value>false</value> otherwise</returns>
        public bool AddResult(IConnector result)
        {
            if (_results.Contains(result))
                return false;

            _results.Add(result);
            return true;
        }
    }
}