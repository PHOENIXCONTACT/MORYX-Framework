using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Serialization;

namespace Marvin.Workflows
{
    /// <summary>
    /// Internal converter implementation
    /// </summary>
    internal class WorkplanEditing : IWorkplanEditing
    {
        /// <summary>
        /// Target workplan of this session
        /// </summary>
        private readonly Workplan _editedWorkplan;
        /// <summary>
        /// Types available within this session
        /// </summary>
        private StepCreationContainer[] _stepTypes = new StepCreationContainer[0];

        /// <summary>
        /// Source strategy to load workplans which are referenced as subworkplan
        /// </summary>
        private readonly IWorkplanSource _workplanSource;

        /// <summary>
        /// Start editing session for a certain workplan
        /// </summary>
        internal WorkplanEditing(Workplan editedWorkplan, IWorkplanSource workplanSource)
        {
            _editedWorkplan = editedWorkplan;
            _workplanSource = workplanSource;
        }

        /// <seealso cref="IWorkplanEditing"/>
        public void SetAvailableTypes(params Type[] stepTypes)
        {
            _stepTypes = stepTypes.Select(StepCreationContainer.FromType).ToArray();
        }

        /// <seealso cref="IWorkplanEditing"/>
        public WorkplanEditingSession ExportSession()
        {
            // Create basic object
            var session = new WorkplanEditingSession
            {
                Id = _editedWorkplan.Id,
                Name = _editedWorkplan.Name,
                State = _editedWorkplan.State,
                Version = _editedWorkplan.Version,
                Steps = new List<WorkplanStepModel>(_editedWorkplan.Steps.Count()),
                AvailableSteps = _stepTypes.Select((value, index) => value.ExportRecipe(index)).ToArray()
            };

            // Connectors can just be copied
            var connectors = _editedWorkplan.Connectors.Select(ConnectorModel.FromConnector).ToList();
            session.Connectors = connectors;

            // Now convert the steps
            var connectorMap = connectors.ToDictionary(c => c.Id, c => c);
            foreach (var step in _editedWorkplan.Steps)
            {
                var converted = SerializeStep(step);
                LinkStep(step, converted, connectorMap);
                session.Steps.Add(converted);
            }

            return session;
        }

        /// <seealso cref="IWorkplanEditing"/>
        public SessionModification AddStep(WorkplanStepRecipe recipe)
        {
            // Create instance from container
            var instance = _stepTypes[recipe.Index].Instantiate(recipe, _workplanSource);

            // Add to the underlying workplan
            _editedWorkplan.Add(instance);

            // Transform for the session
            var newStep = SerializeStep(instance);
            newStep.TemporaryId = recipe.TemporaryId;

            // Publish modifications
            return SessionModification.Summary(UserOperation.AddStep)
                .Added(newStep);
        }

        /// <seealso cref="IWorkplanEditing"/>
        public SessionModification AddConnector(ConnectorModel connectorModel)
        {
            var temporaryId = connectorModel.TemporaryId;
            var connector = connectorModel.ToConnector();
            _editedWorkplan.Add(connector);
            connectorModel = ConnectorModel.FromConnector(connector);
            connectorModel.TemporaryId = temporaryId;
            return SessionModification.Summary(UserOperation.AddConnector)
                .Added(connectorModel, new ConnectionPoint(), new ConnectionPoint());
        }

        /// <summary>
        /// Serialize any instance of <see cref="IWorkplanStep"/> to the DTO
        /// </summary>
        private static WorkplanStepModel SerializeStep(IWorkplanStep step)
        {
            // Create a serialized version of the step
            var stepType = step.GetType();
            var serialized = new WorkplanStepModel
            {
                Id = step.Id,
                Name = step.Name,
                Type = stepType.Name,
                Classification = StepCreationContainer.ToClassification(stepType),
                Inputs = new ConnectorModel[step.Inputs.Length],
                Outputs = new ConnectorModel[step.Outputs.Length],
                OutputDescriptions = step.OutputDescriptions,
                Properties = EntryConvert.EncodeObject(step, EditorVisibleSerialization.Instance)
            };

            // Subworkplans are enhanced with a special property
            if (serialized.Classification == StepClassification.Subworkplan)
            {
                serialized.SubworkplanId = ((ISubworkplanStep)step).Workplan.Id;
            }

            return serialized;
        }

        /// <summary>
        /// Link converted <see cref="WorkplanStepModel"/> to <see cref="ConnectorModel"/> while preserving object reference integrity
        /// </summary>
        private static void LinkStep(IWorkplanStep step, WorkplanStepModel converted, IDictionary<long, ConnectorModel> connectorMap)
        {
            for (int i = 0; i < step.Inputs.Length; i++)
            {
                var con = step.Inputs[i];
                if (con != null)
                {
                    converted.Inputs[i] = GetOrCreateConnector(con, connectorMap);
                }
            }

            for (int i = 0; i < step.Outputs.Length; i++)
            {
                var con = step.Outputs[i];
                if (con != null)
                {
                    converted.Outputs[i] = GetOrCreateConnector(con, connectorMap);
                }
            }
        }

        /// <summary>
        /// Get <see cref="ConnectorModel"/> from map or create new one
        /// </summary>
        private static ConnectorModel GetOrCreateConnector(IConnector connector, IDictionary<long, ConnectorModel> connectorMap)
        {
            return connectorMap.ContainsKey(connector.Id) ? connectorMap[connector.Id] : connectorMap[connector.Id] = ConnectorModel.FromConnector(connector);
        }

        /// <seealso cref="IWorkplanEditing"/>
        public SessionModification UpdateStep(WorkplanStepModel stepModel)
        {
            // Find the associated server side object
            var step = _editedWorkplan.Steps.First(s => s.Id == stepModel.Id);
            EntryConvert.UpdateInstance(step, new Entry { SubEntries = stepModel.Properties.SubEntries.ToList() }, EditorVisibleSerialization.Instance);
            return SessionModification.Summary(UserOperation.UpdateStep).Updated(stepModel);
        }

        /// <seealso cref="IWorkplanEditing"/>
        public SessionModification RemoveStep(long stepId)
        {
            // Remove from underlying workplan
            var step = _editedWorkplan.Steps.First(s => s.Id == stepId);
            _editedWorkplan.Remove(step);
            // Create summary
            var summary = SessionModification.Summary(UserOperation.RemoveStep).Deleted(new WorkplanStepModel { Id = stepId });

            // Add all connectors to the summary that were removed during clean up and remove them from the sessio
            foreach (var connector in ConnectorCleanup(step))
            {
                summary.Deleted(ConnectorModel.FromConnector(connector));
            }

            // Publish changes
            return summary;
        }

        /// <summary>
        /// Clean up connectors of the step that are no longer used
        /// </summary>
        /// <returns>Connectors that were found and removed</returns>
        private IEnumerable<IConnector> ConnectorCleanup(IWorkplanStep step)
        {
            // Count the number of other usages for the connectors of this step
            var usages = step.Inputs.Where(i => i != null && i.Classification == NodeClassification.Intermediate).Union(step.Outputs.Where(o => o != null && o.Classification == NodeClassification.Intermediate))
                                  .ToDictionary(connector => connector, con => 0);
            foreach (var workplanStep in _editedWorkplan.Steps)
            {
                // Check if this steps inputs contain a connector
                var usedInputs = workplanStep.Inputs.Where(x => x != null).Where(usages.ContainsKey);
                var usedOutputs = workplanStep.Outputs.Where(x => x != null).Where(usages.ContainsKey);

                foreach (var connector in usedInputs.Union(usedOutputs))
                {
                    usages[connector]++;
                }
            }

            // Remove all connectors that are no longer referenced
            foreach (var connector in usages.Where(p => p.Value == 0).Select(pair => pair.Key))
            {
                _editedWorkplan.Remove(connector);
                yield return connector;
            }
        }

        /// <seealso cref="IWorkplanEditing"/>
        public SessionModification RemoveConnector(long connectorId)
        {
            // Get the connector
            var connector = _editedWorkplan.Connectors.First(c => c.Id == connectorId);

            // All references to this connector
            foreach (var step in _editedWorkplan.Steps)
            {
                for (var i = 0; i < step.Inputs.Length; i++)
                {
                    if (step.Inputs[i] == connector)
                        step.Inputs[i] = null;
                }
                for (var i = 0; i < step.Outputs.Length; i++)
                {
                    if (step.Outputs[i] == connector)
                        step.Outputs[i] = null;
                }
            }

            // Remove from plan
            _editedWorkplan.Remove(connector);

            return SessionModification.Summary(UserOperation.RemoveConnector).Deleted(new ConnectorModel { Id = connectorId });
        }

        /// <seealso cref="IWorkplanEditing"/>
        public SessionModification Connect(ConnectionPoint source, ConnectionPoint target)
        {
            // Check what shall be connected
            SessionModification modification;
            if (source.IsConnector | target.IsConnector)
            {
                // Set a connector for a step
                if (source.IsConnector)
                    modification = SetConnector(source, target, true);
                else
                    modification = SetConnector(target, source, false);
            }
            else
            {
                // Connect two steps
                modification = ConnectSteps(source, target);
            }

            return modification;
        }

        /// <summary>
        /// Set connector on a step
        /// </summary>
        private SessionModification SetConnector(ConnectionPoint nodeConnection, ConnectionPoint stepConnection, bool input)
        {
            // Fetch nodes
            var connector = _editedWorkplan.Connectors.First(c => c.Id == nodeConnection.NodeId);
            var step = _editedWorkplan.Steps.First(s => s.Id == stepConnection.NodeId);

            if (input)
                step.Inputs[stepConnection.Index] = connector;
            else
                step.Outputs[stepConnection.Index] = connector;

            return SessionModification.Summary(UserOperation.Connect).Updated(ConnectorModel.FromConnector(connector), nodeConnection, stepConnection);
        }

        /// <summary>
        /// Connect two steps
        /// </summary>
        private SessionModification ConnectSteps(ConnectionPoint source, ConnectionPoint target)
        {
            // Get the involved steps and connector
            var steps = GetInvolvedSteps(source, target);

            // Check if source or target have a connector at this index otherwise create a new one
            var connectorCreated = false;
            var connector = steps.Connector;
            if (connector == null)
            {
                connector = new Connector
                {
                    Classification = NodeClassification.Intermediate,
                    Name = steps.Source.OutputDescriptions[source.Index].Name
                };
                _editedWorkplan.Add(connector);
                connectorCreated = true;
            }

            // Link source and target to connector
            steps.Source.Outputs[source.Index] = connector;
            steps.Target.Inputs[target.Index] = connector;

            var summary = SessionModification.Summary(UserOperation.Connect);
            if (connectorCreated)
                summary.Added(ConnectorModel.FromConnector(connector), source, target);
            else
                summary.Updated(ConnectorModel.FromConnector(connector), source, target);

            return summary;
        }

        /// <seealso cref="IWorkplanEditing"/>
        public SessionModification Disconnect(ConnectionPoint source, ConnectionPoint target)
        {
            var summary = SessionModification.Summary(UserOperation.Disconnect);

            // Check what shall be disconnected
            if (source.IsConnector)
            {
                // Set a connector for a step
                var step = _editedWorkplan.Steps.First(s => s.Id == target.NodeId);
                var connector = step.Inputs[target.Index];

                // Remove connector from input
                step.Inputs[target.Index] = null;

                var converted = SerializeStep(step);
                LinkStep(step, converted, new Dictionary<long, ConnectorModel>());
                summary.Updated(converted);

                // If connector was used a variable in the workplan we can delete it, if this was the last reference
                if (connector.Classification == NodeClassification.Intermediate)
                {
                    _editedWorkplan.Remove(connector);
                    summary.Deleted(ConnectorModel.FromConnector(connector));
                }
                return summary;
            }

            if (target.IsConnector)
            {
                // Set a connector for a step
                var step = _editedWorkplan.Steps.First(s => s.Id == source.NodeId);
                var connector = step.Outputs[source.Index];

                // Remove connector from output
                step.Outputs[source.Index] = null;

                var converted = SerializeStep(step);
                LinkStep(step, converted, new Dictionary<long, ConnectorModel>());
                summary.Updated(converted);

                // If connector was used a variable in the workplan we can delete it, if this was the last reference
                if (connector.Classification == NodeClassification.Intermediate)
                {
                    _editedWorkplan.Remove(connector);
                    summary.Deleted(ConnectorModel.FromConnector(connector));
                }
                return summary;
            }

            // Connect two steps
            return DisconnectSteps(source, target);
        }

        /// <summary>
        /// Undo the last operation
        /// </summary>
        /// <returns></returns>
        public SessionModification Undo()
        {
            return SessionModification.Summary(UserOperation.Undo);
        }

        /// <summary>
        /// Redo a previously undone operation
        /// </summary>
        /// <returns></returns>
        public SessionModification Redo()
        {
            return SessionModification.Summary(UserOperation.Redo);
        }

        /// <summary>
        /// Disconnect two steps
        /// </summary>
        private SessionModification DisconnectSteps(ConnectionPoint source, ConnectionPoint target)
        {
            // Get the involved steps and connector
            var steps = GetInvolvedSteps(source, target);

            // Remove reference of source step to connector
            steps.Source.Outputs[source.Index] = null;

            // Check if any other step has this connector as output
            var summary = SessionModification.Summary(UserOperation.Disconnect);
            var otherSteps = _editedWorkplan.Steps.Any(s => s.Outputs.Contains(steps.Connector));
            if (otherSteps)
            {
                // If others use this connector we are done here
                summary.Updated(ConnectorModel.FromConnector(steps.Connector), source, target);
            }
            else
            {
                // Remove reference to the connector and the connector itself
                summary.Deleted(ConnectorModel.FromConnector(steps.Connector), source, target);
            }

            return summary;
        }

        /// <summary>
        /// Get the two steps involved in a connection
        /// </summary>
        private InvolvedSteps GetInvolvedSteps(ConnectionPoint source, ConnectionPoint target)
        {
            // Fetch both steps in a single loop
            var steps = (from step in _editedWorkplan.Steps
                         let isSource = step.Id == source.NodeId
                         let isTarget = step.Id == target.NodeId
                         where isSource | isTarget
                         select new { step, isSource, isTarget }).ToArray();

            // Split result into individual steps
            var sourceStep = steps.First(s => s.isSource).step;
            var targetStep = steps.First(s => s.isTarget).step;

            // Find the connector between those steps
            var connector = sourceStep.Outputs[source.Index] ?? targetStep.Inputs[target.Index];

            return new InvolvedSteps(sourceStep, targetStep, connector);
        }

        /// <seealso cref="IWorkplanEditing"/>
        public Workplan Finish()
        {
            return _editedWorkplan;
        }

        /// <seealso cref="IWorkplanEditing"/>
        public Workplan Finish(WorkplanEditingSession session)
        {
            _editedWorkplan.Name = session.Name;
            _editedWorkplan.State = session.State;
            _editedWorkplan.Version = session.Version;

            return Finish();
        }

        /// <summary>
        /// Steps and connector involved in a connecton
        /// </summary>
        private struct InvolvedSteps
        {
            /// <summary>
            /// Create instance with all involved entities
            /// </summary>
            public InvolvedSteps(IWorkplanStep source, IWorkplanStep target, IConnector connector)
                : this()
            {
                Source = source;
                Target = target;
                Connector = connector;
            }

            /// <summary>
            /// Source of the connection, e.g. the step that references the connector as output
            /// </summary>
            public IWorkplanStep Source { get; private set; }

            /// <summary>
            /// Target of the connection, e.g the step that references the connector as input
            /// </summary>
            public IWorkplanStep Target { get; private set; }

            /// <summary>
            /// Connector of the connection
            /// </summary>
            public IConnector Connector { get; private set; }
        }
    }
}