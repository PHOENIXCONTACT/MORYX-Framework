namespace Marvin.Model
{
    /// <summary>
    /// Factory capable of creating any 
    /// </summary>
    public interface IModelResolver
    {
        /// <summary>
        /// Create an open context using the model name
        /// </summary>
        IUnitOfWorkFactory GetByName(string modelName);
    }
}
