// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans;

/// <summary>
/// Facade for user performed workplan modifications
/// </summary>
public interface IWorkplanEditing
{
    /// <summary>
    /// Steps available for the workplan
    /// </summary>
    IReadOnlyList<Type> AvailableSteps { get; }

    /// <summary>
    /// Edit a given workplan
    /// </summary>
    WorkplanSession EditWorkplan(Workplan workplan, bool duplicate);

    /// <summary>
    /// Auto layout a given session
    /// </summary>
    void AutoLayout(string sessionToken);

    /// <summary>
    /// Repoen a session from token
    /// </summary>
    WorkplanSession OpenSession(string session);

    /// <summary>
    /// Add step to the workplan
    /// </summary>
    void AddStep(string session, IWorkplanStep step);

    /// <summary>
    /// Update a workplan step
    /// </summary>
    bool UpdateStep(string session, IWorkplanStep step);

    /// <summary>
    /// Remove a step
    /// </summary>
    bool RemoveStep(string session, long stepId);

    /// <summary>
    /// Add a connector
    /// </summary>
    void AddConnector(string session, IConnector connector);

    /// <summary>
    /// Remove connector
    /// </summary>
    bool RemoveConnector(string session, long connectorId);

    /// <summary>
    /// Connect output of one step with input of another one
    /// </summary>
    void Connect(string session, IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode target, int targetIndex);

    /// <summary>
    /// Disconnect output of one step with input of another one
    /// </summary>
    void Disconnect(string session, IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode target, int targetIndex);

    /// <summary>
    /// Close a session
    /// </summary>
    void CloseSession(string sessionToken);
}