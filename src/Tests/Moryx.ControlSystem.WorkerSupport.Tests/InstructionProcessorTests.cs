// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.VisualInstructions;
using Moryx.ControlSystem.WorkerSupport;
using NUnit.Framework;

namespace Moryx.Resources.AssemblyInstruction.Tests
{
    [TestFixture]
    public class InstructionProcessorTests
    {
        private const string Input = "35afe06d-8393-4559-b6d7-74d35ce131d8|Master";

        private const string GuidPattern = @"(?<guid>[a-f0-9]*(?:-[a-f0-9]*){4})\|(?<variant>\w+)";

        private const string GuidContentReplacement = @"moryx://media/guid/${guid}?v=${variant}";

        private const string GuidPreviewReplacement = @"http://moryx-server/media/preview/${guid}";

        [TestCase(true, false, true, false, Description = "Replace content without preview from content")]
        [TestCase(false, true, false, true, Description = "Replace preview without content from preview")]
        [TestCase(true, false, true, true, Description = "Replace content and preview from content")]
        [TestCase(false, true, true, true, Description = "Replace content and preview from preview")]
        [TestCase(true, true, true, true, Description = "Replace content and preview from their own fields")]
        public void GuidReplacements(bool contentPattern, bool previewPattern, bool contentReplacement, bool previewReplacement)
        {
            // Arrange
            var config = new InstructionProcessorConfig();
            if (contentPattern)
                config.ContentPattern = GuidPattern;
            if (previewPattern)
                config.PreviewPattern = GuidPattern;
            if (contentReplacement)
                config.ContentReplacement = GuidContentReplacement;
            if (previewReplacement)
                config.PreviewReplacement = GuidPreviewReplacement;
            var processor = new InstructionProcessor(config);

            // Act
            var item = new VisualInstruction
            {
                Content = Input,
                Preview = Input
            };
            processor.ProcessItem(item);

            // Assert
            if (contentReplacement)
                Assert.That(item.Content, Is.EqualTo(@"moryx://media/guid/35afe06d-8393-4559-b6d7-74d35ce131d8?v=Master"));
            if (previewReplacement)
                Assert.That(item.Preview, Is.EqualTo(@"http://moryx-server/media/preview/35afe06d-8393-4559-b6d7-74d35ce131d8"));
        }
    }
}
