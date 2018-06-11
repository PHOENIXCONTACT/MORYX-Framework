using System.Collections.Generic;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.AbstractionLayer.TestTools
{
    public class ReferenceCollection<T> : List<T>, IReferences<T> where T : IResource
    {

    }
}
