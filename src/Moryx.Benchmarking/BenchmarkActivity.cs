// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Identity;
using Moryx.ControlSystem.Activities;

namespace Moryx.Benchmarking
{
    [ActivityResults(typeof(BenchmarkResult))]
    public class BenchmarkActivity : Activity<BenchmarkParameters, BenchmarkTracing>, IInstanceModificationActivity
    {
        /// <inheritdoc />
        public IIdentity InstanceIdentity { get; set; }

        /// <inheritdoc />
        public InstanceModificationType ModificationType { get; set; }

        /// <inheritdoc />
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.NotRequired;

        /// <inheritdoc />
        public override ICapabilities RequiredCapabilities => new BenchmarkCapabilities(Parameters.Step);

        /// <summary>
        /// Create a typed result object for this result number
        /// </summary>
        protected override ActivityResult CreateResult(long resultNumber)
        {
            if (Parameters.Step > 1)
            {
                ModificationType = InstanceModificationType.None;
            }
            else
            {
                switch ((BenchmarkResult)resultNumber)
                {
                    case BenchmarkResult.Success:
                        ModificationType = InstanceModificationType.Created;
                        break;
                    case BenchmarkResult.Rework:
                        ModificationType = InstanceModificationType.Loaded;
                        break;
                    default:
                        ModificationType = InstanceModificationType.None;
                        break;
                }
            }

            return ActivityResult.Create((BenchmarkResult)resultNumber);
        }

        /// <summary>
        /// Create a typed result object for a technical failure.
        /// </summary>
        protected override ActivityResult CreateFailureResult()
        {
            throw new NotImplementedException();
        }
    }
}
