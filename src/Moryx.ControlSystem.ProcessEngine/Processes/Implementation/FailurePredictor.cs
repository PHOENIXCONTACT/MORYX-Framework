// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Workplans;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Uses the <see cref="IPathPredictor"/> to publish an event when a process
    /// will fail in the near future
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener), typeof(IFailurePredictor))]
    internal class FailurePredictor : IFailurePredictor, IDisposable
    {
        /// <summary>
        /// Pool with processes
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Cached instances of <see cref="IPathPredictor"/> to reduce memory load
        /// </summary>
        private readonly IDictionary<long, IPathPredictor> _predictorCache = new ConcurrentDictionary<long, IPathPredictor>();

        /// <inheritdoc />
        public int StartOrder => 100;

        /// <inheritdoc />
        public void Initialize()
        {
            ActivityPool.ProcessChanged += OnProcessChanged;
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var predictor in _predictorCache.Values)
            {
                predictor.Dispose();
            }

            _predictorCache.Clear();
        }

        private void OnProcessChanged(object sender, ProcessEventArgs processEventArgs)
        {
            switch (processEventArgs.Trigger)
            {
                case ProcessState.Initial:
                case ProcessState.Ready:
                    MonitorProcess(processEventArgs.ProcessData);
                    break;
                case ProcessState.RemoveBroken:
                    ProcessWillFail?.Invoke(this, processEventArgs.ProcessData);
                    break;
                case ProcessState.Interrupted:
                case ProcessState.Success:
                case ProcessState.Failure:
                    CleanUp(processEventArgs.ProcessData);
                    break;
            }
        }

        private void MonitorProcess(ProcessData processData)
        {
            // Only monitor production jobs
            if (!(processData.Recipe is IProductRecipe))
                return;

            var recipe = (IWorkplanRecipe)processData.Recipe;
            if (!_predictorCache.ContainsKey(recipe.Id))
            {
                var predictor = WorkplanInstance.PathPrediction(recipe.Workplan);
                predictor.PathPrediction += OnPathPrediction;
                _predictorCache[recipe.Id] = predictor;
            }

            _predictorCache[recipe.Id].Monitor(processData.Engine);
        }

        private void CleanUp(ProcessData process)
        {
            var recipe = process.Recipe;
            if (!_predictorCache.ContainsKey(recipe.Id) || !_predictorCache[recipe.Id].Remove(process.Engine) || _predictorCache[recipe.Id].MonitoredEngines > 0)
                return;

            var predictor = _predictorCache[recipe.Id];
            predictor.Dispose();
            _predictorCache.Remove(recipe.Id);
        }

        private void OnPathPrediction(object sender, PathPredictionEventArgs predictionEventArgs)
        {
            var processContext = (ProcessWorkplanContext)predictionEventArgs.EngineInstance.Context;
            var processData = ActivityPool.GetProcess(processContext.Process);

            if (predictionEventArgs.PredictedOutcome == NodeClassification.Failed)
                ProcessWillFail?.Invoke(this, processData);
        }

        public event EventHandler<ProcessData> ProcessWillFail;
    }
}
