// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Component created for a certain workplan that can predict the outcome of a single
    /// workplan instance by monitoring its workplan engine
    /// </summary>
    public interface IPathPredictor : IDisposable
    {
        /// <summary>
        /// Number of engines currently monitored by the path predictor
        /// </summary>
        int MonitoredEngines { get; }

        /// <summary>
        /// Monitor the engine of a workplan to predict its outcome
        /// </summary>
        void Monitor(IWorkplanEngine instance);

        /// <summary>
        /// Remove a engine instance, that shall be no longer be monitored
        /// </summary>
        bool Remove(IWorkplanEngine instance);

        /// <summary>
        /// The <see cref="IPathPredictor"/> has determined the most likely result of the
        /// execution of the workplan instance.
        /// </summary>
        event EventHandler<PathPredictionEventArgs> PathPrediction;
    }

    /// <summary>
    /// Event args when the engine has predicted a possible outcome of a workplan instance
    /// </summary>
    public class PathPredictionEventArgs : EventArgs
    {
        /// <summary>
        /// Create a <see cref="PathPredictionEventArgs"/> instance for the predicated outcome of a
        /// <see cref="IWorkplanEngine"/> with absolute certainty
        /// </summary>
        public PathPredictionEventArgs(IWorkplanEngine engineInstance, NodeClassification predictedOutcome)
        : this(engineInstance, predictedOutcome, 1)
        {
        }

        /// <summary>
        /// Create a <see cref="PathPredictionEventArgs"/> instance for the predicated outcome of a
        /// <see cref="IWorkplanEngine"/> with a certain probability
        /// </summary>
        public PathPredictionEventArgs(IWorkplanEngine engineInstance, NodeClassification predictedOutcome, double probability)
        {
            EngineInstance = engineInstance;
            PredictedOutcome = predictedOutcome;
            Probability = probability;
        }

        /// <summary>
        /// Instance of the workplan engine this event refers to
        /// </summary>
        public IWorkplanEngine EngineInstance { get; }

        /// <summary>
        /// Predicted outcome place of the workplan instance
        /// </summary>
        public NodeClassification PredictedOutcome { get; }

        /// <summary>
        /// Probability of the prediction. Values greater or equal <value>1.0</value> indicate
        /// absolute certainty about the result.
        /// </summary>
        public double Probability { get; }
    }
}
