// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Snapshot of a token
    /// </summary>
    public class HolderSnapshot
    {
        /// <summary>
        /// Id of this tokens holder
        /// </summary>
        public long HolderId { get; set; }

        /// <summary>
        /// The token
        /// </summary>
        public IToken[] Tokens { get; set; }

        /// <summary>
        /// Internal state object of the holder
        /// </summary>
        public object HolderState { get; set; }
    }

    /// <summary>
    /// Snapshot of the workplan instance used to restore it later
    /// </summary>
    public class WorkplanSnapshot
    {
        /// <summary>
        /// Name of the workplan type
        /// </summary>
        public string WorkplanName { get; set; }

        /// <summary>
        /// All tokens and their position
        /// </summary>
        public HolderSnapshot[] Holders { get; set; }
    }
}
