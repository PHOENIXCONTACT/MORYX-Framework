// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.Resources.Benchmarking.Messages;

namespace Moryx.Resources.Benchmarking
{
    /// <summary>
    /// Example Class copied from the Demo
    /// </summary>
    public class AssemblyCell : Cell
    {
        [ResourceReference(ResourceRelationType.Driver, IsRequired = true)]
        public IMessageDriver<object> Driver { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Driver.Received += OnMessageReceived;
        }

        protected override void OnDispose()
        {
            Driver.Received -= OnMessageReceived;

            base.OnDispose();
        }

        public override IEnumerable<Session> ControlSystemAttached()
        {
            yield break;
        }


        public override IEnumerable<Session> ControlSystemDetached()
        {
            yield break;
        }

        public override void StartActivity(ActivityStart activityStart)
        {
            _activityStart = activityStart;
            Driver.Send(new AssembleProductMessage { ActivityId = activityStart.Activity.Id });
        }

        public override void SequenceCompleted(SequenceCompleted completed)
        {
            Driver.Send(new ReleaseWorkpieceMessage());
        }

        private ActivityStart _activityStart;

        private void OnMessageReceived(object sender, object message)
        {
            switch (message)
            {
                case WorkpieceArrivedMessage arrived:
                    var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, arrived.ProcessId);
                    PublishReadyToWork(rtw);
                    break;
                case AssemblyCompletedMessage completed:
                    if (_activityStart != null)
                    {
                        var result = _activityStart.CreateResult(completed.Result);
                        PublishActivityCompleted(result);
                        _activityStart = null;
                    }
                    break;
            }
        }
    }
}

