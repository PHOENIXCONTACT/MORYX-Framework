// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Identity
{
    /// <summary>
    /// Context to provide access for a principal
    /// </summary>
    public interface IAuthorizationContext
    {
        /// <summary>
        /// Checks access for the given resource and action on a principal
        /// </summary>
        bool CheckAccess(string resource, string action);
    }
}