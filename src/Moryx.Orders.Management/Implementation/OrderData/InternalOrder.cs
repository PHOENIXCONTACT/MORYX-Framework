// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;

namespace Moryx.Orders.Management
{
    internal class InternalOrder : Order
    {
        private readonly List<InternalOperation> _operations;

        public InternalOrder()
        {
            _operations = new List<InternalOperation>();
            base.Operations = _operations;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string Number
        {
            get => base.Number;
            set => base.Number = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string Type
        {
            get => base.Type;
            set => base.Type = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IList<InternalOperation> Operations
        {
            get => _operations;
            set
            {
                _operations.Clear();
                _operations.AddRange(value);
            }
        }
    }
}
