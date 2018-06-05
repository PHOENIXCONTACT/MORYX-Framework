namespace Marvin.Model
{
    internal interface IContextUnitOfWorkFactory
    {
        IUnitOfWork Create(MarvinDbContext context);
    }
}