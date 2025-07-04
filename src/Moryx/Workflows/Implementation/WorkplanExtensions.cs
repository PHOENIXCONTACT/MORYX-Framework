// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moryx.Workplans
{
    /// <summary>
    /// Extension methods for the <see cref="IWorkplan"/>
    /// </summary>
    public static class WorkplanExtensions
    {
        /// <summary>
        /// Extracts the steps of the workplan by using the first output of any step (normally the success path)
        /// </summary>
        /// <param name="workplan">The workplan</param>
        /// <returns>A sorted list of steps for the path</returns>
        public static IReadOnlyList<IWorkplanStep> ExtractPath(this IWorkplan workplan)
        {
            var startConnector = workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.Start);
            if (startConnector == null)
                throw new ArgumentException("The workplan does not have any start connector");

            return ExtractPath(workplan, startConnector, startConnector);
        }

        /// <summary>
        /// Extracts the steps of the workplan by using the first output of any step (normally the success path)
        /// </summary>
        /// <param name="workplan">The workplan</param>
        /// <param name="startConnector">Connector for starting the extraction</param>
        /// <returns>A sorted list of steps for the path</returns>
        public static IReadOnlyList<IWorkplanStep> ExtractPath(this IWorkplan workplan, IConnector startConnector)
        {
            var endConnector = workplan.Connectors.FirstOrDefault(c => c.Classification == NodeClassification.End);
            if (endConnector == null)
                throw new ArgumentException("The workplan does not have any end connector");

            return ExtractPath(workplan, startConnector, endConnector);
        }

        /// <summary>
        /// Extracts the steps of the workplan by using the first output of any step (normally the success path)
        /// </summary>
        /// <param name="workplan">The workplan</param>
        /// <param name="startConnector">Connector for starting the extraction</param>
        /// <param name="endConnector">Connector for ending the extraction</param>
        /// <returns>A sorted list of steps for the path</returns>
        public static IReadOnlyList<IWorkplanStep> ExtractPath(this IWorkplan workplan, IConnector startConnector, IConnector endConnector)
        {
            if (!workplan.Connectors.Contains(startConnector))
                throw new ArgumentException("The start connector is not part of the workplan");

            if (!workplan.Connectors.Contains(endConnector))
                throw new ArgumentException("The end connector is not part of the workplan");

            var path = new List<IWorkplanStep>();

            var currentConnector = startConnector;
            while (currentConnector != null)
            {
                var step = workplan.Steps.First(s => s.Inputs.Contains(currentConnector));
                path.Add(step);

                var output = step.Outputs[0];
                if (output == endConnector)
                    break;

                currentConnector = output;
            }

            return path;
        }
    }
}

