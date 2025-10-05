// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Extensions;
using Moryx.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// A graph that represent any Resource that can be displayed on the UI.
    /// For a Factory this represent the structure of the factory and its visible content.
    /// For a resource, this represent the resource and its parts/children that should be visible on the UI
    /// </summary>
    internal class SimpleGraph
    {
        /// <summary>
        /// Resource Id of this node in the graph
        /// </summary>
        public long Id { get;  set; }

        /// <summary>
        /// Holds the name of the type of the Item (Cell,Factory,Location,etc...)
        /// </summary>
        public string Type {  get; set; }

        /// <summary>
        /// Contains the elements that should be displayed in the current factory
        /// </summary>
        public List<SimpleGraph> Children { get;  set; } = new List<SimpleGraph>();

        /// <summary>
        /// Creates a full simple Graph from the <paramref name="factory"/>
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
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
            Converter converter,
            Func<IMachineLocation, bool> filter)
        {
            VisualizableItemModel result = resourceManager.Read(Id, e =>
            {
                if (e.GetDisplayableResourceLocation(logger) is not MachineLocation displayableItemLocation) return null;

                var resourceAtThisLocation = displayableItemLocation.Children.First(x => x is ICell || x is IManufacturingFactory);
                var model = new VisualizableItemModel();

                switch (resourceAtThisLocation)
                {
                    case ManufacturingFactory factory:
                        model = Converter.ToFactoryStateModel(factory);
                        break;
                    case Cell cell:
                        model = cell.GetResourceChangedModel(converter, resourceManager, filter);
                        model.IsACell = true;
                        break;
                    default:
                        break;
                }
                model.IconName = displayableItemLocation.SpecificIcon;
                model.Id = resourceAtThisLocation?.Id ?? 0;
                model.Location = Converter.ToCellLocationModel(displayableItemLocation);
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
                case MachineLocation location:
                case ManufacturingFactory factory:
                case Cell cell:
                    AddSubGraph(addition);
                    return;

                case MachineGroup group:
                    group.Children?.ForEach(Append);
                    return;
            }
        }

        private void AddSubGraph(Resource addition)
        {
            var subGraph = new SimpleGraph();
            subGraph.Id = addition.Id;
            subGraph.Type = addition.GetType().Name;
            if (addition is not Cell)
                addition.Children.ForEach(subGraph.Append);
            Children.Add(subGraph);
        }
    }
}

