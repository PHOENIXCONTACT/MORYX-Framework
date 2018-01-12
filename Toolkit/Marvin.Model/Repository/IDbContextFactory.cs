namespace Marvin.Model
{
    internal interface IDbContextFactory
    {
        MarvinDbContext CreateContext(ContextMode contextMode);

        MarvinDbContext CreateContext(IDatabaseConfig config, ContextMode contextMode);
    }
}