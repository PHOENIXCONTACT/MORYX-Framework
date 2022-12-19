// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
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
            _inputs = new IPlace[] { new Place { Id = 10 }, new Place { Id = 11 } };
            _outputs = new IPlace[] { new Place { Id = 20 }, new Place { Id = 21 } };
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
                Inputs = new[] { _inputs[0] },
                Outputs = _outputs
            };

            // Act
            trans.Initialize();
            _inputs[0].Add(_token);

            // Assert
            Assert.AreEqual(0, _inputs[0].Tokens.Count());
            Assert.IsTrue(_outputs.All(o => o.Tokens.Count() == 1));
            Assert.IsInstanceOf<SplitToken>(_outputs[0].Tokens.First());
            Assert.IsInstanceOf<SplitToken>(_outputs[1].Tokens.First());
            Assert.AreEqual(_token, ((SplitToken)_outputs[0].Tokens.First()).Original);
            Assert.AreEqual(_token, ((SplitToken)_outputs[1].Tokens.First()).Original);
        }

        [Test]
        public void JoinTransition()
        {
            // Arrange
            var trans = new JoinTransition
            {
                Id = 1,
                Inputs = _inputs,
                Outputs = new[] { _outputs[0] }
            };
            var split1 = new SplitToken(_token);
            var split2 = new SplitToken(_token);

            // Act
            trans.Initialize();
            _inputs[0].Add(split1);
            _inputs[1].Add(split2);

            // Assert
            Assert.IsTrue(_inputs.All(i => !i.Tokens.Any()));
            Assert.AreEqual(1, _outputs[0].Tokens.Count());
            Assert.AreEqual(_token, _outputs[0].Tokens.First());
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
                Outputs = new[] { _outputs[0] }
            };
            var split = new SplitToken(_token);

            // Act
            trans.Initialize();
            _inputs[index].Add(split);

            // Assert
            Assert.AreEqual(1, _inputs[index].Tokens.Count());
            Assert.AreEqual(0, _inputs[(index + 1) % 2].Tokens.Count());
            Assert.AreEqual(0, _outputs[0].Tokens.Count());
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
                Inputs = new[] { _inputs[0] },
                Outputs = _outputs
            };
            var triggered = new List<ITransition>();

            // Act
            trans.Initialize();
            trans.Triggered += (sender, args) => triggered.Add((ITransition)sender);
            _inputs[0].Add(_token);

            // Assert
            Assert.AreEqual(0, _inputs[0].Tokens.Count());
            Assert.AreEqual(_token, _outputs[0].Tokens.First());
            Assert.AreEqual(2, triggered.Count);
            Assert.IsTrue(triggered.All(t => t is DummyTransition));
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
                Inputs = new[] { _inputs[0] },
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
            Assert.AreEqual(0, _inputs[0].Tokens.Count());
            Assert.AreEqual(_token, _outputs[0].Tokens.First());
            Assert.AreEqual(1, triggered.Count);
            Assert.IsInstanceOf<WorkplanSnapshot>(state);
            var snapshot = (WorkplanSnapshot)state;
            Assert.AreEqual(1, snapshot.Holders.Length);
            var stepId = workplan.Steps.First(s => s is PausableStep).Id;
            Assert.AreEqual(stepId, snapshot.Holders[0].HolderId);
        }
    }
}
