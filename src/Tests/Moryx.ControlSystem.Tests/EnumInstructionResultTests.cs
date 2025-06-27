// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.VisualInstructions;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Moryx.ControlSystem.Tests
{
    [TestFixture]
    public class EnumInstructionResultTests
    {
        private enum TestResults1
        {
            Value1,
            Value2
        }

        [Test]
        public void GetAllValuesIfNoResultIsDecorated()
        {
            // Act
            var instructionResult = EnumInstructionResult.PossibleResults(typeof(TestResults1));
            var possibleResults = EnumInstructionResult.PossibleInstructionResults(typeof(TestResults1));

            // Assert
            Assert.AreEqual(2, instructionResult.Count(), "There should be 2 results because all of the results are not decorated");
            Assert.AreEqual(2, possibleResults.Count(), "There should be 2 results because all of the results are not decorated");
        }

        private enum TestResults2
        {
            [EnumInstruction, Display(Name = "Value1")]
            Value1,
            [EnumInstruction, Display(Name = "Value2")]
            Value2
        }

        [Test]
        public void GetAllValuesIfAllResultsAreDecorated()
        {
            // Act
            var instructionResult = EnumInstructionResult.PossibleResults(typeof(TestResults2));

            // Assert
            Assert.AreEqual(2, instructionResult.Count, "There should be 2 results because all of the results are decorated");
        }

        private enum TestResults3
        {
            [EnumInstruction, Display(Name = "Value1")]
            Value1,
            Value2
        }

        [Test]
        public void GetValuesWithAnAttribute()
        {
            // Act
            var instructionResult = EnumInstructionResult.PossibleResults(typeof(TestResults3));

            // Assert
            Assert.AreEqual(1, instructionResult.Count, "There should be 1 result because only one value is decorated");
        }

        private enum TestResults4
        {
            [EnumInstruction(Hide = true)]
            Value1,
            Value2
        }

        [Test]
        public void GetNoneHiddenValue()
        {
            // Act
            var instructionResult = EnumInstructionResult.PossibleResults(typeof(TestResults4));

            // Assert
            Assert.AreEqual(1, instructionResult.Count, "There should be one result, the non-decorated non-hidden enum value");
        }

        private enum TestResults5
        {
            [EnumInstruction(Hide = true)]
            Value1,
            [EnumInstruction(Hide = true)]
            Value2
        }

        [Test]
        public void GetNoValuesIfAllResultsAreHidden()
        {
            // Act
            var instructionResult = EnumInstructionResult.PossibleResults(typeof(TestResults5));

            // Assert
            Assert.AreEqual(0, instructionResult.Count, "There should be no results because all of them are hidden");
        }

        [Test]
        public void ParseResponse()
        {
            // Arrange
            var instructionResult = EnumInstructionResult.PossibleResults(typeof(TestResults1));


            // Act
            var enumValue = (TestResults1)EnumInstructionResult.ResultToEnumValue(typeof(TestResults1), instructionResult[1]);
            var directValue = EnumInstructionResult.ResultToGenericEnumValue<TestResults1>(instructionResult[1]);

            // Assert
            Assert.AreEqual(TestResults1.Value2, enumValue);
            Assert.AreEqual(TestResults1.Value2, directValue);
        }

        private class MyInput
        {
            public int Foo { get; set; }
        }

        private enum TestResults6
        {
            [EnumInstruction, Display(Name = "Value 1")]
            Value1,
            [Display(Name = "Value 3")]
            Value3
        }

        [Test]
        public void UsesDisplayResultsWhereFound()
        {
            // Act
            var instructionResult = EnumInstructionResult.PossibleResults(typeof(TestResults6));

            // Assert
            Assert.AreEqual(1, instructionResult.Count(), "There should be two results, because one does not have the EnumInstruction attribute");
            Assert.AreEqual("Value 1", instructionResult[0].DisplayValue);
        }
    }
}
