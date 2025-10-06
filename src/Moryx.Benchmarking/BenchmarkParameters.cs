// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.VisualInstructions;

namespace Moryx.Benchmarking
{
    public class BenchmarkParameters : VisualInstructionParameters, IActivityTimeoutParameters
    {
        /// <summary>
        /// Step of the benchmark
        /// </summary>
        public int Step { get; set; }

        /// <inheritdoc />
        public int Timeout { get; set; }

        /// <summary>
        /// Creates new <see cref="BenchmarkParameters"/> with default instructions
        /// </summary>
        public BenchmarkParameters()
        {
            Instructions = ["Please select the result for Step {0}:".AsInstruction()];
        }

        /// <inheritdoc />
        protected override void Populate(IProcess process, Parameters instance)
        {
            var parameters = (BenchmarkParameters)instance;
            parameters.Step = Step;
            parameters.Instructions = [string.Format(Instructions[0].Content, Step).AsInstruction()];
        }
    }
}
