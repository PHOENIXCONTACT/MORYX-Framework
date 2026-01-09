// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Extensions;
using Moryx.Tools;

namespace Moryx.FactoryMonitor.Endpoints.Models;

/// <summary>
/// A graph that represent any Resource that can be displayed on the UI.
/// For a Factory this represents the structure of the factory and its visible content.
/// For a resource, this represents the resource and its parts/children that should be visible on the UI
/// </summary>
internal class SimpleGraph
{
    /// <summary>
    /// Resource Id of this node in the graph
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Holds the name of the type of the Item (Cell,Factory,Location,etc...)
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Contains the elements that should be displayed in the current factory
    /// </summary>
    public List<SimpleGraph> Children { get; set; } = new List<SimpleGraph>();

    /// <summary>
    /// Creates a full simple Graph from the <paramref name="resource"/>
    /// </summary>
    public static SimpleGraph Create(Resource resource)
    {
        if (resource is not ManufacturingFactory factory) return null;

        var graph = new SimpleGraph
        {
            Id = factory.Id,
            Type = nameof(ManufacturingFactory)
        };
        factory.Children.ForEach(graph.Append);
        return graph;
    }

    public VisualizableItemModel ToVisualItemModel(IResourceManagement resourceManager,
        ILogger<FactoryMonitorController> logger,
        Converter.Converter converter,
        Func<IMachineLocation, bool> filter)
    {
        var result = resourceManager.ReadUnsafe(Id, resource =>
        {
            if (resource is not MachineLocation machineLocation)
            {
                return null;
            }

            var resourcesAtThisLocation = machineLocation.Children.Where(x => x is ICell || x is IManufacturingFactory).ToArray();
            if (resourcesAtThisLocation.Length == 0)
            {
                logger.LogError("There is no resource type Cell or ManufacturingFactory found under Location '{Name}'",
                    machineLocation.Name);
                return null;
            }

            if (resourcesAtThisLocation.Length > 1)
            {
                logger.Log(LogLevel.Warning, "More than one resource were found under Location '{Name}'. The first child will be used",
                    machineLocation.Name);
            }

            var resourceAtThisLocation = resourcesAtThisLocation.First();

            var model = new VisualizableItemModel();

            switch (resourceAtThisLocation)
            {
                case ManufacturingFactory factory:
                    model = Converter.Converter.ToFactoryStateModel(factory);
                    break;
                case Cell cell:
                    model = cell.GetResourceChangedModel(converter, resourceManager, filter);
                    model.IsACell = true;
                    break;
            }
            model.IconName = machineLocation.SpecificIcon;
            model.Id = resourceAtThisLocation?.Id ?? 0;
            model.Location = Converter.Converter.ToCellLocationModel(machineLocation);
            return model;
        });

        return result;
    }

    public SimpleGraph GetSubGraphById(long id)
    {
        if (Type == nameof(ManufacturingFactory) && Id == id) return this;

        foreach (var child in Children)
        {
            var result = child.GetSubGraphById(id);
            if (result is not null)
                return result;
        }
        return null;
    }

    public void Append(Resource addition)
    {
        switch (addition)
        {
            case MachineLocation:
            case ManufacturingFactory:
            case Cell:
                AddSubGraph(addition);
                return;

            case MachineGroup group:
                group.Children?.ForEach(Append);
                return;
        }
    }

    private void AddSubGraph(Resource addition)
    {
        var subGraph = new SimpleGraph
        {
            Id = addition.Id,
            Type = addition.GetType().Name
        };

        if (addition is not Cell)
            addition.Children.ForEach(subGraph.Append);

        Children.Add(subGraph);
    }
}
