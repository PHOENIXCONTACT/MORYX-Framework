// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using System.Linq;
using System.Threading;
using Moryx.Tools;
using Moryx.Workplans;
using NUnit.Framework;

namespace Moryx.Tests.Workplans
{
    [TestFixture]
    public class PathPredictionTests
    {
        [Test(Description = "The predicator should not raise the event for a workplan where the outcome is unclear till the end.")]
        public void NoPredictionIfNotPossible()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateBig();
            bool triggered = false;
            var predictor = WorkplanInstance.PathPrediction(workplan);
            predictor.PathPrediction += (sender, args) => triggered = triggered = true;

            // Act
            var engine = WorkplanInstance.CreateEngine(workplan, new NullContext());
            engine.Completed += (sender, place) => { };
            engine.TransitionTriggered += (sender, transition) => { };
            predictor.Monitor(engine);
            engine.Start();

            // Assert
            Assert.That(triggered, Is.False, "Path predictor should not have been activiated");
        }

        [Test(Description = "The path predictor should predict the end classification before the workplan instance is executed completly")]
        public void PredictFailureBeforeCompletion()
        {
            // Arrange
            var stopWatch = new Stopwatch();
            var workplan = WorkplanDummy.CreateBig();
            var predictor = WorkplanInstance.PathPrediction(workplan);

            long predictionTime = long.MaxValue;
            NodeClassification prediction = NodeClassification.Intermediate;
            predictor.PathPrediction += (sender, args) =>
            {
                prediction = args.PredictedOutcome;
                predictionTime = stopWatch.ElapsedMilliseconds;
            };

            var engine = WorkplanInstance.CreateEngine(workplan, new NullContext());
            engine.ExecutedWorkplan.Transitions.OfType<DummyTransition>().ForEach(dt => dt.ResultOutput = -1); // Disable automatic execution

            // Act
            long completionTime = 0;
            var finalResult = NodeClassification.Intermediate;
            engine.Completed += (sender, place) => { completionTime = stopWatch.ElapsedMilliseconds; finalResult = place.Classification; };
            engine.TransitionTriggered += (sender, transition) => ThreadPool.QueueUserWorkItem(ResumeAsync, transition);
            predictor.Monitor(engine);
            stopWatch.Start();
            engine.Start();

            // Assert
            while (finalResult == NodeClassification.Intermediate)
                Thread.Sleep(1); // Await completion
            stopWatch.Stop();
            Assert.That(predictionTime, Is.LessThan(completionTime), "Engine was completed before a prediction was published.");
            Assert.That(prediction, Is.EqualTo(finalResult), "Predication was incorrect");
        }

        [Test(Description = "The path predictor must be able to publish a prediction when the execution of a workplan instance was interrupted in a predictable path")]
        public void PublishPredictionAfterInterruption()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateBig();
            var predictor = WorkplanInstance.PathPrediction(workplan);
            NodeClassification prediction = NodeClassification.Intermediate;
            predictor.PathPrediction += (sender, args) => prediction = args.PredictedOutcome;
            // Start and pause engine in 
            var engine = WorkplanInstance.CreateEngine(workplan, new NullContext());
            var transitions = engine.ExecutedWorkplan.Transitions.OfType<DummyTransition>();
            transitions.ForEach(dt => dt.ResultOutput = -1); // Disable automatic execution
            transitions.First().ResultOutput = 1; // Except for the first one
            engine.TransitionTriggered += (sender, transition) => { };
            engine.Start();

            // Act
            var snapshot = engine.Pause(); // Snapshot of the engine in a sure failure path
            engine.Dispose();
            engine = WorkplanInstance.CreateEngine(workplan, new NullContext());
            engine.Restore(snapshot); // Restore new engine from the snapshot
            var finalResult = NodeClassification.Intermediate;
            engine.Completed += (sender, place) => finalResult = place.Classification;
            engine.TransitionTriggered += (sender, transition) => ThreadPool.QueueUserWorkItem(ResumeAsync, transition);
            predictor.Monitor(engine);
            engine.Start(); // This should resume the engine in a failure path and directly raise the event

            // Assert
            while (finalResult == NodeClassification.Intermediate)
                Thread.Sleep(1); // Await completion
            Assert.That(prediction, Is.EqualTo(finalResult), "Predication was incorrect");
        }

        private void ResumeAsync(object state)
        {
            var transition = (DummyTransition)state;
            Thread.Sleep(100);
            transition.ResumeAsync(transition.Outputs.Length > 1 ? 1 : 0);
        }

        [Test(Description = "Predictor must be able to analyze and monitor a workplan with loops")]
        public void PredictorShouldSupportLoops()
        {
            // Arrange
            var workplan = WorkplanDummy.WithLoop();
            var predictor = WorkplanInstance.PathPrediction(workplan);
            var executor = new SingleLoopExecution();
            NodeClassification prediction = NodeClassification.Intermediate;
            predictor.PathPrediction += (sender, args) => prediction = args.PredictedOutcome;

            var engine = WorkplanInstance.CreateEngine(workplan, new NullContext());
            engine.ExecutedWorkplan.Transitions.OfType<DummyTransition>().ForEach(dt => dt.ResultOutput = -1); // Disable automatic execution

            // Act
            var finalResult = NodeClassification.Intermediate;
            engine.Completed += (sender, place) => { finalResult = place.Classification; };
            engine.TransitionTriggered += (sender, transition) => ThreadPool.QueueUserWorkItem(executor.ResumeAsync, transition);
            predictor.Monitor(engine);
            engine.Start();

            // Assert
            while (finalResult == NodeClassification.Intermediate)
                Thread.Sleep(1); // Await completion
            Assert.That(prediction, Is.EqualTo(finalResult), "Predication was incorrect");
        }

        

        private class SingleLoopExecution
        {
            private bool _loopTaken;

            public void ResumeAsync(object state)
            {
                var transition = (DummyTransition)state;
                Thread.Sleep(100);
                if(_loopTaken || transition.Name != "Set pole")
                    transition.ResumeAsync(0);
                else
                {
                    transition.ResumeAsync(1);
                    _loopTaken = true;
                }
            }
        }
    }
}
