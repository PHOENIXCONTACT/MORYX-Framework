using Marvin.Collections;

namespace Marvin.Tests.Collections
{
    internal class SortObj : ISortableObject
    {
        public SortObj(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public int SortOrder { get; set; }

        public override string ToString()
        {
            return string.Format("Order: {0} - Name: {1}", SortOrder, Name);
        }
    }
}