// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Workplans
{
    /// <summary>
    /// Additional facade interface for components that store and provide workplans
    /// </summary>
    public interface IWorkplans
    {
        /// <summary>
        /// Saves workplan and return database id
        /// </summary>
        /// <param name="workplan">Workplan instance</param>
        /// <returns>Database id of workplan</returns>
        long SaveWorkplan(Workplan workplan);

        /// <summary>
        /// Loads the workplan
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        /// <returns>Instance of workplan</returns>
        Workplan LoadWorkplan(long workplanId);

        /// <summary>
        /// Load all workplans managed by the module
        /// </summary>
        IReadOnlyList<Workplan> LoadAllWorkplans();

        /// <summary>
        /// Deletes workplan if in new state
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        bool DeleteWorkplan(long workplanId);

        /// <summary>
        /// Load previous versions of the workplan
        /// </summary>
        IReadOnlyList<Workplan> LoadVersions(long workplanId);
    }
}
