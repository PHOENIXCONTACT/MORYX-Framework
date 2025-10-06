// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.VisualInstructions;
using NUnit.Framework;

namespace Moryx.ControlSystem.Tests.VisualInstructions
{
    [TestFixture]

    public class VisualInstructorExtensionsTests
    {
        [Test]
        public void ExtensionCreatesStringAsInstruction()
        {
            var str = "text instruction";

            var instruction = str.AsInstruction();

            Assert.That(instruction.Type, Is.EqualTo(InstructionContentType.Text));
            Assert.That(instruction.Content, Is.EqualTo(str));
        }

    }
}
