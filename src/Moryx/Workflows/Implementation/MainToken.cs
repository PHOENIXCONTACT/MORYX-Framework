// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Main execution token passed trough the workplan instance
    /// </summary>
    internal class MainToken : IToken
    {
        /// <summary>
        /// Token name
        /// </summary>
        public string Name => nameof(MainToken);
    }
}
