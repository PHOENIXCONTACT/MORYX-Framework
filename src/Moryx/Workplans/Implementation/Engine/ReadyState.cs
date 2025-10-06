// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    internal class ReadyState : EngineState
    {
        public ReadyState(WorkplanEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        internal override void Initialize(IWorkplanInstance workplanInstance)
        {
            // Nothing happens
        }

        internal override void Start()
        {
            NextState(StateRunning);
            Context.ExecuteStart();
        }

        internal override void Restore(WorkplanSnapshot snapshot)
        {
            NextState(StateRestored);
            Context.CurrentSnapshot = snapshot;
            Context.ExecuteRestore();
        }

        internal override void Destroy()
        {
            ExecuteDestroy();
        }
    }
}
