using System;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Enum representing the relation between two entities and definition of their life cycle coupling.
    /// The lowest 3 bits define the UML association type. The other 29 bits are for free use
    /// Bit:    31 - 3 |    2    |      1      |     0
    /// Flag:    Type  |  Usage  | Composition | Aggregation
    /// Usage: Pick a number and bitshift it by 3 and add the UML type, e.g. 42 => (42 &lt;&lt; 3) + 1
    /// </summary>
    [Flags]
    public enum ResourceRelationType
    {
        /// <summary>
        /// Relation type was not set
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Relation is a UML aggregations
        /// !DO NOT USE ALONE!
        /// </summary>
        Aggregation = 1,

        /// <summary>
        /// Relation is UML composition
        /// !DO NOT USE ALONE!
        /// </summary>
        Composition = 2,

        /// <summary>
        /// Relations is UML usage
        /// !DO NOT USE ALONE!
        /// </summary>
        Usage = 4,

        /// <summary>
        /// Standard parent child relation
        /// </summary>
        ParentChild = (1 << 3) + 2,  // Parent childs relationships are compositions

        /// <summary>
        /// Referenced resource is a driver
        /// </summary>
        Driver = (3 << 3) + 1,

        /// <summary>
        /// Targets of this relation are in a transport relationship like cell and routing
        /// </summary>
        TransportSystem = (8 << 3) + 1,  // Transports are the next bit and an aggregation

        /// <summary>
        /// Defines a transport route from one resource to another
        /// </summary>
        TransportRoute =  (9 << 3)  + 4,  // Transport relation of type usage relation

        /// <summary>
        /// Defines the current mounted exchangeable part like a probe head.
        /// </summary>
        CurrentExchangablePart = (16 << 3) + 4, // Exchangable parts are usages

        /// <summary>
        /// Defines the possible exchangeable parts like probe heads.
        /// </summary>
        PossibleExchangablePart = (17 << 3) + 4, // Exchangeable parts are usages

        /// <summary>
        /// Indicates that the source decorates the target
        /// </summary>
        Decorated = (32 << 3) + 2,

        /// <summary>
        /// Indicates that the source is an extension of the target.
        /// </summary>
        Extension = (33 << 3) + 1, // Extension relation of type aggregation relation.

        /// <summary>
        /// Undefined custom relation that uses the RelationName field
        /// </summary>
        Custom = (int.MaxValue & ~3) // FF FF FF FC
    }
}