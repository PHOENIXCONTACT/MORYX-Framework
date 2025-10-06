// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Users;

namespace Moryx.Orders
{
    /// <summary>
    /// Event args of a started operation
    /// </summary>
    public class OperationStartedEventArgs : OperationChangedEventArgs
    {
        /// <summary>
        /// The user which started the operation
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OperationStartedEventArgs"/>
        /// </summary>
        public OperationStartedEventArgs(Operation operation, User user) : base(operation)
        {
            User = user;
        }
    }
}