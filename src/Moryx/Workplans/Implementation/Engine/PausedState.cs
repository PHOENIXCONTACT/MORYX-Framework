// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans;

internal class PausedState : EngineState
{
    public PausedState(WorkplanEngine context, StateMap stateMap) : base(context, stateMap)
    {
    }

    /// <summary>
    /// Pause execution on the engine
    /// </summary>
    internal override WorkplanSnapshot Pause()
    {
        return Context.CurrentSnapshot;
    }

    internal override void Restore(WorkplanSnapshot snapshot)
    {
        if (snapshot != Context.CurrentSnapshot)
            throw new InvalidOperationException("Can not restore paused engine with different snapshot. Create a new instance instead!");
    }

    internal override void Start()
    {
        NextState(StateRunning);
        Context.ExecuteResume();
    }

    internal override void Destroy()
    {
        ExecuteDestroy();
    }
}