---
uid: GettingStarted.CodeFirst
---
# Code first

This article describes the usage of the Code First approach with [Entity Framework](https://docs.microsoft.com/en-us/ef/) and [Npgsql](http://www.npgsql.org) in the world of the MORYX Core. MORYX fully supports the Code First approach. The next lines of code will show you exemplary how the Code First approach can be implemented.

## Basics

You have to implement a few basics to get started working with the PostgreSQL database. The things to implement are:

| Component | Description |
|-----------|-------------|
| [DbContext](https://msdn.microsoft.com/en-us/library/jj729737.aspx) | The primary class that is responsible for interacting with data as objects (often referred to as context). The context class manages the entity objects during run time, which includes populating objects with data from a database, change tracking, and persisting data to the database. |
| Entities | One entity is one table |
| Repositories | Helper functions to access a specific table. The MORYX framework implements a speciality to make your life easier. You just need to define the interface and the code will be generated at runtime. |

More extended, additional repositories can be created for the [UnitOfWork Repository Pattern](../../articles/Core/DataModel/UnitOfWorkPattern.md).

## The database DbContext

Create your own database context by deriving from `DbContext` or for more comfortability from [MoryxDbContext](xref:Moryx.Model.MoryxDbContext) and add the three constructor overloads as shown below.

````cs
[ModelConfigurator(typeof(NpgsqlModelConfigurator))]
[DbConfigurationType(typeof(NpgsqlConfiguration))]
public class SolarSystemContext : MoryxDbContext
{
    public SolarSystemContext()
    {
        // Important: Migration functions needing a parameterless constructor
    }

    public SolarSystemContext(string connectionString)
        : base(connectionString)
    {
        // Used by MORYX internals
    }

    public SolarSystemContext(DbConnection connection)
        : base(connection)
    {
        // Optional: If Moryx.InMemory is used for testing
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

Please note the [ModelConfiguratorAttribute](xref:Moryx.Model.ModelConfiguratorAttribute) which is used by MORYX to create a configuration for the context. Here the `NpgsqlModelConfigurator` is using `Npgsql` for PostgreSQL databases.

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

The entities are either derived from [EntityBase](xref:Moryx.Model.EntityBase) or [ModificationTrackedEntityBase](xref:Moryx.Model.ModificationTrackedEntityBase).
Is that necessary?
That's pretty much better because the [EntityBase](xref:Moryx.Model.EntityBase) defines an extra property for the `Id`.
This `Id` is treated specially as self incrementing primary key. [ModificationTrackedEntityBase](xref:Moryx.Model.ModificationTrackedEntityBase) derives from the base.
It has three properties which are monitored by triggers (Created, Updated, Deleted).
These triggers are automatically applied to the entity.
You can find further information regarding Modification Tracking [here](../../articles/Core/DataModel/ModificationTracking.md).

## Entity loading & change tracking behavior

The Entity Framework supports a set of [configurable features](https://msdn.microsoft.com/en-us/library/system.data.entity.infrastructure.dbcontextconfiguration.aspx) how the context behaves when you are accessing or changing entites. Please have a look on [Entity Framework Loading Related Entities](https://msdn.microsoft.com/en-us/library/jj574232(v=vs.113).aspx) if you want to dive deeper.

These two features are called lazy loading and change tracking.

### Change tracking

Entity Framework needs change tracking to create the changing SQL statement, i.e. it has to know which column did change and which not.
You can decide between three options of tracking:

- **Snapshot Change Tracking**: EF keeps the original data "in mind" and compares them to the new version.
- **Dynamic Change Tracking**: Can only be used if proxies are enabled and the entity properties are `virtual`. The proxy sends a changed message so the framework does not need to compare the whole dataset.
- **Read only**: No change tracking is enabled. The access to the database is readonly.

### Entity loading

If an entity consists of one or more navigation properties you might want to have access to them.
There are three ways to access these kind of properties:

| Mode | Explaination |
|---------------------|---|
| `Eager loading` | With eager loading it is possible to configure which relations shall be loaded every time (Have a look on the [Include extension](https://msdn.microsoft.com/en-us/library/system.data.entity.dbextensions.include.aspx)) |
| `Lazy loading`     | Lazy loading loads relations when you access them |
| `Explicit loading`   | You can load a relation explictly when lazy loading was turned off |

### Enable lazy loading

If you want to profit from lazy loading, setting the right `ContextMode` is only one of the two steps required.
EntityFramework supports a per navigation property switch for lazy loading support.
You need __explicitly__ define the navigation property as __`virtual`__ if you want lazy loading support to be enabled on this.

### MORYX specific

To use the context within a MORYX module, you must declare a dependency on `IDbContextManager` and register it together with the context specific factory in your local container.
Afterwards you can use injection for context specific factories anywhere in the module.

````cs
// ModuleController.cs

public IDbContextManager DbContextManager { get; set; }

protected override void OnInitialize()
{
    Container.ActivateDbContexts(DbContextManager);

    // ..


// Somewhere within the modules composition
public class MyComponent : IMyComponent
{
    // Injected
    public IContextFactory<SolarSystemContext> SolarContextFactory { get; set; }
}
````

The MORYX framework uses `Dynamic Change Tracking` and lazy loading by default,  but it is possbile to override these settings.
You can change settings via the [ContextMode](xref:Moryx.Model.ContextMode):

````cs
// Example how you can change the default setting via ContextMode on the context
// This call enables `Dynamic Change Tracking` only feature
using (var context = SolarContextFactory.Create(ContextMode.Tracking))
{
    // or later with
    context.SetContextMode(ContextMode.Tracking);
}
````

## UnitOfWork Repository Pattern

MORYX brings out of the box extensions on the `DbContext` to provide the [UnitOfWork Repository Pattern](../../articles/Core/DataModel/UnitOfWorkPattern.md).
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
If you want to know more about the automatic repository instantiation please have a look into the [Repository Proxy Builder](../../articles/Core/DataModel/RepositoryProxyBuilder.md).
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
using (var uow = UnitOfWokFactory.Create())
{
    var planetRepo = uow.GetRepository<IPlanetRepository>();
    var planetEntity = planetRepo.Create();

    //[...]

    uow.SaveChanges();
}
````

You do not need to implement your repository interface, this is completly done by the [Repository Proxy Builder](../../articles/Core/DataModel/RepositoryProxyBuilder.md).

## Database Migration

Half of way is done for now.
We are prepared to do the first step of migration. Database migration is a hard business if you have many changes made to your data model.
The EntityFramework comes with a tool to make the migration hassle a little bit easier.

Database migration consists of a migration configuration that is needed for every `DbContext` and a set of migration steps. Both things are generated by migration scripts.

The entity framework comes with three scripts to do database model migration:

| Script | Explaination |
|---------------------|---|
| `Enable-Migrations` | As the name implicits this script initializes the project and makes it ready for migration |
| `Add-Migration`     | This script is executed every time you want to create a new migration |
| `Update-Database`   | This runs all database migrations found in the corresponding assembly. This step can also be called from code. |

When EF guys are talking about the *CodeFirst Migrations* approach they mean exactly these three scripts.

### Initial creation of migration configuration

Before we implement the UnitOfWork you can configure your migration.
This is an easy step to take, because MORYX has prepared a few things for you.
But before we call the Enable-Migration script you need to add `Npgsql` & `System.Threading.Tasks.Extensions` via Nuget to your project.
After that, you need to open your `App.config` and add the following lines:

````xml
  <configSections>
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
  </configSections>
  <entityFramework>
    <providers>
      <provider invariantName="Npgsql" type="Npgsql.NpgsqlServices, EntityFramework6.Npgsql" />
    </providers>
  </entityFramework>
````

If you found an already existing section called configSections and entityFramework replace it.
The migration script needs this to identify the driver to communicate with the database.
Note: If you use another version of EntityFramwork you need to change the full qualified assembly name.

Now, open the `Package Manager Console`
(You may ask 'Really?
Why the Package Manager Console?'
Thats because in Visual Studio the Package Manager Console also is the Powershell console within Visual Studio).
Then enter the following command

````ps
Enable-Migrations -ContextTypeName SolarSystemContext -EnableAutomaticMigrations `
    -ProjectName Moryx.TestTools.Test.Model `
    -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=NpgsqlTest" ` -ConnectionProviderName Npgsql
````

and execute it.
This will create a folder named Migrations in the project and a `Configurations.cs` file within.
As mentioned before Moryx has already done some work for you.

### Add-Migrations

Now time has come to create the initial migration. You need to ensure that the target database does NOT exist yet.
This forces the Add-Migration script to create the code for a full database setup.

To start, type the following line to the `Package Manager Console`:

````ps
Add-Migration -Name InitialCreate -ProjectName Moryx.TestTools.Test.Model `
    -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=NpgsqlTest" ` -ConnectionProviderName Npgsql
````

That's it. The script should have added a new class to the `Migrations` folder of the project. It's name should look like `201801120742211_InitialCreate`. The leading numbers will be different.

### Update-Database

The last step to do, is to apply the migrations to the database so that your context and its entities are on the same version.
Now the last of the three scripts comes into play.
Copy the following line to the Package Manager Console:

````ps
Update-Database -TargetMigration Version1 -ProjectName Moryx.TestTools.Test.Model `
    -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=NpgsqlTest" ` -ConnectionProviderName Npgsql -Verbose
````

The database should now have been updated. Have a look.

### Repeat when necessary

When you have changed your entities or your context while developing you only need to call the `Add-Migration` script once more.
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

## Freqeuently asked questions

- Is it possible to add several migrations to the same database or schema?
  Yes, that's possible. But you have to be carefull and avoid name collisions.
- Is it possible to downgrade to an older migration?
  Yes, but you have to take care of your context implementation that will differ from the older migration.
- Is it possible to change the schema later?
  Yes. Just change the schema.
- I need to do some additional data migration tasks. Where should I implement them?
  Every generated migration can be changed later. There you can call custom SQL statements.
