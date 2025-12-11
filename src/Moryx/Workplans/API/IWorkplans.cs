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
        /// <param name="cancellationToken"></param>
        /// <returns>Database id of workplan</returns>
        Task<long> SaveWorkplanAsync(Workplan workplan, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the workplan
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Instance of workplan</returns>
        Task<Workplan> LoadWorkplanAsync(long workplanId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load all workplans managed by the module
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IReadOnlyList<Workplan>> LoadAllWorkplansAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes workplan if in new state
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        /// <param name="cancellationToken"></param>
        Task<bool> DeleteWorkplanAsync(long workplanId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load previous versions of the workplan
        /// </summary>
        /// <param name="workplanId">Workplan id</param>
        /// <param name="cancellationToken"></param>
        Task<IReadOnlyList<Workplan>> LoadVersionsAsync(long workplanId, CancellationToken cancellationToken = default);
    }
}
