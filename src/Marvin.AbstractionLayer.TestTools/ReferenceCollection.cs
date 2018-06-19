using System.Collections.Generic;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of a resource reference collection
    /// </summary>
    public class ReferenceCollection<T> : List<T>, IReferences<T> where T : IResource
    {

    }
}
