// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.ProcessData.Listener;
using Moryx.Tools;

namespace Moryx.ProcessData.Monitor
{
    /// <summary>
    /// Process data listener which is writing each measurement to the module logger
    /// </summary>
    [Plugin(LifeCycle.Transient, typeof(IProcessDataListener), Name = nameof(LogListener))]
    public class LogListener : ProcessDataListenerBase
    {
        /// <inheritdoc />
        protected override void OnMeasurandAdded(Measurand measurand) =>
            measurand.Measurements.ForEach(OnMeasurementAdded);

        /// <inheritdoc />
        protected override void OnMeasurementAdded(Measurement measurement)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"{measurement.TimeStamp}: {measurement.Measurand}: Values (");

            for (var index = 0; index < measurement.Fields.Count; index++)
            {
                var field = measurement.Fields[index];
                strBuilder.Append($"{field.Name}: {field}");

                if (index < measurement.Fields.Count - 1)
                    strBuilder.Append(", ");
            }

            strBuilder.Append(") Tags (");

            for (var index = 0; index < measurement.Tags.Count; index++)
            {
                var tag = measurement.Tags[index];
                strBuilder.Append($"{tag.Name}: {tag.Value}");

                if (index < measurement.Tags.Count - 1)
                    strBuilder.Append(", ");
            }

            strBuilder.Append(")");
            Logger.Log(LogLevel.Trace, strBuilder.ToString());
        }
    }
}
