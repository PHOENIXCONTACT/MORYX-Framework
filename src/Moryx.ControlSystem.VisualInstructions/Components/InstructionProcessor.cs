// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.RegularExpressions;

namespace Moryx.ControlSystem.VisualInstructions
{
    internal class InstructionProcessor
    {
        private readonly InstructionProcessorConfig _config;

        private readonly Regex _contentRegex;

        private readonly Regex _previewRegex;

        public InstructionProcessor(InstructionProcessorConfig config)
        {
            _config = config;

            if (!string.IsNullOrEmpty(config.ContentPattern))
                _contentRegex = new Regex(config.ContentPattern);

            if (!string.IsNullOrEmpty(config.PreviewPattern))
                _previewRegex = new Regex(config.PreviewPattern);
        }

        public void ProcessItem(VisualInstruction item)
        {
            var content = item.Content;
            var preview = item.Preview;

            // Apply content pattern
            if (!string.IsNullOrEmpty(_config.ContentReplacement))
            {
                // Fill content with content regex
                if (_contentRegex != null && !string.IsNullOrEmpty(item.Content))
                    item.Content = _contentRegex.Replace(content, _config.ContentReplacement);
                // If replacement was configured but no regex, we apply the preview regex
                else if (_previewRegex != null && !string.IsNullOrEmpty(item.Preview))
                    item.Content = _previewRegex.Replace(preview, _config.ContentReplacement);
            }

            // Apply preview pattern
            if (!string.IsNullOrEmpty(_config.PreviewReplacement))
            {
                // Fill preview with preview regex
                if (_previewRegex != null && !string.IsNullOrEmpty(item.Preview))
                    item.Preview = _previewRegex.Replace(preview, _config.PreviewReplacement);
                // Fill preview with content regex and content
                else if (_contentRegex != null && !string.IsNullOrEmpty(item.Content))
                    item.Preview = _contentRegex.Replace(content, _config.PreviewReplacement);
            }
        }
    }
}
