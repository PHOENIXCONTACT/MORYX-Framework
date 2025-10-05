// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Different positions a job can have in the JobList after its insertion
    /// </summary>
    public enum JobPositionType
    {
        /// <summary>
        /// Append the job at the end of the list. This is the default behavior
        /// </summary>
        Append = 0,

        /// <summary>
        /// Add the job at the start of the list
        /// </summary>
        Start = 1,

        /// <summary>
        /// Move job before another one
        /// </summary>
        BeforeOther = 2,

        /// <summary>
        /// PositionType job after another one
        /// </summary>
        AfterOther = 3,

        /// <summary>
        /// The new jobs surround existing ones
        /// </summary>
        AroundExisting = 4,

        /// <summary>
        /// Append job(s) to group of jobs with the same recipe
        /// </summary>
        AppendToRecipe = 5,
    }
}
