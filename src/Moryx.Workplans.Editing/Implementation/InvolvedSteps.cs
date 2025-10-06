// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Editing.Implementation
{
    internal struct InvolvedSteps
    {
        /// <summary>
         /// Create instance with all involved entities
         /// </summary>
        public InvolvedSteps(IWorkplanStep source, IWorkplanStep target, IConnector connector)
            : this()
        {
            Source = source;
            Target = target;
            Connector = connector;
        }

        /// <summary>
        /// Source of the connection, e.g. the step that references the connector as output
        /// </summary>
        public IWorkplanStep Source { get; private set; }

        /// <summary>
        /// Target of the connection, e.g the step that references the connector as input
        /// </summary>
        public IWorkplanStep Target { get; private set; }

        /// <summary>
        /// Connector of the connection
        /// </summary>
        public IConnector Connector { get; private set; }
    }
}

