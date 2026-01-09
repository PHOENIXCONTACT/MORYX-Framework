// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Workplans;

/// <summary>
/// Default implementation of IWorkplan
/// </summary>
[DataContract]
public class Workplan : IWorkplan, IPersistentObject
{
    /// <summary>
    /// Create a new workplan instance
    /// </summary>
    public Workplan() : this([], [])
    {
    }

    /// <summary>
    /// Private constructor used for new and restored workplans
    /// </summary>
    private Workplan(List<IConnector> connectors, List<IWorkplanStep> steps)
    {
        _connectors = connectors;
        _steps = steps;
    }

    /// <see cref="IWorkplan"/>
    public long Id { get; set; }

    ///<see cref="IWorkplan"/>
    public string Name { get; set; }

    ///<see cref="IWorkplan"/>
    public int Version { get; set; }

    ///<see cref="IWorkplan"/>
    public WorkplanState State { get; set; }

    /// <summary>
    /// Current biggest id in the workplan
    /// </summary>
    public int MaxElementId { get; set; }

    /// <summary>
    /// Editable list of connectors
    /// </summary>
    [DataMember]
    private List<IConnector> _connectors;

    /// <see cref="IWorkplan"/>
    public IEnumerable<IConnector> Connectors => _connectors;

    /// <summary>
    /// Editable list of steps
    /// </summary>
    [DataMember]
    private List<IWorkplanStep> _steps;

    /// <see cref="IWorkplan"/>
    public IEnumerable<IWorkplanStep> Steps => _steps;

    /// <summary>
    /// Add a range of connectors to the workplan
    /// </summary>
    public void Add(params IWorkplanNode[] nodes)
    {
        foreach (var node in nodes)
        {
            node.Id = ++MaxElementId;
            if (node is IConnector)
                _connectors.Add((IConnector)node);
            else
                _steps.Add((IWorkplanStep)node);
        }
    }
    /// <summary>
    /// Removes a node from the workplan
    /// </summary>
    public bool Remove(IWorkplanNode node)
    {
        return node is IConnector ? _connectors.Remove((IConnector)node) : _steps.Remove((IWorkplanStep)node);
    }

    /// <summary>
    /// Restore a workplan with a list of connectors and steps
    /// </summary>
    public static Workplan Restore(List<IConnector> connectors, List<IWorkplanStep> steps)
    {
        return new Workplan(connectors, steps);
    }
}