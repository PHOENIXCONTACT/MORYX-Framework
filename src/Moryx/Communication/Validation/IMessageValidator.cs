// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Communication
{
    /// <summary>
    /// Class to validate and assign binary header instances
    /// </summary>
    public interface IMessageValidator
    {
        /// <summary>
        /// Validate the message
        /// </summary>
        bool Validate(BinaryMessage message);

        /// <summary>
        /// Interpreter used by the protocol of this validator
        /// </summary>
        IMessageInterpreter Interpreter { get; }
    }

    /// <summary>
    ///  Extended interface to validate initial assignment 
    /// </summary>
    public interface IAdvancedMessageValidator : IMessageValidator
    {
        /// <summary>
        /// Validation method with additional information
        /// </summary>
        bool Validate(BinaryMessage message, bool initialMessage);
    }
}
