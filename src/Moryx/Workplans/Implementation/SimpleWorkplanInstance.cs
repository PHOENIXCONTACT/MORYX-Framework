// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans;

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
    extension(IWorkplanInstance workplanInstance)
    {
        public IEnumerable<IPlace> StartPlaces()
        {
            return workplanInstance.Places.Where(p => p.Classification == NodeClassification.Start);
        }

        public IEnumerable<IPlace> EndPlaces()
        {
            return workplanInstance.Places.Where(p => p.Classification.HasFlag(NodeClassification.Exit));
        }

        public ITransition GetTransition(long id)
        {
            return workplanInstance.Transitions.First(t => t.Id == id);
        }
    }
}