using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Default part accessor without model merge
    /// </summary>
    public class DefaultLinkStrategy<TProduct> : ILinkStrategy
        where TProduct : class, IProduct
    {
        /// <summary>
        /// Create instance for type accessor
        /// </summary>
        public DefaultLinkStrategy(string name)
        {
            Name = name;
            TypeName = typeof(TProduct).Name;
        }

        /// <summary>
        /// Name of the parts property
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Type of parts connected by this link
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Strategy to determine how article instances are loaded as parts
        /// </summary>
        public virtual PartSourceStrategy PartCreation => PartSourceStrategy.FromPartlink;

        /// <summary>
        /// Load typed object and set on product
        /// </summary>
        public virtual IProductPartLink Load(IUnitOfWork uow, PartLink linkEntity)
        {
            return new ProductPartLink<TProduct>(linkEntity.Id);
        }

        /// <summary>
        /// Save link to entity
        /// </summary>
        public virtual PartLink Create(IUnitOfWork uow, IProductPartLink link)
        {
            var repo = uow.GetRepository<IPartLinkRepository>();
            return repo.Create(Name);
        }

        /// <summary>
        /// Update existing link entity
        /// </summary>
        public virtual void Update(IUnitOfWork uow, IProductPartLink link, PartLink linkEntity)
        {
            // Nothing to do on plain links
        }

        /// <summary>
        /// Old link entity
        /// </summary>
        public virtual void Delete(IUnitOfWork uow, PartLink[] entities)
        {
            var repo = uow.GetRepository<IPartLinkRepository>();
            repo.RemoveRange(entities);
        }
    }
}