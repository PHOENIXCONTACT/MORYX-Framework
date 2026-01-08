// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;

namespace Moryx.VisualInstructions.Tests
{
    [TestFixture]

    public class VisualInstructorExtensionsTests
    {
        [Test]
        public void ExtensionCreatesStringAsInstruction()
        {
            const string str = "text instruction";

            var instruction = str.AsInstruction();

            Assert.That(instruction.Type, Is.EqualTo(InstructionContentType.Text));
            Assert.That(instruction.Content, Is.EqualTo(str));
        }
    }
}
