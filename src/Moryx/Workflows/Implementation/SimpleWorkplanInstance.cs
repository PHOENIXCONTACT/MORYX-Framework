// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;

namespace Moryx.Workplans
{
    /// <summary>
    /// Simple workplan instance created from connected places and transitions
    /// </summary>
    internal class SimpleWorkplanInstance : IWorkplanInstance
    {
        /// <summary>
        /// Constructor to create a <see cref="SimpleWorkplanInstance"/> from a <paramref name="workplan"/>, 
        /// from <paramref name="places"/> and from <paramref name="transitions"/>
        /// </summary>
        public SimpleWorkplanInstance(IWorkplan workplan, IReadOnlyList<IPlace> places, IReadOnlyList<ITransition> transitions)
        {
            Workplan = workplan;
            Places = places;
            Transitions = transitions;
        }

        /// <inheritdoc />
        public IWorkplan Workplan { get; }

        /// <inheritdoc />
        public IReadOnlyList<IPlace> Places { get; }

        /// <inheritdoc />
        public IReadOnlyList<ITransition> Transitions { get; }
    }

    internal static class WorkplanInstanceExtensions
    {
        public static IEnumerable<IPlace> StartPlaces(this IWorkplanInstance workplanInstance)
        {
            return workplanInstance.Places.Where(p => p.Classification == NodeClassification.Start);
        }

        public static IEnumerable<IPlace> EndPlaces(this IWorkplanInstance workplanInstance)
        {
            return workplanInstance.Places.Where(p => p.Classification.HasFlag(NodeClassification.Exit));
        }

        public static ITransition GetTransition(this IWorkplanInstance workplanInstance, long id)
        {
            return workplanInstance.Transitions.First(t => t.Id == id);
        }
    }
}
