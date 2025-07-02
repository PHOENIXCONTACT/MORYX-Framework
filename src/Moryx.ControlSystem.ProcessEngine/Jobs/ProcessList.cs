using Moryx.ControlSystem.ProcessEngine.Processes;
using System.Collections;
using System.Collections.Generic;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    internal class ProcessList<T> : List<T>, IReadOnlyList<T>
    where T: class
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
