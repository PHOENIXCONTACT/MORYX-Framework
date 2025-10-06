// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Identity;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Condition for the process to match the session
    /// </summary>
    public readonly struct ProcessReference : IEquatable<ProcessReference>
    {
        private const long EmptyId = 0;

        private const long IrrelevantId = -1;

        private readonly long _processId;

        private readonly IIdentity _identity;

        private ProcessReference(long processId, IIdentity identity)
        {
            _processId = processId;
            _identity = identity;
        }

        /// <summary>
        /// Check if the condition specifies an empty session
        /// </summary>
        public bool IsEmpty => _processId == EmptyId && _identity == null;

        /// <summary>
        /// Check if the process reference 
        /// </summary>
        public bool HasReference => _processId != EmptyId | _identity != null;

        /// <summary>
        /// Check if a process 
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public bool Matches(IProcess process)
        {
            if (_processId != IrrelevantId)
                return _processId == process.Id;

            var prodProcess = process as ProductionProcess;
            var identity = (prodProcess?.ProductInstance as IIdentifiableObject)?.Identity;
            if (identity == null || _identity == null)
                return false;

            return identity.Equals(_identity);
        }

        /// <summary>
        /// Empty request without a process reference
        /// </summary>
        public static ProcessReference Empty => new ProcessReference(EmptyId, null);

        /// <summary>
        /// Reference a certain process by id
        /// </summary>
        public static ProcessReference ProcessId(long id) => new ProcessReference(id, null);

        /// <summary>
        /// Reference process by product instance identity
        /// </summary>
        public static ProcessReference InstanceIdentity(IIdentity identity) => new ProcessReference(IrrelevantId, identity);

        /// <inheritdoc />
        public override string ToString()
        {
            if (IsEmpty)
                return "Empty reference";

            return _processId == IrrelevantId ? $"Identity: {_identity}" : $"ProcessId: {_processId}";
        }

        /// <summary>
        /// Compare this reference to another reference
        /// </summary>
        public bool Equals(ProcessReference other)
        {
            return _processId == other._processId && Equals(_identity, other._identity);
        }

        /// <summary>
        /// Compare reference to unknown object
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ProcessReference && Equals((ProcessReference)obj);
        }

        /// <summary>
        /// Define equal operator
        /// </summary>
        public static bool operator ==(ProcessReference a, ProcessReference b) => a.Equals(b);

        /// <summary>
        /// Define equal operator
        /// </summary>
        public static bool operator !=(ProcessReference a, ProcessReference b) => !a.Equals(b);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (_processId.GetHashCode() * 397) ^ (_identity != null ? _identity.GetHashCode() : 0);
            }
        }
    }
}
