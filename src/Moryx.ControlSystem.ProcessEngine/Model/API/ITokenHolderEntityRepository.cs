// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Model.Repositories;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    /// <summary>
    /// The public API of the TokenHolderEntity repository.
    /// </summary>
    public interface ITokenHolderEntityRepository : IRepository<TokenHolderEntity>
    {
        /// <summary>
        /// Get all TokenHolderEntitys where ProcessId equals given value
        /// </summary>
        /// <param name="processId">Value the entities have to match</param>
        /// <returns>Collection of all matching TokenHolderEntitys</returns>
        ICollection<TokenHolderEntity> GetAllByProcessId(long processId);

        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        TokenHolderEntity Create(long holderId, string tokens);
    }
}

