// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools;
using Moryx.Workplans.Editing.Components;
using Moryx.Workplans.Editing.Properties;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Moryx.Workplans.Editing.Implementation
{
    internal class WorkplanEditingSession : IWorkplanEditingSession
    {
        private readonly Workplan _workplan;

        public WorkplanSession Session { get; private set; }

        public WorkplanEditingSession(Workplan workplan)
        {
            _workplan = workplan;

            Session = new WorkplanSession
            {
                Token = Guid.NewGuid().ToString(),
                Workplan = workplan,
            };
        }

        public void AddStep(IWorkplanStep step)
        {
            if (step is ISubworkplanStep stepSubWorkplan && _workplan.Id == stepSubWorkplan.Workplan.Id)
                throw new ArgumentException(Strings.SAME_SUBWORKPLAN);

            _workplan.Add(step);
        }

        public bool UpdateStep(IWorkplanStep step)
        {
            var stepToBeUpdated = _workplan.Steps.FirstOrDefault(s => s.Id == step.Id);
            if (stepToBeUpdated != null)
            {
                RemoveStep(step.Id);
                AddStep(step);
                return true;
            }

            return false;
        }

        public bool RemoveStep(long stepId)
        {
            var step = _workplan.Steps.FirstOrDefault(s => s.Id == stepId);
            if (step == null)
                return false;

            if (!_workplan.Remove(step))
                return false;

            // Check for each connector if another step uses it in the same role
            // Otherwise all remaining usages no longer represent a connection and can be deleted
            foreach (var input in step.Inputs.Where(i => i != null))
            {
                var otherInput = _workplan.Steps.Any(s => s.Inputs.Contains(input));
                if (otherInput)
                    continue;

                // If no other input uses it, we can remove all output references
                foreach (var otherStep in _workplan.Steps)
                {
                    for (int i = 0; i < otherStep.Outputs.Length; i++)
                    {
                        if (otherStep.Outputs[i] == input)
                            otherStep.Outputs[i] = null;
                    }
                }

                // Now we can remove the connector
                if (input.Classification == NodeClassification.Intermediate)
                    _workplan.Remove(input);
            }

            // The same logic applied to outputs
            foreach (var output in step.Outputs.Where(o => o != null))
            {
                var otherOutput = _workplan.Steps.Any(s => s.Outputs.Contains(output));
                if (otherOutput)
                    continue;

                // If no other output uses it, we can remove all input references
                foreach (var otherStep in _workplan.Steps)
                {
                    for (int i = 0; i < otherStep.Inputs.Length; i++)
                    {
                        if (otherStep.Inputs[i] == output)
                            otherStep.Inputs[i] = null;
                    }
                }

                // Now we can remove the connector
                if (output.Classification == NodeClassification.Intermediate)
                    _workplan.Remove(output);
            }

            return true;
        }

        public void AddConnector(IConnector connector)
        {
            _workplan.Add(connector);
        }

        public bool RemoveConnector(long connectorId)
        {
            // Get the connector
            var connector = _workplan.Connectors.FirstOrDefault(c => c.Id == connectorId);
            if (connector == null)
                return false;

            // All references to this connector
            foreach (var step in _workplan.Steps)
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
            return _workplan.Remove(connector);
        }

        public void Connect(IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode targetNode, int targetIndex)
        {
            if (sourceNode is IConnector start)
            {
                if (targetNode is IConnector)
                    throw new InvalidOperationException("Cannot connect start to End or Failed");

                if (start == ((IWorkplanStep)targetNode).Inputs[targetIndex])
                    return;

                var targetStep = (IWorkplanStep)targetNode;
                var targetConnector = targetStep.Inputs[targetIndex];
                ConnectStart(targetIndex, start, targetStep, targetConnector);
            }
            else if (targetNode is IConnector end)
            {
                var step = (IWorkplanStep)sourceNode;
                step.Outputs[sourceIndex] = end;
            }
            else
                ConnectTwoSteps(sourceNode, sourceIndex, targetNode, targetIndex);
        }

        private void ConnectStart(int targetIndex, IConnector start, IWorkplanStep targetStep, IConnector targetConnector)
        {
            var connectedInput = _workplan.Steps.SingleOrDefault(s => s.Inputs.Contains(start));
            if (connectedInput != null)
            {
                IConnector connector = null;
                if (_workplan.Steps.Any(s => s.Outputs.Contains(start)))
                {
                    connector = new Connector { Classification = NodeClassification.Intermediate };
                    _workplan.Add(connector);

                }
                ReplaceConnector(start, connector);
            }

            if (targetConnector != null)
            {
                ReplaceConnector(targetConnector, start);
                _workplan.Remove(targetConnector);
            }
            else
                targetStep.Inputs[targetIndex] = start;
        }

        private void ReplaceConnector(IConnector source, IConnector replacement)
        {
            foreach (var workplanStep in _workplan.Steps)
            {
                for (var i = 0; i < workplanStep.Outputs.Length; i++)
                {
                    if (workplanStep.Outputs[i] == source)
                        workplanStep.Outputs[i] = replacement;
                }
                for (var i = 0; i < workplanStep.Inputs.Length; i++)
                {
                    if (workplanStep.Inputs[i] == source)
                        workplanStep.Inputs[i] = replacement;
                }
            }
        }

        private void ConnectTwoSteps(IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode targetNode, int targetIndex)
        {

            // Source and target are steps
            var sourceStep = (IWorkplanStep)sourceNode;
            var targetStep = (IWorkplanStep)targetNode;

            //delete old connection if only sourceStep is connected
            var currentConnector = sourceStep.Outputs[sourceIndex];

            if (currentConnector != null)
            {
                if (_workplan.Steps.Count(s => s.Outputs.Contains(currentConnector)) == 1 && sourceStep.Outputs.Count(s => s == currentConnector) == 1)
                {
                    var oldInputStep = _workplan.Steps.SingleOrDefault(s => s.Inputs.Contains(currentConnector)); //is null when old connection is a connector (End/Failed)
                    if (oldInputStep != null && currentConnector.Classification == NodeClassification.Intermediate)
                    {
                        var oldInputId = Array.IndexOf(oldInputStep.Inputs, currentConnector);
                        oldInputStep.Inputs[oldInputId] = null;
                        _workplan.Remove(currentConnector);
                    }
                }
                sourceStep.Outputs[sourceIndex] = null;
            }

            // Check if source or target have a connector at this index otherwise create a new one
            var connector = targetStep.Inputs[targetIndex];
            if (connector == null)
            {
                connector = new Connector
                {
                    Classification = NodeClassification.Intermediate,
                    Name = sourceStep.OutputDescriptions[sourceIndex].Name
                };
                _workplan.Add(connector);
            }

            // Link source and target to connector
            sourceStep.Outputs[sourceIndex] = connector;
            targetStep.Inputs[targetIndex] = connector;
        }

        public void Disconnect(IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode targetNode, int targetIndex)
        {
            // Check what shall be disconnected
            if (sourceNode is IConnector start)
            {
                // Set a connector for a step
                var step = _workplan.Steps.First(s => s.Id == targetNode.Id);

                // Remove connector from input
                step.Inputs[targetIndex] = null;

                // Check if any other step uses this connector as output
                var others = _workplan.Steps.Where(s => s.Outputs.Contains(start)).ToList();
                if (!others.Any())
                    return;

                // That means it was a loop and we need to replace the connection
                var replacement = new Connector { Classification = NodeClassification.Intermediate };
                _workplan.Add(replacement);
                foreach (var other in others)
                {
                    for (var i = 0; i < other.Outputs.Length; i++)
                    {
                        if (other.Outputs[i] == start)
                            other.Outputs[i] = replacement;
                    }
                }
                step.Inputs[targetIndex] = replacement;
            }
            else if (targetNode is IConnector end)
            {
                // Set a connector for a step
                var step = _workplan.Steps.First(s => s.Id == sourceNode.Id);

                // Remove connector from output
                step.Outputs[sourceIndex] = null;
            }
            else
            {
                // Source and target are steps
                var sourceStep = (IWorkplanStep)sourceNode;
                var targetStep = (IWorkplanStep)targetNode;

                // Find the connector between those steps
                var connector = targetStep.Inputs[targetIndex] ?? sourceStep.Outputs[sourceIndex];

                // Remove reference of source step to connector
                sourceStep.Outputs[sourceIndex] = null;

                // Check if any other step has this connector as output
                var otherSteps = _workplan.Steps.Any(s => s.Outputs.Contains(connector));
                if (!otherSteps && connector.Classification != NodeClassification.Start)
                {
                    targetStep.Inputs[targetIndex] = null;
                    _workplan.Remove(connector);
                }
            }
        }

        // All values depend on the UI unfortunately, this is bad software design
        // but not worse then the independent magic numbers we had before
        private const int initialY = 0; // Since we create a stair-like structure 
        private const int initialX = 1120; //in the auto layout, start in the top right
        private const int nextYOffset = 70; // Height of a single step in the UI
        private const int extraYOffset = 14; // More outputs require more space for routing
        private const int nextXOffset = -112; // Since Success outputs are always left the next steps should be placed more left
        public void AutoLayout()
        {
            // Place start
            var start = _workplan.GetStart();
            start.Position = new Point(initialX, initialY);

            // Place intermediate steps
            var firstSteps = _workplan.GetNextSteps(start);
            var repositionedSteps = new Collection<IWorkplanStep>();
            var (lastLayerX, lastLayerY) = PlaceSteps(repositionedSteps, firstSteps, start.Position.X, start.Position.Y);

            // Place end
            _workplan.GetEnd().Position = new Point(lastLayerX, lastLayerY + nextYOffset + extraYOffset);
            _workplan.GetFailed().Position = new Point(initialX, lastLayerY + nextYOffset + extraYOffset);
        }

        private (int, int) PlaceSteps(ICollection<IWorkplanStep> repositionedSteps, IEnumerable<IWorkplanStep> nextSteps, int currentX, int currentY)
        {
            var newNextSteps = new Collection<IWorkplanStep>();

            var nextY = currentY + nextYOffset;
            var nextX = currentX + nextXOffset;
            currentY += nextYOffset + nextSteps.Count() * extraYOffset;

            foreach (var step in nextSteps)
            {
                step.Position = new Point(currentX, currentY);
                currentY -= extraYOffset;
                currentX -= 4 * nextXOffset;
                nextY = Math.Max(currentY + step.Outputs.Length * extraYOffset, nextY);

                var newNexts = _workplan.GetNextSteps(step)
                    .FilterAlreadyRepositionedSteps(repositionedSteps)
                    .ToArray();
                repositionedSteps.AddRange(newNexts);
                newNextSteps.AddRange(newNexts);
            }

            return !newNextSteps.Any() ? (nextX, nextY) : PlaceSteps(repositionedSteps, newNextSteps, nextX, nextY);
        }
    }
}

