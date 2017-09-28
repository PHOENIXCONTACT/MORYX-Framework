using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.Model
{
    /// <summary>
    /// Interface for all entities that are merged with a parent entity
    /// </summary>
    public interface IMergedEntity<TParent> : IEntity
        where TParent : class, IEntity
    {
        /// <summary>
        /// Parent reference of this entity
        /// </summary>
        TParent Parent { get; }
    }
}
