namespace Marvin.Model
{
    /// <summary>
    /// <see cref="EntityFrameworkRepository{T}"/> with an additional parent unit of work
    /// </summary>
    public class InheritedEntityFrameworkRepository<T> : EntityFrameworkRepository<T>
        where T : class, IEntity
    {
        /// <summary>
        /// Access to the parent model
        /// </summary>
        protected IUnitOfWork ParentUow { get; set; }
    }
}
