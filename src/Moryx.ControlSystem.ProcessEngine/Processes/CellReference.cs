// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    internal class CellReference : ICell
    {
        public long Id { get; }

        // ReSharper disable UnassignedGetOnlyAutoProperty

        public string Name { get; }

        public string LocalIdentifier { get; }

        public string GlobalIdentifier { get; }

        public ICapabilities Capabilities { get; }

        public CellReference(long id)
        {
            Id = id;
        }

        public IEnumerable<Session> ControlSystemAttached()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Session> ControlSystemDetached()
        {
            throw new NotImplementedException();
        }

        public void StartActivity(ActivityStart activityStart)
        {
            throw new NotImplementedException();
        }

        public void ProcessAborting(IActivity affectedActivity)
        {
            throw new NotImplementedException();
        }

        public void SequenceCompleted(SequenceCompleted completed)
        {
            throw new NotImplementedException();
        }

#pragma warning disable 67
        public event EventHandler<ReadyToWork> ReadyToWork;

        public event EventHandler<NotReadyToWork> NotReadyToWork;

        public event EventHandler<ActivityCompleted> ActivityCompleted;

        public event EventHandler<ICapabilities> CapabilitiesChanged;
#pragma warning restore 67
    }
}
