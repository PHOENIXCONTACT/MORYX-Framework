// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Moryx.Workplans
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
        public IEnumerable<IConnector> Connectors => _connectors;

        /// <summary>
        /// Editable list of steps
        /// </summary>
        [DataMember]
        private List<IWorkplanStep> _steps;

        /// <see cref="IWorkplan"/>
        public IEnumerable<IWorkplanStep> Steps => _steps;

        /// <summary>
        /// Add a range of connectors to the workplan
        /// </summary>
        public void Add(params IWorkplanNode[] nodes)
        {
            foreach (var node in nodes)
            {
                node.Id = ++MaxElementId;
                if (node is IConnector)
                    _connectors.Add((IConnector)node);
                else
                    _steps.Add((IWorkplanStep)node);
            }
        }
        /// <summary>
        /// Removes a node from the workplan
        /// </summary>
        public bool Remove(IWorkplanNode node)
        {
            return node is IConnector ? _connectors.Remove((IConnector)node) : _steps.Remove((IWorkplanStep)node);
        }

        /// <summary>
        /// Restore a workplan with a list of connectors and steps
        /// </summary>
        public static Workplan Restore(List<IConnector> connectors, List<IWorkplanStep> steps)
        {
            return new Workplan(connectors, steps);
        }

        public static bool NoteSameFollowringSteps(Workplan workplan, Workplan newWorkplan ,IWorkplanStep step, IWorkplanStep newStep, List<IWorkplanStep> needToCheck, List<IWorkplanStep> newNeedToCheck, List<IWorkplanStep> isChecked)
        {
            
            for (int a = 0; a < step.Outputs.Length; a++)
            {
                var connector = step.Outputs[a];
                var newConnector = newStep.Outputs[a];

                if (connector.Id == newConnector.Id)
                {

                    bool isNotEndConnector = !(connector.Classification.Equals(NodeClassification.End)) && !(newConnector.Classification.Equals(NodeClassification.End));
                    bool isNotFailedConnector = !(connector.Classification.Equals(NodeClassification.Failed)) && !(newConnector.Classification.Equals(NodeClassification.Failed));

                    if (isNotEndConnector && isNotFailedConnector)
                    {
                        var follower = workplan.Steps.FirstOrDefault(x => x.Inputs.Any(y => y.Equals(connector)));
                        var newFollower = newWorkplan.Steps.FirstOrDefault(x => x.Inputs.Any(y => y.Equals(newConnector)));

                        bool isAlreadyChecked = (isChecked.Contains(follower) || isChecked.Contains(newFollower));

                        if (!(isAlreadyChecked))
                        {
                            needToCheck.Add(follower);
                            newNeedToCheck.Add(newFollower);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Compare two workplans
        /// </summary>
        /// <param name="workplan"></param>
        /// <param name="newWorkplan"></param>
        /// <returns></returns>
        public static bool Equals(Workplan workplan, Workplan newWorkplan)
        {
            var step = workplan.Steps.First(x => x.Inputs.Any(y => y.Classification.Equals(NodeClassification.Start)));
            var newStep = newWorkplan.Steps.First(x => x.Inputs.Any(y => y.Classification.Equals(NodeClassification.Start)));

            List<IWorkplanStep> needToCheck = new List<IWorkplanStep>() { step };
            List<IWorkplanStep> newNeedToCheck = new List<IWorkplanStep>() { newStep };

            List<IWorkplanStep> isChecked = new List<IWorkplanStep>();

            while (needToCheck.Count != 0 && newNeedToCheck.Count != 0)
            {

                bool isSameStep = (step.GetType() == newStep.GetType());
                if (isSameStep)
                {

                    bool sameConnections = NoteSameFollowringSteps(workplan, newWorkplan, step, newStep, needToCheck, newNeedToCheck, isChecked);
                    if (sameConnections)
                    {
                        needToCheck.Remove(step);
                        newNeedToCheck.Remove(newStep);

                        isChecked.Add(step);

                        if (needToCheck.Count != 0 && newNeedToCheck.Count != 0)
                        {
                            step = needToCheck[0];
                            newStep = newNeedToCheck[0];
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
          

    }
}