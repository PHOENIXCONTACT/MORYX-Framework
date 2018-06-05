namespace Marvin
{
    /// <summary>
    /// Interface for all type structures that use a lot if inheritance. The idea is to provide an instance type property that can be compared to a
    /// constant string field on an concrete type. This allows for switch statements to determine the correct type
    /// </summary>
    /// <example>
    /// IQuickCast someInstance = MethodFarAway();
    /// switch(someInstance.Type)
    /// {
    ///     case SomeType.TypeName: 
    ///         var instance = (SomeType)someInstance;
    ///         break;
    /// }
    /// </example>
    public interface IQuickCast
    {
         /// <summary>
         /// Type name of this implementation that is unique within its group. A group refers to all implementations of an
         /// interface that derives from IQuickCast.
         /// </summary>
         string Type { get; }
    }
}