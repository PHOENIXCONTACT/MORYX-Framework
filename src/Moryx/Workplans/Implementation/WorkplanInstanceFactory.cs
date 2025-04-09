﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    internal static class WorkplanInstanceFactory
    {
        public static IWorkplanInstance Instantiate(IWorkplan workplan, IWorkplanContext context)
        {
            // Prepare variables
            var places = new Dictionary<long, IPlace>();
            var transitions = new List<ITransition>();

            // Iterate over each step and find its connectors
            foreach (var step in workplan.Steps)
            {
                // Create transition
                var transition = step.CreateInstance(context);

                // Set inputs
                for (var i = 0; i < step.Inputs.Length; i++)
                {
                    transition.Inputs[i] = GetPlace(step.Inputs[i], places);
                }

                // Set outputs
                for (int i = 0; i < step.Outputs.Length; i++)
                {
                    transition.Outputs[i] = GetPlace(step.Outputs[i], places);
                }

                transitions.Add(transition);
            }

            return new SimpleWorkplanInstance(workplan, places.Values.ToList(), transitions);
        }

        private static IPlace GetPlace(IConnector connector, IDictionary<long, IPlace> cache)
        {
            IPlace instance;
            if (cache.ContainsKey(connector.Id))
                instance = cache[connector.Id];
            else
            {
                instance = connector.CreateInstance();
                cache[connector.Id] = instance;
            }
            return instance;
        }
    }
}