using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Interface to easily access 
    /// </summary>
    public interface ILinkStrategy
    {
        /// <summary>
        /// Name of the parts property
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Type of parts connected by this link
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Flag if saving a part should include saving all parts recursively
        /// </summary>
        bool RecursivePartSaving { get; }

        /// <summary>
        /// Strategy how article parts are created during loading
        /// </summary>
        PartSourceStrategy PartCreation { get; }

        /// <summary>
        /// Load typed object and set on product
        /// </summary>
        IProductPartLink Load(IUnitOfWork uow, PartLink linkEntity);

        /// <summary>
        /// Save link to entity
        /// </summary>
        PartLink Create(IUnitOfWork uow, IProductPartLink link);

        /// <summary>
        /// Update existing link entity
        /// </summary>
        void Update(IUnitOfWork uow, IProductPartLink link, PartLink linkEntity);

        /// <summary>
        /// Old link entity
        /// </summary>
        void Delete(IUnitOfWork uow, PartLink[] entities);
    }
}