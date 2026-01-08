// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Restrictions
{
    /// <summary>
    /// Description of the result of a performed rule
    /// </summary>
    public class RestrictionDescription
    {
        /// <summary>
        /// Constructor to create a <see cref="RestrictionDescription"/> instance with the needed information
        /// </summary>
        public RestrictionDescription(string text, RestrictionSeverity severity)
        {
            Text = text;
            Severity = severity;
        }

        /// <summary>
        /// Description of the result
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Severity of the result
        /// </summary>
        public RestrictionSeverity Severity { get; }
    }
}