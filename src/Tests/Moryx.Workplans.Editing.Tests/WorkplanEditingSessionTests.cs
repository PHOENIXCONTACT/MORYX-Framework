// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using NUnit.Framework;
using Moryx.Workplans.Editing.Implementation;
using Moryx.Tests.Workplans;
using System.Linq;

namespace Moryx.Workplans.Editing.Tests
{
    [TestFixture]
    public class WorkplanEditingSessionTests
    {
        /// <summary>
        /// Tests for WorkplanEditingSession
        /// </summary>

        private WorkplanEditingSession _editingSession;
        private WorkplanDummy _workplan;
        
        [SetUp]
        public void SetUp() {
            //Arrange
            _workplan = new WorkplanDummy();
            _editingSession = new WorkplanEditingSession(_workplan);
        }

        [Test(Description ="Check if the Connector gets removed when trying to create a duplicate connection")]
        public void StartConnectorExistsAfterConnectingAgain()
        {
            // Act
            _editingSession.Connect( _workplan.StartConnector, 0, _workplan.StepA, 0);

            // Assert
            Assert.That(_workplan.Connectors.Any(c =>  c.Classification.Equals(_workplan.StartConnector.Classification)));
        }
        [Test(Description = "Check if start gets replaced when connecting start somewhere else")]
        public void StartConnectorGetsReplaced()
        {
            //Act
            _editingSession.Connect(_workplan.StartConnector, 0, _workplan.StepC, 0);
            _editingSession.Connect(_workplan.StartConnector, 0, _workplan.StepA, 0);

            //Assert
            Assert.That(HasLoseConnectors(_workplan), Is.False);
            Assert.That(_workplan.StepC.Inputs[0], Is.EqualTo(_workplan.StepA.Outputs[0]));
            Assert.That(_workplan.StepC.Inputs[0], Is.EqualTo(_workplan.StepB.Outputs[0]));
        }

        [Test(Description ="Check that the connections do not get changed when connecting somewhere else and connecting back again")]
        public void ConnectionSwitchesOverAndBack()
        {
            // Act
            _editingSession.Connect(_workplan.StepA, 1, _workplan.StepC, 0);
            
            //Assert
            Assert.That(_workplan.StepB.Inputs[0], Is.Null);
            Assert.That(HasLoseConnectors(_workplan), Is.False);

            // Act
            _editingSession.Connect(_workplan.StepA, 1, _workplan.StepB, 0);

            //Assert
            Assert.That(HasLoseConnectors(_workplan), Is.False);
            Assert.That(_workplan.StepC.Inputs[0], Is.EqualTo(_workplan.StepA.Outputs[0]));
            Assert.That(_workplan.StepC.Inputs[0], Is.EqualTo(_workplan.StepB.Outputs[0]));
        }

        [Test(Description = "Nothing should change when trying to establish a connection that already exists")]
        public void ReconnectingNotChangingConnections()
        {
            // Act
            _editingSession.Connect(_workplan.StepA, 0, _workplan.StepC, 0);

            //Assert
            Assert.That(HasLoseConnectors(_workplan), Is.False);
            Assert.That(_workplan.StepC.Inputs[0], Is.EqualTo(_workplan.StepA.Outputs[0]));
            Assert.That(_workplan.StepC.Inputs[0], Is.EqualTo(_workplan.StepB.Outputs[0]));
            
        }

        private static bool HasLoseConnectors(WorkplanDummy workplan)
        {
            foreach (var connector in workplan.Connectors)
            {
                if (connector.Classification != NodeClassification.Intermediate)
                    continue;
                if (!workplan.Steps.Any(s => s.Outputs.Contains(connector)) && !workplan.Steps.Any(s => s.Inputs.Contains(connector)))
                    return true;
            }
            return false;
        }
    }
}

