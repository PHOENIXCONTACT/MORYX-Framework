---
uid: GettingStarted.CodeFirst
---
# Code first

This article describes the usage of the Code First approach with [Entity Framework](https://docs.microsoft.com/en-us/ef/) and [Npgsql](http://www.npgsql.org) (or any other data provider) in the world of the MORYX Core. MORYX fully supports the Code First approach. The next lines of code will show you exemplary how the Code First approach can be implemented.

## Basics

You have to implement a few basics to get started working with the PostgreSQL database. The things to implement are:

| Component | Description |
|-----------|-------------|
| [DbContext](https://msdn.microsoft.com/en-us/library/jj729737.aspx) | The primary class that is responsible for interacting with data as objects (often referred to as context). The context class manages the entity objects during run time, which includes populating objects with data from a database, change tracking, and persisting data to the database. |
| Entities | One entity is one table |
| Repositories | Helper functions to access a specific table. The MORYX framework implements a speciality to make your life easier. You just need to define the interface and the code will be generated at runtime. |

More extended, additional repositories can be created for the [UnitOfWork Repository Pattern](UnitOfWorkPattern.md).

## The database DbContext

Create your own database context by deriving from `DbContext` or for more comfortability from [MoryxDbContext](/src/Moryx.Model/MoryxDbContext.cs) and add the two constructor overloads as shown below.

````cs
public class SolarSystemContext : MoryxDbContext
{
    public SolarSystemContext()
    {
    }

    public SolarSystemContext(DbContextOptions options) : base(options)
    {
        // Necessary for DesignTimeDbContextFactory
    }

    public virtual DbSet<Planet> Planets { get; set; }

    public virtual DbSet<Satellite> Satellites { get; set; }

    public virtual DbSet<Asteroid> Asteroids { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Additional FluentAPI calls ...
    }
}
````

If you want to provide database provider specific implementations you can derive from this context. The database specific attribute provides an argument for the base context. This is important to use the base context in a generic way, without knowledge about the specific database providers. For example for PostgreSQL:

````cs
[NpgsqlDbContext(typeof(SolarSystemContext))]
public class NpgsqlSolarSystemContext : SolarSystemContext
{
    public NpgsqlSolarSystemContext()
    {
    }

    public NpgsqlSolarSystemContext(DbContextOptions options) : base(options)
    {
        // Necessary for DesignTimeDbContextFactory
    }
}
````

or for Sqlite:

````cs
[SqliteDbContext(typeof(SolarSystemContext))]
public class SqliteSolarSystemContext : SolarSystemContext
{
    public SqliteSolarSystemContext()
    {
    }

    public SqliteSolarSystemContext(DbContextOptions options) : base(options)
    {
        // Necessary for DesignTimeDbContextFactory
    }
}
````

If you just want to support one database provider, you can use the database specific attribute on the base context directly.

````cs
[NpgsqlDbContext]
public class SolarSystemContext : MoryxDbContext
{
    ...
}
````

## Entities

Then you need to define your entities which will be represented as tables in your database.

````cs
public class Planet : ModificationTrackedEntityBase
{
    public virtual string Name { get; set; }

    public virtual double EquatorialRadius { get; set; }

    public virtual bool IsLifePossible { get; set; }

    public virtual bool VisitedByEnterprise { get; set; }

    public virtual ICollection<Satellite> Satellites { get; set; }
}

public class Satellite : EntityBase
{
    public virtual string Name { get; set; }

    public virtual double Distance { get; set; }

    public virtual bool IsSpying { get; set; }

    public virtual bool IsWeaponOfMassDestruction { get; set; }
}

public class Asteroid : ModificationTrackedEntityBase
{
    public virtual string Name { get; set; }

    public virtual double Speed { get; set; }

    public virtual int Mass { get; set; }

    public virtual bool WasFiredByBugs { get; set; }
}
````

The entities are either derived from [EntityBase](/src/Moryx.Model/EntityBase.cs) or [ModificationTrackedEntityBase](/src/Moryx.Model/ModificationTrackedEntityBase.cs).
Is that necessary?
That's pretty much better because the [EntityBase](/src/Moryx.Model/EntityBase.cs) defines an extra property for the `Id`.
This `Id` is treated specially as self incrementing primary key. [ModificationTrackedEntityBase](/src/Moryx.Model/ModificationTrackedEntityBase.cs) derives from the base.
It has three properties which are monitored by triggers (Created, Updated, Deleted).
These triggers are automatically applied to the entity.
You can find further information regarding modification tracking [here](/docs/articles/Core/DataModel/ModificationTracking.md).

## Proxies

EF Core can dynamically generate proxy types that implement `INotifyPropertyChanging` and `INotifyPropertyChanged`. This requires installing the `Microsoft.EntityFrameworkCore.Proxies` NuGet package, and enabling change-tracking proxies with UseChangeTrackingProxies For example:

````cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseChangeTrackingProxies();
````

## Entity loading & change tracking behavior

The Entity Framework supports a set of features how the context behaves when you are accessing or changing entities. Please have a look on [Entity Framework Loading Related Data](https://docs.microsoft.com/en-us/ef/core/querying/related-data/) if you want to dive deeper.

These two features are called lazy loading and change tracking.

### Change tracking

Entity Framework needs change tracking to create the changing SQL statement, i.e. it has to know which column did change and which not.
You can decide between three options of tracking:

- **Snapshot Change Tracking**: EF keeps the original data "in mind" and compares them to the new version.
- **Dynamic Change Tracking**: Can only be used if proxies are enabled and the entity properties are `virtual`. The proxy sends a changed message so the framework does not need to compare the whole data set.
- **Read only**: No change tracking is enabled. The access to the database is readonly.

### Entity loading

If an entity consists of one or more navigation properties you might want to have access to them.
There are three ways to access these kind of properties:

| Mode               | Explanation                                                                                                                                                                                                               |
|--------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Eager loading`    | With eager loading it is possible to configure which relations shall be loaded every time (Have a look on the [Include extension](https://msdn.microsoft.com/en-us/library/system.data.entity.dbextensions.include.aspx)) |
| `Lazy loading`     | Lazy loading loads relations when you access them                                                                                                                                                                         |
| `Explicit loading` | You can load a relation explicitly when lazy loading was turned off                                                                                                                                                       |

### Enable automatic lazy loading

If you want to profit from automatic lazy loading, ef proxies are required. EntityFramework supports a per navigation property switch for lazy loading support.
You need __explicitly__ define the navigation property as __`virtual`__ if you want lazy loading support to be enabled on this.

### MORYX specific

To use the context within a MORYX module, you must declare a dependency on `IDbContextManager` and register it together with the context specific factory in your local container. Afterwards you can use injection for context specific factories anywhere in the module.

````cs
// ModuleController.cs

public IDbContextManager DbContextManager { get; set; }

protected override Task OnInitializeAsync(CancellationToken cancellationToken)
{
    ...
    Container.ActivateDbContexts(DbContextManager);
    ...
}

// Somewhere within the modules composition
public class MyComponent : IMyComponent
{
    // Injected
    public IContextFactory<SolarSystemContext> SolarContextFactory { get; set; }
}
````

## UnitOfWork Repository Pattern

MORYX brings out of the box extensions on the `DbContext` to provide the [UnitOfWork Repository Pattern](/docs/articles/framework/DataModel/UnitOfWorkPattern.md).
For the following paragraphs, it is necessary to have a rough understanding of it.

### Repositories

First let's define a repository API (same assembly as the context is defined in)

````cs
public interface IPlanetRepository : IRepository<Planet>
{
    Planet Create(string name);

    IEnumerable<Planet> GetAllBy(string name);
    List<Planet> GetAllBy(bool isLifePossible);
    ICollection<Planet> GetAllBy(string name, bool isLifePossible);

    ICollection<Planet> GetAllContains(string name);
    ICollection<Planet> GetAllContains(string name, int isLifePossible);

    Planet GetSingleBy(string name);
    Planet GetSingleContains(string name);
    Planet GetSingleOrDefaultBy(string name);
    Planet GetSingleOrDefaultContains(string name);

    Planet GetFirstBy(string name);
    Planet GetFirstContains(string name);
    Planet GetFirstOrDefaultBy(string name);
    Planet GetFirstOrDefaultContains(string name);

    Planet GetBy(string name);
    Planet GetContains(string name);
    Planet Get(string name);

    ICollection<Planet> GetAllByName(string name);
    ICollection<Planet> GetAllContainsName(string name);
    ICollection<Planet> GetAllByNameAndIsLifePossible(string name, int isLifePossible);
}

public interface ISatelliteRepository : IRepository<Satellite>
{
}

public interface IAsteroidRepository : IRepository<Asteroid>
{
}
````

If you got scared that you have to implement all these functions you are lucky, they will be implemented automatically.
So you only have to define the interfaces.
If you want to know more about the automatic repository instantiation please have a look into the [Repository Proxy Builder](/docs/articles/Core/DataModel/RepositoryProxyBuilder.md).
The example functions defined above are also not necessary.
Only add functions that you really need.

But if you need a more specialized implementation of a repository you can either use a mixture of repository proxies and self implemented repository or your own repository implementation.

To use the unit of work pattern, another factory is registered to the local container of the module `IUnitOfWorkFactory` and this can also be injected per context:

````cs
// Somewhere within the modules composition
public class MyComponent : IMyComponent
{
    // Injected
    public IUnitOfWorkFactory<SolarSystemContext> UnitOfWorkFactory { get; set; }
}
````

The repository can be used by creating a unit of work with the factory and resolving the repository

````cs
using var uow = UnitOfWokFactory.Create();
var planetRepo = uow.GetRepository<IPlanetRepository>();
var planetEntity = planetRepo.Create();

await uow.SaveChangesAsync();

````

You do not need to implement your repository interface, this is completely done by the [Repository Proxy Builder](/docs/articles/framework/DataModel/RepositoryProxyBuilder.md).

## Database Migration

Half of way is done for now.
We are prepared to do the first step of migration. Database migration is a hard business if you have many changes made to your data model.
The EntityFramework comes with a tool to make the migration hassle a little bit easier.

Database migration consists of a migration configuration that is needed for every `DbContext` and a set of migration steps. Both things are generated by migration scripts.

The entity framework comes with two cli commands to do database model migration:

| Script                      | Explanation                                                                                                            |
|-----------------------------|------------------------------------------------------------------------------------------------------------------------|
| `dotnet ef migrations add`  | This command is executed every time you want to create a new migration                                                 |
| `dotnet ef database update` | This command runs all database migrations found in the corresponding assembly. This step can also be called from code. |

When EF guys are talking about the *CodeFirst Migrations* approach they mean exactly these commands.

### Creation of migration configuration

Before we implement the UnitOfWork you can configure your migration. The ef migration every time needs an executable for the startup project.
It is necessary to have one if you project is just a class library.
We use the default StartProject of an application to create the migration. Keep in mind that the project must be referenced in the start project. The migration tool needs to know the data model connection string for the migration. MORYX cannot configure the migration process therefore we provide two options by default:

- Pass the connection string as parameter to the migration command
- Use `EFCORETOOLSDB` environment variable

For both options we need implementations of the `IDesignTimeDbContextFactory<TContext>` interface for every `DbContext` you want to migrate ([Microsoft Docs](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-a-design-time-factory)). This factory is used by the migration tool to create an instance of your context during design time. MORYX provides a base implementation for you to make your life easier and avoid code duplication. Each database specific implementation provides a base class for you to derive from. For PostgreSQL it is the `NpgsqlDesignTimeDbContextFactory<TContext>` located in the `Moryx.Model.PostgreSQL` package. For Sqlite it is the `SqliteDesignTimeDbContextFactory<TContext>` located in the `Moryx.Model.Sqlite` package.

Samples for the SolarSystemContext could look as follows. There is no additional code necessary within the classes.

````cs
// Npgsql
public class NpgsqlSolarSystemContextDesignTimeFactory : NpgsqlDesignTimeDbContextFactory<NpgsqlSolarSystemContext>
{
}

// Sqlite
public class SqliteSolarSystemContextDesignTimeFactory : NpgsqlDesignTimeDbContextFactory<SqliteSolarSystemContext>
{
}
````

The `Moryx.Model.DesignTimeFactory<TContext>` base classes already implement the logic to read the connection string from the command line parameters or the `EFCORETOOLSDB` environment variable.

````ps
// Using command line parameter
dotnet ef migrations add InitialCreate -s .path/to/StartProject.csproj -c SolarSystemContext -- --connection "YourConnectionString"

// Using environment variable
$env:EFCORETOOLSDB="YourConnectionString"
dotnet ef migrations add InitialCreate -s .path/to/StartProject.csproj -c SolarSystemContext
````

### Update-Database

The last step to do, is to apply the migrations to the database so that your context and its entities are on the same version. Copy the following line to the command line:

````ps
// Using command line parameter
dotnet ef database update InitialCreate -s path/to/StartProject.csproj -c SolarSystemContext -- --connection "YourConnectionString"

// Using environment variable
$env:EFCORETOOLSDB="YourConnectionString"
dotnet ef database update InitialCreate -s path/to/StartProject.csproj -c SolarSystemContext
````

The database should now have been updated. Have a look.

### Repeat when necessary

When you have changed your entities or your context while developing you only need to call the `dotnet ef migrations add` once more.
You have to consider two things: First ensure that your database is on the latest migration.
Second change the `Name` parameter of the script to a speaking version name.

## Accessing your data

But how to access the data? The next snippet shows an example call of the database. Note that usually a `DbContextFactory` is injected.

````cs
var context = DbContextFactory.Create<SolarSystemContext>();
var planets = context.Planets;
````

## Schema support

We added schema support to the migration routines so that you don't need to take care of it by yourself.
It is possible to set the target schema of a table in two ways:

- On the table itself. You can do this with the `TableAttribute`: `[Table(nameof(Planet), Schema = SolarSystemModelConstants.Schema)]`. This attributes applies to the table where the attribute was placed.
- On the `DbContext`. You can do this with the `DefaultSchemaAttribute`: `[DefaultSchema(SolarSystemModelConstants.Schema)]`. This attribute applies to all tables within the context where the attribute was placed.

## Inheritance

The Entity Framework supports three inheritance strategies (an overview can be found [here](http://www.entityframeworktutorial.net/code-first/inheritance-strategy-in-code-first.aspx)):

| Strategy | Description |
|----------|-------------|
| Table per Hierarchy (TPH) | This approach suggests one table for the entire class inheritance hierarchy. Each table includes a discriminator column which distinguishes between inheritance classes. This is a default inheritance mapping strategy in Entity Framework. |
| Table per Type (TPT) | This approach suggests a separate table for each domain class. |
| Table per Concrete class (TPC) | This approach suggests one table for one concrete class, but not for the abstract class. So, if you inherit the abstract class in multiple concrete classes, then the properties of the abstract class will be part of each table of the concrete class. |

The default strategy the EF is using is the `Table per Hierarchy`.

## Frequently asked questions

- Is it possible to add several migrations to the same database or schema?
  Yes, that's possible. But you have to be careful and avoid name collisions.
- Is it possible to downgrade to an older migration?
  Yes, but you have to take care of your context implementation that will differ from the older migration.
- Is it possible to change the schema later?
  Yes. Just change the schema.
- I need to do some additional data migration tasks. Where should I implement them?
  Every generated migration can be changed later. There you can call custom SQL statements.
