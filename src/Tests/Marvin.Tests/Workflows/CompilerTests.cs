// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Workflows;
using NUnit.Framework;

namespace Marvin.Tests.Workflows
{
    [TestFixture]
    public class CompilerTests
    {
        [Test]
        public void Compile()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateFull();

            // Act
            var compiled = Workflow.Compile(workplan, new NullContext(), new DummyStepCompiler());

            // Assert
            Assert.AreEqual(4, compiled.Steps.Length);
            Assert.AreEqual("A", compiled.Steps[0].Name);
            Assert.AreEqual("B", compiled.Steps[1].Name);
            Assert.AreEqual("C", compiled.Steps[2].Name);
            Assert.AreEqual("End", compiled.Steps[3].Name);
            var expected = new[,] { { 3, 2 }, { 3, 0 }, { 4, 4 } };
            Assert.IsTrue(MatrixValidation(expected, compiled.DecisionMatrix), "Matrix not compiled correctly");
        }

        [Test]
        public void CompileMultiOutput()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateSub();

            // Act
            var compiled = Workflow.Compile(workplan, new NullContext(), new DummyStepCompiler());

            // Assert
            Assert.AreEqual(4, compiled.Steps.Length);
            var expected = new[,] { { 2, 0 }, { 3, 4 }};
            Assert.IsTrue(MatrixValidation(expected, compiled.DecisionMatrix), "Matrix not compiled correctly");
        }

        [Test]
        public void CompileStepConfusion()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateMixed();

            // Act
            var compiled = Workflow.Compile(workplan, new NullContext(), new DummyStepCompiler());

            // Assert
            Assert.AreEqual(3, compiled.Steps.Length);
            Assert.AreEqual("A", compiled.Steps[0].Name);
            Assert.AreEqual("B", compiled.Steps[1].Name);
            Assert.AreEqual("End", compiled.Steps[2].Name);
            var expected = new[,] { { 2 }, { 3 } };
            Assert.IsTrue(MatrixValidation(expected, compiled.DecisionMatrix), "Matrix not compiled correctly");
        }

        private static bool MatrixValidation(int[,] expected, int[,] found)
        {
            for (int stepId = 0; stepId < expected.GetLength(0); stepId++)
            {
                for (int result = 0; result < expected.GetLength(1); result++)
                {
                    if(expected[stepId, result] != found[stepId, result])
                        return false;
                }
            }
            return true;
        }
    }
}
