// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
        Task<long> SaveWorkplanAsync(Workplan workplan);

        /// <summary>
        /// Loads the workplan
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        /// <returns>Instance of workplan</returns>
        Task<Workplan> LoadWorkplanAsync(long workplanId);

        /// <summary>
        /// Load all workplans managed by the module
        /// </summary>
        Task<IReadOnlyList<Workplan>> LoadAllWorkplansAsync();

        /// <summary>
        /// Deletes workplan if in new state
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        Task<bool> DeleteWorkplanAsync(long workplanId);

        /// <summary>
        /// Load previous versions of the workplan
        /// </summary>
        Task<IReadOnlyList<Workplan>> LoadVersionsAsync(long workplanId);
    }
}
