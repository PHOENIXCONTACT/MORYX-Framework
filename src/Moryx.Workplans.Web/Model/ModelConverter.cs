// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.Workplans.Endpoint;

internal static class ModelConverter
{
    private static readonly ICustomSerialization WorkplanStepSerialization = new WorkplanStepSerialization();
    public static WorkplanNodeClassification ToClassification(Type type)
    {
        // Everything not explicitly set is an execution step
        var classification = WorkplanNodeClassification.Execution;

        // Split and join are recognized explicitly
        if (type == typeof(SplitWorkplanStep) || type == typeof(JoinWorkplanStep))
            classification = WorkplanNodeClassification.ControlFlow;

        // And subworkpans are another exception
        else if (typeof(ISubworkplanStep).IsAssignableFrom(type))
            classification = WorkplanNodeClassification.Subworkplan;

        return classification;
    }

    public static WorkplanSessionModel ConvertSession(WorkplanSession session)
    {
        var workplan = session.Workplan;
        return new WorkplanSessionModel
        {
            Name = workplan.Name,
            WorkplanId = workplan.Id,
            Version = workplan.Version,
            State = workplan.State,
            SessionToken = session.Token,
            Nodes = ExtractNodes(session)
        };
    }

    private static WorkplanNodeModel[] ExtractNodes(WorkplanSession session)
    {
        var workplan = session.Workplan;
        var steps = workplan.Steps.ToList();
        var workplanNodes = new List<WorkplanNodeModel>(workplan.MaxElementId);

        var explicitConnectors = session.Workplan.Connectors
            .Where(c => c.Classification != NodeClassification.Intermediate);
        foreach (var connector in explicitConnectors)
        {
            var nodeModel = ConvertConnectorNode(connector, steps);
            workplanNodes.Add(nodeModel);
        }

        foreach (var step in steps)
        {
            var nodeModel = ConvertWorkplanStep(step, steps);
            workplanNodes.Add(nodeModel);
        }

        return workplanNodes.ToArray();
    }

    private static WorkplanNodeModel ConvertConnectorNode(IConnector connector, List<IWorkplanStep> steps)
    {
        var classification = connector.Classification.HasFlag(NodeClassification.Entry)
            ? WorkplanNodeClassification.Input
            : WorkplanNodeClassification.Output;

        var nodeModel = new WorkplanNodeModel
        {
            Id = connector.Id,
            Name = connector.Name,
            Type = connector.GetType().Name,
            Classification = classification,
            PositionLeft = connector.Position.X,
            PositionTop = connector.Position.Y,
        };

        if (classification == WorkplanNodeClassification.Input)
        {
            nodeModel.Inputs = [];
            var output = new NodeConnectionPoint { Name = "Out" };
            output.Connections.AddRange(steps.SelectMany(s => s.Inputs.Where(o => o == connector)
                .Select(o => new NodeConnector { NodeId = s.Id, Index = Array.IndexOf(s.Inputs, connector) })));
            nodeModel.Outputs = [output];
        }
        else
        {
            nodeModel.Outputs = [];
            var input = new NodeConnectionPoint { Name = "In" };
            input.Connections.AddRange(steps.SelectMany(s => s.Outputs.Where(o => o == connector)
                .Select(o => new NodeConnector { NodeId = s.Id, Index = Array.IndexOf(s.Outputs, connector) })));
            nodeModel.Inputs = [input];
        }

        return nodeModel;
    }

    public static WorkplanNodeModel ConvertWorkplanStep(IWorkplanStep step, List<IWorkplanStep> steps)
    {
        var workplanStep = (WorkplanStepBase)step;
        var nodeModel = new WorkplanNodeModel
        {
            Id = step.Id,
            Name = step.Name,
            DisplayName = step?.Name,
            Type = step.GetType().Name,
            Classification = ToClassification(step.GetType()),
            PositionLeft = step.Position.X,
            PositionTop = step.Position.Y,
            SubworkplanId = step is ISubworkplanStep subworkplan ? subworkplan.WorkplanId : 0,
            Properties = EntryConvert.EncodeObject(step, WorkplanStepSerialization)
        };

        var connections = new NodeConnectionPoint[step.Inputs.Length];
        for (int i = 0; i < step.Inputs.Length; i++)
        {
            connections[i] = new NodeConnectionPoint { Name = "Input_" + i, Index = i };
            var connector = step.Inputs[i];
            if (connector == null)
                continue;

            connections[i].Connections.AddRange(steps.SelectMany(s => s.Outputs.Where(o => o == connector)
                .Select(o => new NodeConnector { NodeId = s.Id, Index = Array.IndexOf(s.Outputs, connector) })));

            if (connector.Classification != NodeClassification.Intermediate)
                connections[i].Connections.Add(new NodeConnector { NodeId = connector.Id, Index = 0 });
        }
        nodeModel.Inputs = connections;

        connections = new NodeConnectionPoint[step.Outputs.Length];
        for (int i = 0; i < step.Outputs.Length; i++)
        {
            connections[i] = new NodeConnectionPoint { Name = step.OutputDescriptions[i].Name, Index = i };
            var connector = step.Outputs[i];
            if (connector == null)
                continue;

            connections[i].Connections.AddRange(steps.SelectMany(s => s.Inputs.Where(i => i == connector)
                .Select(o => new NodeConnector { NodeId = s.Id, Index = Array.IndexOf(s.Inputs, connector) })));

            if (connector.Classification != NodeClassification.Intermediate)
                connections[i].Connections.Add(new NodeConnector { NodeId = connector.Id, Index = 0 });
        }
        nodeModel.Outputs = connections;

        return nodeModel;
    }
}