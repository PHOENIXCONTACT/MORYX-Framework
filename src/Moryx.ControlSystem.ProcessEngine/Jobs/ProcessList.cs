// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    internal class ProcessList<T> : List<T>, IReadOnlyList<T>
    where T : class
    {
        public ProcessList() : base()
        {
        }

        public ProcessList(int capacity) : base(capacity)
        {
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return new ProcessDataIterator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

