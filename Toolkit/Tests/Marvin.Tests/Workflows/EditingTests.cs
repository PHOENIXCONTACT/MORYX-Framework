using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Marvin.Model;
using Marvin.Serialization;
using Marvin.Workflows;
using Marvin.Workflows.WorkplanSteps;
using NUnit.Framework;

namespace Marvin.Tests.Workflows
{
    [TestFixture]
    public class EditingTests
    {
        private readonly IWorkplanSource _dummySource = new DummyProvider {Target = WorkplanDummy.CreateSub()};

        [TestCase("", 2, 0, Description = "Start editing a new workplan")]
        [TestCase("empty", 0, 0, Description = "Start editing an empty workplan")]
        [TestCase("full", 4, 3, Description = "Start editing a complete workplan")]
        [TestCase("lone", 4, 3, Description = "Start editing workplan with unreachable step")]
        [TestCase("dead", 4, 3, Description = "Start editing workplan with step with open end")]
        [TestCase("sub", 2, 1, Description = "Workplan with sub workplan reference")]
        public void StartEditing(string type, int conCount, int stepCount)
        {
            // Arrange
            Workplan workplan;
            switch (type)
            {
                case "empty":
                    workplan = new WorkplanDummy();
                    break;
                case "full":
                    workplan = WorkplanDummy.CreateFull();
                    break;
                case "lone":
                    workplan = WorkplanDummy.CreateLoneWolf();
                    break;
                case "dead":
                    workplan = WorkplanDummy.CreateDeadEnd();
                    break;
                case "sub":
                    workplan = WorkplanDummy.CreateWithSub();
                    break;
                default:
                    workplan = null;
                    break;
            }

            // Act
            var editing = workplan == null ? Workflow.CreateWorkplan(_dummySource): Workflow.EditWorkplan(workplan, _dummySource);
            var session = editing.ExportSession();

            // Assert
            Assert.NotNull(editing, "Failed to open editing mode");
            var export = editing.Finish();
            Assert.NotNull(export, "No workplan received in finish call");
            Assert.AreEqual(conCount, session.Connectors.Count, "Number of connectos invalid");
            Assert.AreEqual(stepCount, session.Steps.Count, "Number of steps invalid");
            if (workplan != null)
            {
                Assert.AreEqual(workplan, export, "Workplan not preserved");
            }
            if (type == "sub")
            {
                Assert.AreEqual(StepClassification.Subworkplan, session.Steps[0].Classification);
                Assert.AreEqual(42, session.Steps[0].SubworkplanId);
            }
        }

        [Test]
        public void StepExport()
        {
            // Arrange
            var types = new[] { typeof(SplitWorkplanStep), typeof(JoinWorkplanStep), typeof(DummyStep) };
            var editing = Workflow.CreateWorkplan(_dummySource);

            // Act
            editing.SetAvailableTypes(types);
            var session = editing.ExportSession();

            // Assert
            Assert.AreEqual(3, session.AvailableSteps.Length, "Nuber of steps does not match");
            for (var i = 0; i < 3; i++)
            {
                var step = session.AvailableSteps[i];
                Assert.AreEqual(i, step.Index, "Index not properly set");
                Assert.AreEqual(types[i].Name, step.Name, "Name not set");
            }
        }

        [Test(Description = "Check if step without valid constructor throws exception")]
        public void InvalidStep()
        {
            // Arrange
            var editing = Workflow.CreateWorkplan(_dummySource);

            // Act
            Assert.Throws<ArgumentException>(() => editing.SetAvailableTypes(typeof(InvalidStep)));
        }

        [TestCase(typeof(ParameterStep), Description = "Read parameter property")]
        [TestCase(typeof(ParameterConstructorStep), Description = "Read paramter from constructor")]
        public void StepParameterExport(Type paramStepType)
        {
            // Arrange
            var editing = Workflow.CreateWorkplan(_dummySource);

            // Act
            editing.SetAvailableTypes(paramStepType);
            var session = editing.ExportSession();

            // Assert
            Assert.AreEqual(1, session.AvailableSteps.Length, "Step not resolved");
            var step = session.AvailableSteps[0];
            Assert.AreEqual(1, step.Initializers.Length);
            var param = step.Initializers[0];
            Assert.AreEqual(2, param.SubEntries.Count);
            Assert.AreEqual("Number", param.SubEntries[0].Key.Name);
            Assert.AreEqual("Name", param.SubEntries[1].Key.Name);
        }

        //[Test]
        // TODO: Add platform 3 samples again
        //public void MultiLevelGenericParamers()
        //{
        //    // Arrange
        //    var editing = Workflow.CreateWorkplan(_dummySource);

        //    // Act
        //    editing.SetAvailableTypes(typeof(AssignSerialTask));
        //    var session = editing.ExportSession();

        //    Assert.AreEqual(1, session.AvailableSteps.Length, "Step not resolved");
        //    var step = session.AvailableSteps[0];
        //    Assert.AreEqual(1, step.Initializers.Length);
        //    var param = step.Initializers.First(i => i.Key.Identifier == "Parameters");
        //    Assert.AreEqual(3, param.SubEntries.Count, "Not all properties found");
        //}

        [TestCase(typeof(ParameterStep), Description = "Assign values to property")]
        [TestCase(typeof(ParameterConstructorStep), Description = "Pass values to constructor")]
        public void InstantiateParameterStep(Type paramStepType)
        {
            // Arrange
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(paramStepType);
            var session = editing.ExportSession();
            var step = session.AvailableSteps[0];

            // Act
            step.Initializers[0].SubEntries[0].Value.Current = "10";
            if (paramStepType == typeof(ParameterConstructorStep))
                step.Initializers[0].SubEntries[1].Value.Current = "Thomas";
            var summary = editing.AddStep(step);
            var workplan = editing.Finish();

            // Assert
            Assert.AreEqual(1, workplan.Steps.Count());
            var created = workplan.Steps.First();
            Assert.IsInstanceOf<IParameterHolder>(created);
            var paramHolder = (IParameterHolder)created;
            var parameters = paramHolder.Export();
            Assert.AreEqual(10, parameters.Number);
            if (paramStepType == typeof(ParameterConstructorStep))
                Assert.AreEqual("Thomas", parameters.Name);
            else
                Assert.Null(parameters.Name);

            AssertSerialize(summary);
        }

        // TODO: Add platform 3 samples again
        //[TestCase(2, Description = "Test adding mount step")]
        //[TestCase(0, Description = "Test adding mount step without instructions")]
        //public void MountTask(int amount)
        //{
        //    // Arrange
        //    var editing = Workflow.CreateWorkplan(_dummySource);
        //    editing.SetAvailableTypes(typeof(MountTask));
        //    var session = editing.ExportSession();
        //    var step = session.AvailableSteps[0];

        //    // Act
        //    var instructions = step.Initializers.First(i => i.Key.Identifier == "Parameters").SubEntries[0];
        //    var proto = instructions.Prototypes[0];
        //    Assert.AreEqual(EntryKey.ProtoIdentifier, proto.Key.Identifier);
        //    // One text entry
        //    if (amount > 0)
        //    {
        //        var entry = proto.Instantiate();
        //        entry.SubEntries[0].Value.Current = "Text";
        //        entry.SubEntries[1].Value.Current = "Hallo";
        //        instructions.SubEntries.Add(entry);
        //        // One image entry
        //        entry = proto.Instantiate();
        //        entry.SubEntries[0].Value.Current = "Image";
        //        entry.SubEntries[1].Value.Current = "C:\\Test.jpg";
        //        instructions.SubEntries.Add(entry);
        //    }
        //    var summary = editing.AddStep(step);
        //    var result = editing.Finish();

        //    // Assert
        //    Assert.AreEqual(1, result.Steps.Count(), "No step created");
        //    Assert.IsInstanceOf<MountTask>(result.Steps.First(), "Type mismatch");
        //    var mountStep = (MountTask)result.Steps.First();
        //    Assert.NotNull(mountStep.Parameters, "Parameters not set");
        //    Assert.NotNull(mountStep.Parameters.Instructions, "Instructions not created");
        //    Assert.AreEqual(amount, mountStep.Parameters.Instructions.Length);
        //    if(amount == 0)
        //        return;
        //    var param = mountStep.Parameters.Instructions[0];
        //    Assert.AreEqual("Hallo", param.Content);
        //    Assert.AreEqual(InstructionContentType.Text, param.Type);
        //    param = mountStep.Parameters.Instructions[1];
        //    Assert.AreEqual("C:\\Test.jpg", param.Content);
        //    Assert.AreEqual(InstructionContentType.Image, param.Type);

        //    AssertSerialize(summary);
        //}

        [Test]
        public void UpdateStep()
        {
            // Arrange
            var workplan = new WorkplanDummy();
            var step = new ParameterStep
            {
                Parameters = new DummyParameters
                {
                    Name = "Thomas",
                    Number = 10
                }
            };
            workplan.Add(step);

            // Act
            var editing = Workflow.EditWorkplan(workplan, _dummySource);
            var session = editing.ExportSession();
            var stepModel = session.Steps.First();
            stepModel.Properties[0].SubEntries[0].Value.Current = "15";
            stepModel.Properties[0].SubEntries[1].Value.Current = "Bob";
            editing.UpdateStep(stepModel);

            // Assert
            Assert.AreEqual(15, step.Parameters.Number);
            Assert.AreEqual("Bob", step.Parameters.Name);
        }

        [Test]
        public void SubworkplanExport()
        {
            // Arrange
            var types = new[] { typeof(SubworkflowStep) };
            var editing = Workflow.CreateWorkplan(_dummySource);

            // Act
            editing.SetAvailableTypes(types);
            var session = editing.ExportSession();

            // Assert
            Assert.AreEqual(1, session.AvailableSteps.Length);
            var step = session.AvailableSteps.First();
            Assert.AreEqual(types[0].Name, step.Name);
            Assert.AreEqual(1, step.Initializers.Length);
            var wpInit = step.Initializers[0];
            Assert.IsTrue(wpInit.SubWorkplan);
            Assert.AreEqual(EntryValueType.Int64, wpInit.Value.Type);
        }

        [Test]
        public void SubworkplanCreate()
        {
            // Arrange
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(typeof(SubworkflowStep));
            var session = editing.ExportSession();

            // Act
            var step = session.AvailableSteps[0];
            step.Initializers[0].Value.Current = "1";
            editing.AddStep(step);
            var workplan = editing.Finish();

            // Assert
            Assert.NotNull(workplan);
            Assert.AreEqual(1, workplan.Steps.Count());
            var subStep = workplan.Steps.First() as SubworkflowStep;
            Assert.NotNull(subStep);
            Assert.AreEqual(2, subStep.Outputs.Length);
            Assert.AreEqual(2, subStep.OutputDescriptions.Length);
            Assert.AreEqual("End", subStep.OutputDescriptions[0].Name);
            Assert.AreEqual("Failed", subStep.OutputDescriptions[1].Name);
        }
        private class DummyProvider : IWorkplanSource
        {
            public IWorkplan Target { get; set; }

            public IWorkplan Load(long id)
            {
                return Target;
            }
        }

        [Test]
        public void ConstructorOverload()
        {
            // Arrange 
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(typeof(DummyStep));

            // Act
            var session = editing.ExportSession();

            // Assert
            Assert.AreEqual(1, session.AvailableSteps.Length);
            var step = session.AvailableSteps[0];
            Assert.AreEqual(2, step.Initializers.Count(i => i.FromConstructor), "Did not use constructor with most arguments");
            Assert.AreEqual("outputs", step.Initializers[0].Key.Identifier);
            Assert.AreEqual("name", step.Initializers[1].Key.Identifier);
        }

        [Test]
        public void DefaultParameter()
        {
            // Arrange 
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(typeof(DefaultValueStep));

            // Act
            var session = editing.ExportSession();

            // Assert
            Assert.AreEqual(1, session.AvailableSteps.Length);
            var step = session.AvailableSteps[0];
            Assert.AreEqual(4, step.Initializers.Length, "Insufficient number of parameters");
            // Constructor parameters
            Assert.IsTrue(step.Initializers[0].FromConstructor & step.Initializers[1].FromConstructor, "Constructor parameters not flagged as required");
            Assert.IsNull(step.Initializers[0].Value.Default);
            Assert.NotNull(step.Initializers[1].Value.Default);
            Assert.AreEqual("2", step.Initializers[1].Value.Default);
            // Properties
            Assert.IsFalse(step.Initializers[2].FromConstructor | step.Initializers[3].FromConstructor, "Properties not flagged as optional");
            Assert.NotNull(step.Initializers[2].Value.Default, "Value types always have a default");
            Assert.NotNull(step.Initializers[3].Value.Default, "This property should have default value");
            Assert.AreEqual("10", step.Initializers[3].Value.Default, "Default value does not match");
        }

        [Test]
        public void AddStep()
        {
            // Arrange 
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(typeof(DummyStep));
            var session = editing.ExportSession();
            var dummyRecipe = session.AvailableSteps[0];

            // Act
            dummyRecipe.Initializers[0].Value.Current = "2";
            dummyRecipe.Initializers[1].Value.Current = "Test";
            var mod = editing.AddStep(dummyRecipe);
            var result = editing.Finish();

            // Assert
            Assert.NotNull(mod, "Event was not raised");
            Assert.AreEqual(1, mod.StepModifications.Length);
            Assert.AreEqual(ModificationType.Insert, mod.StepModifications[0].Modification);

            Assert.AreEqual(1, result.Steps.Count());
            var step = result.Steps.First();
            Assert.IsInstanceOf<DummyStep>(step);
            var dummy = (DummyStep)step;
            Assert.AreEqual(2, dummy.Outputs.Length, "Outputs not set");
            Assert.AreEqual("Test", dummy.Name, "Name not set");

            AssertSerialize(mod);
        }

        [Test]
        public void AddConnector()
        {
            // Arrange 
            var editing = Workflow.CreateWorkplan(_dummySource);

            // Act
            var summary = editing.AddConnector(new ConnectorDto { Name = "Entry", Classification = NodeClassification.Entry });
            var workplan = editing.Finish();

            // Assert
            Assert.NotNull(summary);
            Assert.AreEqual(1, summary.ConnectorModifications.Length);
            Assert.AreEqual(3, summary.ConnectorModifications[0].Data.Id);
            Assert.AreEqual("Entry", summary.ConnectorModifications[0].Data.Name);
            Assert.AreEqual(NodeClassification.Entry, summary.ConnectorModifications[0].Data.Classification);

            Assert.AreEqual(3, workplan.Connectors.Count());
            var con = workplan.Connectors.ElementAt(2);
            Assert.AreEqual(3, con.Id);
            Assert.AreEqual("Entry", con.Name);
            Assert.AreEqual(NodeClassification.Entry, con.Classification);

            AssertSerialize(summary);
        }

        [Test]
        public void ConnectSteps()
        {
            // Arrange
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(typeof(DummyStep));
            var session = editing.ExportSession();
            var dummyRecipe = session.AvailableSteps[0];

            // Act
            dummyRecipe.Initializers[0].Value.Current = "1";
            dummyRecipe.Initializers[1].Value.Current = "Test";

            editing.AddStep(dummyRecipe);
            editing.AddStep(dummyRecipe);
            editing.Connect(new ConnectionPoint { NodeId = 3, Index = 0 }, new ConnectionPoint { NodeId = 4, Index = 0 });

            var plan = editing.Finish();

            // Assert
            Assert.AreEqual(2, plan.Steps.Count(), "Did not create two steps");
            Assert.AreEqual(plan.Steps.First().Outputs[0], plan.Steps.ElementAt(1).Inputs[0], "Steps not connected");
        }

        [Test]
        public void ConnectStartAndStep()
        {
            // Arrange
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(typeof(DummyStep));
            var session = editing.ExportSession();
            var dummyRecipe = session.AvailableSteps[0];

            // Act
            dummyRecipe.Initializers[0].Value.Current = "1";
            dummyRecipe.Initializers[1].Value.Current = "Test";
            editing.AddStep(dummyRecipe);

            editing.Connect(new ConnectionPoint { NodeId = 1, IsConnector = true }, new ConnectionPoint { NodeId = 3, Index = 0 });

            var plan = editing.Finish();

            // Assert
            Assert.AreEqual(2, plan.Connectors.Count());
            Assert.AreEqual(1, plan.Steps.Count());
            var step = plan.Steps.First();
            Assert.AreEqual(step.Inputs[0], plan.Connectors.First());
        }

        [Test]
        public void RemoveStep()
        {
            // Arrange
            var editing = Workflow.CreateWorkplan(_dummySource);
            editing.SetAvailableTypes(typeof(DummyStep));
            var session = editing.ExportSession();
            var dummyRecipe = session.AvailableSteps[0];
            dummyRecipe.Initializers[0].Value.Current = "1";
            dummyRecipe.Initializers[1].Value.Current = "Test";
            editing.AddStep(dummyRecipe);

            // Act
            var summary = editing.RemoveStep(3);
            var workplan = editing.Finish();

            // Assert
            Assert.NotNull(summary, "Event not raised");
            Assert.AreEqual(0, workplan.Steps.Count());
            Assert.AreEqual(1, summary.StepModifications.Length);
            Assert.AreEqual(ModificationType.Delete, summary.StepModifications[0].Modification);

            AssertSerialize(summary);
        }

        /// <summary>
        /// Make sure object can be serialized
        /// </summary>
        private void AssertSerialize<T>(T obj)
        {
            var serializer = new DataContractSerializer(typeof (T));

            using (var memStream = new MemoryStream())
            {
                serializer.WriteObject(memStream, obj);
                memStream.Position = 0;
                var result = serializer.ReadObject(memStream);
            }
        }
    }
}