// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Editing.Implementation
{
    internal static class Extensions
    {
        public static IConnector GetStart(this Workplan workplan) =>
            workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.Start);

        public static IConnector GetEnd(this Workplan workplan) =>
            workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.End);

        public static IConnector GetFailed(this Workplan workplan) =>
            workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.Failed);

        public static IEnumerable<IWorkplanStep> GetNextSteps(this Workplan workplan, IWorkplanStep from) =>
            workplan.Steps.Where(s => s.Inputs.Any(i => from.Outputs.Any(o => o?.Id == i?.Id)));

        public static IEnumerable<IWorkplanStep> FilterAlreadyRepositionedSteps(this IEnumerable<IWorkplanStep> workplanSteps, IEnumerable<IWorkplanStep> repositionedSteps) =>
            workplanSteps.Where(s => repositionedSteps.All(rS => rS.Id != s.Id));

        public static IEnumerable<IWorkplanStep> GetNextSteps(this Workplan workplan, IConnector from)
            => workplan.Steps.Where(s => s.Inputs.Any(i => i.Id == from.Id));
    }
}

