// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using NUnit.Framework;

namespace Moryx.Tests.Workplans
{
    [TestFixture]
    public class TransitionTests
    {
        /// <summary>
        /// Input places for test transitions
        /// </summary>
        private IPlace[] _inputs;

        /// <summary>
        /// Outputs for test transitions
        /// </summary>
        private IPlace[] _outputs;

        /// <summary>
        /// Token passed through the transition
        /// </summary>
        private IToken _token;

        [SetUp]
        public void Prepare()
        {
            // Inputs and outputs
            _inputs = [new Place { Id = 10 }, new Place { Id = 11 }];
            _outputs = [new Place { Id = 20 }, new Place { Id = 21 }];
            foreach (var output in _outputs)
            {
                output.TokenAdded += (sender, token) => { };
            }

            // Token passed through the transition
            _token = new DummyToken();
        }

        [Test]
        public void SplitTransition()
        {
            // Arrange
            var trans = new SplitTransition
            {
                Id = 1,
                Inputs = [_inputs[0]],
                Outputs = _outputs
            };

            // Act
            trans.Initialize();
            _inputs[0].Add(_token);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_inputs[0].Tokens, Is.Empty);
                Assert.That(_outputs.Select(o => o.Tokens), Has.All.Count.EqualTo(1));
                Assert.That(_token, Is.EqualTo(((SplitToken)_outputs[0].Tokens.First()).Original));
                Assert.That(_token, Is.EqualTo(((SplitToken)_outputs[1].Tokens.First()).Original));
            });
        }

        [Test]
        public void JoinTransition()
        {
            // Arrange
            var trans = new JoinTransition
            {
                Id = 1,
                Inputs = _inputs,
                Outputs = [_outputs[0]]
            };
            var split1 = new SplitToken(_token);
            var split2 = new SplitToken(_token);

            // Act
            trans.Initialize();
            _inputs[0].Add(split1);
            _inputs[1].Add(split2);
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_inputs.All(i => !i.Tokens.Any()));
                Assert.That(_outputs[0].Tokens.Count(), Is.EqualTo(1), "The split token should be joined into one");
                Assert.That(_token, Is.EqualTo(_outputs[0].Tokens.First()));
            });
        }

        [TestCase(0, Description = "Place only one split token on the first input")]
        [TestCase(1, Description = "Place only one split token on the second input")]
        public void IncompleteJoinTransition(int index)
        {
            // Arrange
            var trans = new JoinTransition
            {
                Id = 1,
                Inputs = _inputs,
                Outputs = [_outputs[0]]
            };
            var split = new SplitToken(_token);

            // Act
            trans.Initialize();
            _inputs[index].Add(split);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_inputs[index].Tokens, Has.Count.EqualTo(1));
                Assert.That(_inputs[(index + 1) % 2].Tokens, Is.Empty);
                Assert.That(_outputs[0].Tokens, Is.Empty);
            });
        }

        [Test]
        public void SubWorkplanTransition()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateSub();
            var exits = workplan.Connectors.Where(c => c.Classification.HasFlag(NodeClassification.Exit)).ToArray();
            var outputs = new[]
            {
                new OutputDescription {MappingValue = (int) exits[0].Id},
                new OutputDescription {MappingValue = (int) exits[1].Id},
            };
            var trans = new SubworkplanTransition(WorkplanInstance.CreateEngine(workplan, new NullContext()), TransitionBase.CreateIndexResolver(outputs))
            {
                Id = 1,
                Inputs = [_inputs[0]],
                Outputs = _outputs
            };
            var triggered = new List<ITransition>();

            // Act
            trans.Initialize();
            trans.Triggered += (sender, args) => triggered.Add((ITransition)sender);
            _inputs[0].Add(_token);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_inputs[0].Tokens, Is.Empty);
                Assert.That(_token, Is.EqualTo(_outputs[0].Tokens.First()));
                Assert.That(2, Is.EqualTo(triggered.Count));
                Assert.That(triggered, Has.All.InstanceOf<DummyTransition>());
            });
        }

        [Test]
        public void SubworkplanPause()
        {
            // Arrange
            var workplan = WorkplanDummy.CreatePausableSub();
            var exits = workplan.Connectors.Where(c => c.Classification.HasFlag(NodeClassification.Exit)).ToArray();
            var outputs = new[]
            {
                new OutputDescription {MappingValue = (int) exits[0].Id},
            };
            var trans = new SubworkplanTransition(WorkplanInstance.CreateEngine(workplan, new NullContext()), TransitionBase.CreateIndexResolver(outputs))
            {
                Id = 1,
                Inputs = [_inputs[0]],
                Outputs = _outputs
            };
            var triggered = new List<ITransition>();

            // Act
            trans.Initialize();
            trans.Triggered += (sender, args) => triggered.Add((ITransition)sender);
            _inputs[0].Add(_token);
            trans.Pause();
            var state = trans.InternalState;
            trans.Resume();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_inputs[0].Tokens, Is.Empty);
                Assert.That(_token, Is.EqualTo(_outputs[0].Tokens.First()));
                Assert.That(triggered, Has.Count.EqualTo(1));
                Assert.That(state, Is.InstanceOf<WorkplanSnapshot>());
                var snapshot = (WorkplanSnapshot)state;
                Assert.That(snapshot.Holders, Has.Length.EqualTo(1));
                var stepId = workplan.Steps.First(s => s is PausableStep).Id;
                Assert.That(stepId, Is.EqualTo(snapshot.Holders[0].HolderId));
            });
        }
    }
}
