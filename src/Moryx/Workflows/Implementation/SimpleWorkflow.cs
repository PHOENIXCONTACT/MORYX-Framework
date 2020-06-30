// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;

namespace Marvin.Workflows
{
    /// <summary>
    /// Simple workflow created from connected places and transitions
    /// </summary>
    internal class SimpleWorkflow : IWorkflow
    {
        /// <summary>
        /// Constructor to create workflow from places and transistions
        /// </summary>
        public SimpleWorkflow(IWorkplan workplan, IReadOnlyList<IPlace> places, IReadOnlyList<ITransition> transitions)
        {
            Workplan = workplan;
            Places = places;
            Transitions = transitions;
        }

        /// <summary>
        /// Workplan this workflow is an instance of
        /// </summary>
        public IWorkplan Workplan { get; }

        /// <summary>
        /// All places
        /// </summary>
        public IReadOnlyList<IPlace> Places { get; }

        /// <summary>
        /// All transitions of the workflow
        /// </summary>
        public IReadOnlyList<ITransition> Transitions { get; }
    }

    internal static class WorkflowExtenstions
    {
        public static IEnumerable<IPlace> StartPlaces(this IWorkflow workflow)
        {
            return workflow.Places.Where(p => p.Classification == NodeClassification.Start);
        }

        public static IEnumerable<IPlace> EndPlaces(this IWorkflow workflow)
        {
            return workflow.Places.Where(p => p.Classification.HasFlag(NodeClassification.Exit));
        }

        public static ITransition GetTransition(this IWorkflow workflow, long id)
        {
            return workflow.Transitions.First(t => t.Id == id);
        }
    }
}
