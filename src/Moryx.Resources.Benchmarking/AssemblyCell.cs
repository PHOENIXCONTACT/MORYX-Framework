// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
        public IMessageDriver Driver { get; set; }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);

            Driver.Received += OnMessageReceived;
        }

        protected override void OnDispose()
        {
            Driver.Received -= OnMessageReceived;

            base.OnDispose();
        }

        protected override IEnumerable<Session> ProcessEngineAttached()
        {
            yield break;
        }

        protected override IEnumerable<Session> ProcessEngineDetached()
        {
            yield break;
        }

        public override void StartActivity(ActivityStart activityStart)
        {
            _activityStart = activityStart;
            Driver.SendAsync(new AssembleProductMessage { ActivityId = activityStart.Activity.Id }).Wait();
        }

        public override void SequenceCompleted(SequenceCompleted completed)
        {
            Driver.SendAsync(new ReleaseWorkpieceMessage()).Wait();
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

