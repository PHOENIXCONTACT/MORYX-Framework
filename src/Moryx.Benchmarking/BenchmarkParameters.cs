// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.VisualInstructions;

namespace Moryx.Benchmarking
{
    public class BenchmarkParameters : Parameters, IVisualInstructions, IActivityTimeoutParameters
    {
        /// <summary>
        /// Step of the benchmark
        /// </summary>
        public int Step { get; set; }

        /// <inheritdoc />
        public VisualInstruction[] Instructions { get; set; } =
        {
            new VisualInstruction
            {
                Type = InstructionContentType.Text,
                Content = "Please select the result for Step {0}:"
            }
        };

        /// <inheritdoc />
        public int Timeout { get; set; }

        /// <inheritdoc />
        protected override void Populate(IProcess process, Parameters instance)
        {
            var parameters = (BenchmarkParameters) instance;
            parameters.Step = Step;
            parameters.Instructions = new[]
            {
                new VisualInstruction
                {
                    Type = Instructions[0].Type,
                    Content = string.Format(Instructions[0].Content, Step)
                }
            };
        }
    }
}
