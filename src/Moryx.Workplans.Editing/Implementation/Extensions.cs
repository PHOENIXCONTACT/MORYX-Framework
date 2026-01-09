// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Editing;

internal static class Extensions
{
    extension(Workplan workplan)
    {
        public IConnector GetStart() =>
            workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.Start);

        public IConnector GetEnd() =>
            workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.End);

        public IConnector GetFailed() =>
            workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.Failed);

        public IEnumerable<IWorkplanStep> GetNextSteps(IWorkplanStep from) =>
            workplan.Steps.Where(s => s.Inputs.Any(i => from.Outputs.Any(o => o?.Id == i?.Id)));
    }

    public static IEnumerable<IWorkplanStep> FilterAlreadyRepositionedSteps(this IEnumerable<IWorkplanStep> workplanSteps, IEnumerable<IWorkplanStep> repositionedSteps) =>
        workplanSteps.Where(s => repositionedSteps.All(rS => rS.Id != s.Id));

    public static IEnumerable<IWorkplanStep> GetNextSteps(this Workplan workplan, IConnector from)
        => workplan.Steps.Where(s => s.Inputs.Any(i => i.Id == from.Id));
}