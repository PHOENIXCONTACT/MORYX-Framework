// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Class representing the result of an activity
    /// </summary>
    public sealed class ActivityResult
    {
        /// <summary>
        /// Create the result instance for this activity
        /// </summary>
        public static ActivityResult Create<T>(T result)
            where T : struct
        {
            var numeric = (int)(object)result;
            return Create(numeric == 0, numeric);
        }

        /// <summary>
        /// Create the result instance for this activity
        /// </summary>
        public static ActivityResult Create(bool success, long value)
        {
            var activityResult = new ActivityResult
            {
                Numeric = value,
                Success = success,
            };
            return activityResult;
        }

        /// <summary>
        /// Indicator if this result is considered successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Result number used to determine next steps
        /// </summary>
        public long Numeric { get; set; }
    }
}
