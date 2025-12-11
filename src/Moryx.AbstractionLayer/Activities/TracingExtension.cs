// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Activities
{
    /// <summary>
    /// Extension to override tracing information
    /// </summary>
    public static class TracingExtension
    {
        /// <summary>
        /// Transform tracing type on activity
        /// </summary>
        public static T TransformTracing<T>(this Activity activity) where T : Tracing, new()
        {
            var baseType = (Activity)activity;
            var tracing = baseType.Tracing.Transform<T>();
            baseType.Tracing = tracing;
            return tracing;
        }

        /// <summary>
        /// Add a trace information to the tracing object
        /// </summary>
        /// <param name="activityTracing">Tracing to add information to</param>
        /// <param name="setter">Setter delegate</param>
        public static T Trace<T>(this T activityTracing, Action<T> setter) where T : Tracing, new()
        {
            setter(activityTracing);
            return activityTracing;
        }
    }
}
