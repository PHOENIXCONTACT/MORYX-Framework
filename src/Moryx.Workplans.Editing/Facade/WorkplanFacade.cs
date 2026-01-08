// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;
using Moryx.Runtime.Modules;

namespace Moryx.Workplans.Editing;

internal class WorkplanFacade : IFacadeControl, IWorkplanEditing
{
    public Action ValidateHealthState { get; set; }

    public IWorkplanEditor Editor { get; set; }

    public IModuleLogger Logger { get; set; }

    public void Activate()
    {
    }

    public void Deactivate()
    {
    }

    public IReadOnlyList<Type> AvailableSteps => Editor.AvailableSteps;

    public WorkplanSession EditWorkplan(Workplan workplan, bool duplicate)
    {
        ValidateHealthState();
        return Editor.EditWorkplan(workplan, duplicate).Session;
    }

    public WorkplanSession OpenSession(string session)
    {
        ValidateHealthState();
        return Editor[session]?.Session;
    }

    public void AddStep(string session, IWorkplanStep step)
    {
        ValidateHealthState();
        Editor[session].AddStep(step);
    }

    public bool UpdateStep(string session, IWorkplanStep step)
    {
        ValidateHealthState();
        return Editor[session]?.UpdateStep(step) ?? false;
    }

    public bool RemoveStep(string session, long stepId)
    {
        ValidateHealthState();
        return Editor[session]?.RemoveStep(stepId) ?? false;
    }

    public void AddConnector(string session, IConnector connector)
    {
        ValidateHealthState();
        Editor[session]?.AddConnector(connector);
    }

    public bool RemoveConnector(string session, long connectorId)
    {
        ValidateHealthState();
        return Editor[session]?.RemoveConnector(connectorId) ?? false;
    }

    public void Connect(string session, IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode target, int targetIndex)
    {
        ValidateHealthState();
        Editor[session]?.Connect(sourceNode, sourceIndex, target, targetIndex);
    }

    public void Disconnect(string session, IWorkplanNode sourceNode, int sourceIndex, IWorkplanNode target, int targetIndex)
    {
        ValidateHealthState();
        Editor[session]?.Disconnect(sourceNode, sourceIndex, target, targetIndex);
    }

    public void CloseSession(string sessionToken)
    {
        ValidateHealthState();
        Editor.CloseSession(sessionToken);
    }

    public void AutoLayout(string sessionToken)
    {
        ValidateHealthState();
        Editor[sessionToken].AutoLayout();
    }
}