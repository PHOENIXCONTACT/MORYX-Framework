// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Workflows
{
    /// <summary>
    /// Component created for a certain workplan that can predict the outcome of a single
    /// workflow instance by monitoring its workflow engine
    /// </summary>
    public interface IPathPredictor : IDisposable
    {
        /// <summary>
        /// Number of engines currently monitored by the path predictor
        /// </summary>
        int MonitoredEngines { get; }

        /// <summary>
        /// Monitor the engine of a worklow to predict its outcome
        /// </summary>
        void Monitor(IWorkflowEngine instance);

        /// <summary>
        /// Remove a engine instance, that shall be no longer be monitored
        /// </summary>
        bool Remove(IWorkflowEngine instance);

        /// <summary>
        /// The <see cref="IPathPredictor"/> has determined the most likely result of the
        /// workflow.
        /// </summary>
        event EventHandler<PathPredictionEventArgs> PathPrediction;
    }

    /// <summary>
    /// Event args when the engine has predicted a possible outcome of a workflow
    /// </summary>
    public class PathPredictionEventArgs : EventArgs
    {
        /// <summary>
        /// Create a <see cref="PathPredictionEventArgs"/> instance for the predicated outcome of a 
        /// <see cref="IWorkflowEngine"/> with absolute certaintity
        /// </summary>
        public PathPredictionEventArgs(IWorkflowEngine engineInstance, NodeClassification predictedOutcome)
        : this(engineInstance, predictedOutcome, 1)
        {
        }

        /// <summary>
        /// Create a <see cref="PathPredictionEventArgs"/> instance for the predicated outcome of a 
        /// <see cref="IWorkflowEngine"/> with a certain probability
        /// </summary>
        public PathPredictionEventArgs(IWorkflowEngine engineInstance, NodeClassification predictedOutcome, double probability)
        {
            EngineInstance = engineInstance;
            PredictedOutcome = predictedOutcome;
            Probability = probability;
        }

        /// <summary>
        /// Instance of the workflow engine this event refers to
        /// </summary>
        public IWorkflowEngine EngineInstance { get; }

        /// <summary>
        /// Predicted outcome place of the workflow
        /// </summary>
        public NodeClassification PredictedOutcome { get; }

        /// <summary>
        /// Probability of the prediction. Values greater or equal <value>1.0</value> indicate 
        /// absolute certainty about the result.
        /// </summary>
        public double Probability { get; }
    }
}
