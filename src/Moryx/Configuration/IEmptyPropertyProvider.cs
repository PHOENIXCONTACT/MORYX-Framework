// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Configuration
{
    /// <summary>
    /// Empty property provider to pre-fill newly created objects
    /// </summary>
    public interface IEmptyPropertyProvider
    {
        /// <summary>
        /// Fills the object
        /// </summary>
        void FillEmpty(object obj);
    }
}
