// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Bindings;
using Moryx.ProcessData.Configuration;

namespace Moryx.ProcessData.Bindings
{
    /// <summary>
    /// Applies a collection of measurement binding configurations and applies them to the given measurement
    /// </summary>
    public class MeasurementBindingProcessor
    {
        private readonly ResolverDescription[] _resolvers;

        /// <summary>
        /// Creates a new instance of the <see cref="MeasurementBindingProcessor"/>
        /// </summary>
        public MeasurementBindingProcessor(IBindingResolverFactory resolverFactory, IEnumerable<MeasurementBinding> bindings)
        {
            _resolvers = bindings.Select(config => new ResolverDescription
            {
                Name = config.Name,
                BindingResolver = resolverFactory.Create(config.Binding),
                IsField = config.ValueTarget == ValueTarget.Field
            }).ToArray();
        }

        /// <summary>
        /// Applies the binding to the given measurement
        /// </summary>
        public void Apply(Measurement measurement, object source)
        {
            foreach (var resolver in _resolvers)
            {
                var value = resolver.BindingResolver.Resolve(source);
                if (value == null)
                    continue;

                if (resolver.IsField)
                    measurement.Add(new DataField(resolver.Name, value));
                else
                    measurement.Add(new DataTag(resolver.Name, value.ToString()));
            }
        }

        private class ResolverDescription
        {
            public string Name { get; set; }

            public IBindingResolver BindingResolver { get; set; }

            public bool IsField { get; set; }
        }
    }
}