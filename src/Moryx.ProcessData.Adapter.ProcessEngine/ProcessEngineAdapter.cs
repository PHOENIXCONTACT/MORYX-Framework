// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Processes;
using Moryx.Modules;
using Moryx.ProcessData.Bindings;

namespace Moryx.ProcessData.Adapter.ProcessEngine
{
    [Plugin(LifeCycle.Singleton)]
    internal class ProcessEngineAdapter : IPlugin
    {
        private const string MeasurementPrefix = "controlSystem_";

        #region Fields and Properties

        private MeasurementBindingProcessor _processBindingProcessor;
        private MeasurementBindingProcessor _activityBindingProcessor;

        #endregion

        #region Dependencies

        public IProcessControl ProcessControl { get; set; }

        public IJobManagement JobManagement { get; set; }

        public IResourceManagement ResourceManagement { get; set; }

        public IProcessDataMonitor ProcessDataMonitor { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        /// <inheritdoc />
        public void Start()
        {
            var processResolverFactory = new ProcessBindingResolverFactory();
            _processBindingProcessor = new MeasurementBindingProcessor(processResolverFactory, ModuleConfig.ProcessBindings);

            var activityResolverFactory = new ResourceActivityBindingResolverFactory(ResourceManagement);
            _activityBindingProcessor = new MeasurementBindingProcessor(activityResolverFactory, ModuleConfig.ActivityBindings);

            ProcessControl.ProcessUpdated += OnProcessUpdated;
            ProcessControl.ActivityUpdated += OnActivityUpdated;
        }

        /// <summary>
        /// Stops the adapter component
        /// </summary>
        public void Stop()
        {
            ProcessControl.ActivityUpdated -= OnActivityUpdated;
            ProcessControl.ProcessUpdated -= OnProcessUpdated;
        }

        private void OnProcessUpdated(object sender, ProcessUpdatedEventArgs e)
        {
            // Process finished
            if (e.Progress == ProcessProgress.Completed)
            {
                var process = e.Process;
                var measurement = new Measurement(MeasurementPrefix + "processes");
                measurement.Add(new DataField("id", process.Id));
                var success = process is ProductionProcess prodProcess && prodProcess.ProductInstance.State == ProductInstanceState.Success;
                measurement.Add(new DataField("success", success));

                var firstActivity = process.GetActivity(ActivitySelectionType.First);
                var lastActivity = process.GetActivity(ActivitySelectionType.Last);

                if (firstActivity != null && lastActivity != null)
                {
                    var cycleTime = lastActivity.Tracing.Completed - firstActivity.Tracing.Started;
                    if (cycleTime.HasValue)
                        measurement.Add(new DataField("cycleTime", cycleTime.Value));
                }

                _processBindingProcessor.Apply(measurement, process);

                ProcessDataMonitor.Add(measurement);
            }
        }

        private void OnActivityUpdated(object sender, ActivityUpdatedEventArgs args)
        {
            // Activity finished
            if (args.Progress == ActivityProgress.Completed && args.Activity.Result != null)
            {
                var activity = args.Activity;
                var process = activity.Process;

                var measurement = new Measurement(MeasurementPrefix + "activities");
                measurement.Add(new DataField("id", activity.Id));
                measurement.Add(new DataField("success", activity.Result.Success));
                measurement.Add(new DataField("process", process.Id));

                if (activity.Tracing.Started.HasValue && activity.Tracing.Completed.HasValue)
                {
                    var runtimeTimespan = activity.Tracing.Completed - activity.Tracing.Started;
                    measurement.Add(new DataField("runtimeMs", runtimeTimespan.Value));
                }

                var lastActivity = process.GetActivity(ActivitySelectionType.LastOrDefault, a => a.Id == activity.Id - 1);
                if (lastActivity != null && lastActivity.Tracing.Completed.HasValue &&
                    activity.Tracing.Started.HasValue)
                {
                    var waitTime = activity.Tracing.Started - lastActivity.Tracing.Completed;
                    measurement.Add(new DataField("waittimeMS", waitTime.Value));
                }

                measurement.Add(new DataTag("type", activity.GetType().Name));

                _activityBindingProcessor.Apply(measurement, activity);

                ProcessDataMonitor.Add(measurement);
            }
        }
    }
}
