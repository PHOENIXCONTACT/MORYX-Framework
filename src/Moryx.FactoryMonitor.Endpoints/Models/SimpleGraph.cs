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
    public List<SimpleGraph> Children { get; set; } = [];

    /// <summary>
    /// Creates a full <see cref="SimpleGraph"/> from the <paramref name="resource"/>
    /// </summary>
    public static SimpleGraph Create(Resource resource)
    {
        if (resource is not IManufacturingFactory factory)
        {
            return null;
        }

        var graph = new SimpleGraph
        {
            Id = factory.Id,
            Type = nameof(IManufacturingFactory)
        };
        resource.Children.ForEach(graph.AppendLocation);
        return graph;
    }

    // ToDo: Use existing graph model
    public VisualizableItemModel ToVisualItemModel(IResourceManagement resourceManager,
        ILogger<FactoryMonitorController> logger,
        Converter.Converter converter)
    {
        if (Type is not nameof(IMachineLocation))
        {
            return null;
        }

        if (Children.Count == 0)
        {
            logger.LogError("There is no resource of type {cell} or {factory} found under Location '{id}'",
                nameof(ICell), nameof(ManufacturingFactory), Id);
            return null;
        }

        if (Children.Count > 1)
        {
            logger.Log(LogLevel.Warning, "More than one resource were found under {location} '{id}'. The first leaf will be used",
                nameof(IMachineLocation), Id);
        }

        var location = resourceManager.GetResource<IMachineLocation>(Id);
        var targetResourceId = Children.First().Id;
        return resourceManager.ReadUnsafe<VisualizableItemModel>(targetResourceId, target =>
        {
            switch (target)
            {
                case ICell cell:
                    var model = cell.GetResourceChangedModel(converter, resourceManager, location);
                    model.IsACell = true;
                    return model;
                case IManufacturingFactory factory:
                    return new FactoryStateModel
                    {
                        Id = factory.Id,
                        Location = Converter.Converter.ToCellLocationModel(location),
                        IconName = location.SpecificIcon
                    };
                default:
                    return null;
            }
        });
    }

    public SimpleGraph GetSubGraphById(long id)
    {
        if (Type == nameof(IManufacturingFactory) && Id == id)
        {
            return this;
        }

        foreach (var child in Children)
        {
            var result = child.GetSubGraphById(id);
            if (result is not null)
            {
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// Appends a non-location layer, currently only <see cref="IManufacturingFactory"/> and
    /// <see cref="ICell"/>, the latter of which denote leaf notes
    /// </summary>
    /// <param name="addition">A possible <see cref="IManufacturingFactory"/>, <see cref="ICell"/> or a parent resource of one</param>
    public void Append(Resource addition)
    {
        switch (addition)
        {
            case IManufacturingFactory:
            case ICell:
                AddSubGraph(addition);
                return;

            default:
                addition.Children?.ForEach(Append);
                return;
        }
    }


    /// <summary>
    /// Appends a leaf node to the current <see cref="SimpleGraph"/>
    /// </summary>
    /// <param name="leave">Any resource which denotes a leaf to the graph</param>
    public void AppendLeaf(Resource leave) => AddSubGraphRoot(leave);

    /// <summary>
    /// Appends a location level to the current <see cref="SimpleGraph"/>
    /// </summary>
    /// <param name="addition">A possible <see cref="IMachineLocation"/> or a parent resource of one</param>
    public void AppendLocation(Resource addition)
    {
        if (addition is IMachineLocation)
        {
            AddSubGraph(addition);
            return;
        }

        addition.Children?.ForEach(AppendLocation);
        return;
    }

    private void AddSubGraph(Resource addition)
    {
        // Add the node itself
        var subGraph = AddSubGraphRoot(addition);

        // Proceed through the resource tree
        switch (addition)
        {
            // Prefer machine property as target of the location before using children
            // This unifies behaviour with the gathering of resource changed models in the controller
            // ToDo: With MORYX 12 locations shhould hold a list of targets which should be used exclusively instead of children here
            case IMachineLocation { Machine: Resource leaf }:
                subGraph.AppendLeaf(leaf);
                return;
            // Keep fallback behaviour to use machine location children, except the location is also a cell
            case IMachineLocation and ICell:
                subGraph.AppendLeaf(addition);
                return;
            // Keep fallback behaviour to use machine location children
            case IMachineLocation:
                addition.Children.ForEach(subGraph.Append);
                return;
            // In factories we first need locations
            case IManufacturingFactory:
                addition.Children.ForEach(subGraph.AppendLocation);
                return;
            // Cells denote leaf nodes in the Graph
            // ToDo: MORYX 12: Change behaviour to make all none IManufacturingFactory resources
            // leaf nodes, when they were registered by a location
            case ICell:
                return;
            // Skip layers of resources without meaning for the factory monitor
            default:
                addition.Children.ForEach(subGraph.Append);
                return;
        }
    }

    private SimpleGraph AddSubGraphRoot(Resource addition)
    {
        var subGraph = new SimpleGraph
        {
            Id = addition.Id,
            Type = addition switch
            {
                IManufacturingFactory => nameof(IManufacturingFactory),
                IMachineLocation => nameof(IMachineLocation),
                ICell => nameof(ICell),
                _ => addition.GetType().Name
            }
        };
        Children.Add(subGraph);
        return subGraph;
    }
}
