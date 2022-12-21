// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Runtime.Serialization;

namespace Moryx.Workplans.WorkplanSteps
{
    /// <summary>
    /// Base class for all steps that are build around another workplan
    /// </summary>
    [DataContract]
    public abstract class SubWorkplanStepBase : WorkplanStepBase, ISubworkplanStep
    {
        /// <summary>
        /// Create empty instance and set workplan later
        /// </summary>
        protected SubWorkplanStepBase()
        {
        }

        /// <summary>
        /// Create step from another workplan
        /// </summary>
        protected SubWorkplanStepBase(IWorkplan workplan)
        {
            Workplan = workplan;
            Name = workplan.Name;

            // Step outputs are created from all exits of the sub workplan
            OutputDescriptions = (from connector in workplan.Connectors
                                  where connector.Classification.HasFlag(NodeClassification.Exit)
                                  select new OutputDescription
                                  {
                                      Name = connector.Name,
                                      MappingValue = connector.Id,
                                      OutputType = connector.Classification == NodeClassification.End ? OutputType.Success : OutputType.Failure
                                  }).ToArray();
            Outputs = new IConnector[OutputDescriptions.Length];
        }

        /// <see cref="ISubworkplanStep.WorkplanId"/>
        [DataMember]
        long ISubworkplanStep.WorkplanId => Workplan.Id;

        /// <summary>
        /// Our SubWorkplan
        /// </summary>
        protected IWorkplan Workplan { get; private set; }

        /// <see cref="ISubworkplanStep"/>
        IWorkplan ISubworkplanStep.Workplan
        {
            get => Workplan;
            set => Workplan = value;
        }
    }
}
