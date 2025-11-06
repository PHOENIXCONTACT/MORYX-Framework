// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Factory;
using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Activities;

namespace Moryx.FactoryMonitor.Endpoints.Tests.Resources
{
    public class DummyCell : Resource, ICell
    {
        [EntryVisualization("celcius", "thermometer")]
        public double Temperature { get; set; }

        public ICapabilities Capabilities { get; set; }

        public virtual event EventHandler<ReadyToWork> ReadyToWork;
        public virtual event EventHandler<NotReadyToWork> NotReadyToWork;
        public virtual event EventHandler<ActivityCompleted> ActivityCompleted;
        public virtual event EventHandler<ICapabilities> CapabilitiesChanged;

        public void RaiseCapabilitiesChanged(ICapabilities capabilities)
        {
            Capabilities = capabilities;
            CapabilitiesChanged.Invoke(this, capabilities);
        }

        public IEnumerable<Session> ControlSystemAttached()
        {
            yield break;
        }

        public IEnumerable<Session> ControlSystemDetached()
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

        public void StartActivity(ActivityStart activityStart)
        {
            throw new NotImplementedException();
        }
    }
}

