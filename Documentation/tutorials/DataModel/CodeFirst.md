---
uid: GettingsStarted.CodeFirst
---
# Code first

This article describes the usage of the Code First approach with [Entity Framework](https://docs.microsoft.com/en-us/ef/) and [Npgsql](http://www.npgsql.org) in the world of the MaRVIN Platform.

The database access layer is implemented with help of the [UnitOfWork Repository Pattern](xref:Model.UnitOfWorkPattern). For the further reading it is necessary to have a rough understanding about it.

MaRVIN fully supports the Code First approach. The next lines of code will show you exemplary how the Code First approach can be implemented.

## Basics

You have to implement a few basics to get started working with the PostgreSQL database. The things to implement are:

| Component | Description |
|-----------|-------------|
| [DbContext](https://msdn.microsoft.com/en-us/library/jj729737.aspx) | The primary class that is responsible for interacting with data as objects (often referred to as context). The context class manages the entity objects during run time, which includes populating objects with data from a database, change tracking, and persisting data to the database. |
| Entities | One entity is one table |
| Repositories | Helper functions to access a specific table. The MaRVIN framework implements a speciality to make your life easier. You just need to define the interface and the code will be generated at runtime. |
| UnitOfWorkFactory | Factory for the database within the MaRVIN context |

### A small view on constants

Constants are good practice if you want to make you life easier. You have one single source of truth which you can refactor much easier.

````cs
/// <summary>
/// String constants defined by the Products database model.
/// </summary>
public class SolarSystemModelConstants
{
    /// <summary>
    /// Namespace of the generated code within this model. This can be used for the
    /// ImportAttribute and UseChildAttribute.
    /// </summary>
    public const string Namespace = "Marvin.TestTools.Test.Model";

    /// <summary>
    /// Schema name for the database context
    /// </summary>
    public const string Schema = "testModel";
}
````

## The database DbContext

Create your own database context by deriving from [MarvinDbContext](xref:Marvin.Model.MarvinDbContext) and the three constructor overloads as shown below.

````cs
[DbConfigurationType(typeof(NpgsqlConfiguration))]
[DefaultSchema(SolarSystemModelConstants.Schema)]
public class SolarSystemContext : MarvinDbContext
{
    public SolarSystemContext()
    {
        // Important: Migration functions needing a parameterless constructor
    }

    public SolarSystemContext(string connectionString, ContextMode mode)
        : base(connectionString, mode)
    {
    }

    public SolarSystemContext(DbConnection connection, ContextMode mode)
        : base(connection, mode)
    {
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

Please note that we follow the policy to place tables of a context into to a different schema when possible. Please use the global `DefaultSchemaAttribute` on the context.

## Entities

Then you need to define your entities which will represented as tables in your database.

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

Mmh, the entities are either derived from [EntityBase](xref:Marvin.Model.EntityBase) or [ModificationTrackedEntityBase](xref:Marvin.Model.ModificationTrackedEntityBase). Is that necessary? That's pretty much better because the [EntityBase](xref:Marvin.Model.EntityBase) defines an extra property for the `Id`. This `Id` is treated specially as self incrementing primary key. [ModificationTrackedEntityBase](xref:Marvin.Model.ModificationTrackedEntityBase) derives from the base and has three special properties which are monitored by triggers (Created, Updated, Deleted). These trigger will automatically installed inside the database and tables. Further reading [Modification Tracking](xref:Model.ModificationTracking) here.

## Entity loading & change tracking behavior

The Entity Framework supports a set of [configurable features](https://msdn.microsoft.com/en-us/library/system.data.entity.infrastructure.dbcontextconfiguration.aspx) how the context behaves when you are accessing or changing entites. Please have a look on [Entity Framework Loading Related Entities](https://msdn.microsoft.com/en-us/library/jj574232(v=vs.113).aspx) if you want to dive deeper.

This two features are called lazy loading and change tracking.

### Change tracking

Entity Framework needs change tracking to create the changing SQL statement. So it has to know which column did change and which not. So you can decide between three options of tracking:

- **Snapshot Change Tracking**: EF keeps the original data "in mind" and compares them to the new version.
- **Dynamic Change Tracking**: Can only be used if proxies are enabled and the entity properties are `virtual`. The proxy sends a changed message so the framework does not need to compare the whole dataset.
- **Read only**: No change tracking is enabled. The access to the database is readonly.

### Entity loading

If an entity consists of one or more navigation properties you might want to have access to them. There are three ways definied to access these kind of properties:

| Mode | Explaination |
|---------------------|---|
| `Eager loading` | With eager loading it is possible to configure which relations shall be loaded every time (Have a look on the [Include extension](https://msdn.microsoft.com/en-us/library/system.data.entity.dbextensions.include.aspx)) |
| `Lazy loading`     | Lazy loading loads relations when you access them |
| `Explicit loading`   | You can load a relation explictly when lazy loading was turned off |

### Enable lazy loading

If you want to take profit about lazy loading setting the right `ContextMode` is only the half way. EntityFramework supports a per navigation property switch for lazy loading support. You need __explicitly__ define the navigation property as __`virtual`__ if you want lazy loading support to be enabled on this.

### MaRVIN specific

MaRVIN framework uses per default `Dynamic Change Tracking` and lazy loading but it is possbile to override these settings. You are allowed to change settings via [ContextMode](xref:Marvin.Model.ContextMode):

````cs
// Example how you can change the default setting via ContextMode on the `UnitOfWorkFactory`
// This call enables `Dynamic Change Tracking` only feature
var uow = uowFactory.Create(ContextMode.Tracking);

// or later with
uow.Mode = ContextMode.Tracking;
````

## Repositories

After that we need do define the repository interfaces.

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

If you got scared that you have to implement all these functions you are lucky they will be implemented automatically. So you only have to define the interfaces. If you want to know more about the automatic repository instantiation please have a look onto [Repository Proxy Builder](xref:Model.RepositoryProxyBuilder) page. The example functions defined above are also not necessary. Add just functions you really need.

But if you need a more specialized implementation of a repository you can either use a mixture of repository proxies and self implemented repository or your own repository implementation.

## Database Migration

Half of way is done for now. Now we are prepared to do the first step of migration. Database migration is a hard business if you have many changes made to your data model. The EntityFramework comes with a tool to make the migration hassle a little bit easier.

Database migration consists of a migration configuration that is needed for every `DbContext` and a set of migration steps. Both things are generated by migration scripts.

The entity framework comes with three scripts to do database model migration:

| Script | Explaination |
|---------------------|---|
| `Enable-Migrations` | As the name implicits this script initializes the project and makes it ready for migration |
| `Add-Migration`     | This script is executed every time you want to create a new migration |
| `Update-Database`   | This runs all database migrations found in the corresponding assembly. This step can also be called from code. |

When EF guys are talking about the *CodeFirst Migrations* approach they mean exaclty these three scripts.

### Initial creation of migration configuration

Before we implement the UnitOfWork you can configure your migration. This is an easy step to take because MaRVIN has prepared a few things for you. But before we call the Enable-Migration script you need to add `Npgsql` & `System.Threading.Tasks.Extensions` via Nuget to your project. After then you need to open your `App.config` and add the following lines:

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

If you found already a section called configSections and entityFramework replace it. The migration script needs this to identify the driver to communicate with the database. Note: If you use another version of EntityFramwork you need to change the full qualified assembly name.

Now, open the `Package Manager Console` (You may ask 'Really? Why the Package Manager Console?' Thats because in Visual Studio 2015 the Package Manager Console also is the Powershell console within the Visual Studio). Then enter the following command

````ps
Enable-Migrations -ContextTypeName SolarSystemContext -EnableAutomaticMigrations `
    -ProjectName Marvin.TestTools.Test.Model `
    -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=NpgsqlTest" ` -ConnectionProviderName Npgsql
````

and execute it. This will create a folder named Migrations into the project and a `Configurations.cs` file within. As mentioned before Marvin has already done some work for you.

### Add-Migrations

Now time has come to create the initial migration. You need to ensure that the target database does NOT exist. This forces the Add-Migration script to create the code for a full database setup.

To start type the following line to the `Package Manager Console`:

````ps
Add-Migration -Name InitialCreate -ProjectName Marvin.TestTools.Test.Model `
    -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=NpgsqlTest" ` -ConnectionProviderName Npgsql
````

That's it. The script should have added a new class to the `Migrations` folder of the project. It's name should look like `201801120742211_InitialCreate`. The leading numbers will be different.

### Update-Database

The last step to do is to apply the migrations to the database so that your context and its entities are on the same version. Now the last of the three scripts comes into play. the following line to the Package Manager Console:

````ps
Update-Database -TargetMigration Version1 -ProjectName Marvin.TestTools.Test.Model `
    -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=NpgsqlTest" ` -ConnectionProviderName Npgsql -Verbose
````

The database should now have been updated. Have a look.

### Repeat when necessary

When you have changed your entities or your context while development you only need to call the `Add-Migration` script once more. You have to consider two things: First ensure that your database is on the latest migration. Second change the `Name` parameter of the script to a speaking version name.

## The UnitOfWorkFactory

Lastly we need to implement the corresponding `UnitOfWorkFactory`. The base implementation comes with a set of predefined functions to manipulate your data. So you only need to register your repositories and the migration configuration.

````cs
[ModelFactory(SolarSystemModelConstants.Namespace)]
public class SolarSystemModelUnitOfWorkFactory : UnitOfWorkFactoryBase<SolarSystemContext, NpgsqlModelConfigurator>
{
    protected override void Configure()
    {
        RegisterRepository<IPlanetRepository>();
        RegisterRepository<ISatelliteRepository>();
        RegisterRepository<IAsteroidRepository>();
    }
}
````

Are we done now? No, but you have just implemented the basic data entities and the repository to access the data.

## Accessing your data

But how to access the data? The next snippet shows an example call of the unit of work. Note that usually a `UnitOfWorkFactory` is injected and the mock is not necessary.

````cs
// Create a Mock to inject the correct database config
var configManagerMock = new Mock<IConfigManager>();
configManagerMock.Setup(c => c.GetConfiguration<NpgsqDatabaseConfig>(It.IsAny<string>())).Returns(dbConfig);

// instantiate & initialize the unit of work factory (all proxies will be generated)
var uowFactory = new SolarSystemModelUnitOfWorkFactory { ConfigManager = configManagerMock.Object };
uowFactory.Initialize();

// create an instance
var uow = uowFactory.Create();

// retrieve your repository
var houseRepository = uow.GetRepository<IPlanetRepository>();
````

## Schema support

We added schema support to the migration routines so that you don't need to take care. It is possible to set the target schema of a table in two ways:

- On the table itself. You can do this with the `TableAttribute`: `[Table(nameof(Planet), Schema = SolarSystemModelConstants.Schema)]`. This attributes applies to the table where the attribute was placed.
- On the `DbContext`. You can do this with the `DefaultSchemaAttribute`: `[DefaultSchema(SolarSystemModelConstants.Schema)]`. This attribute applies to all tables within the context where the attribute was placed.

## Inheritance

The Entity Framework supports three inheritance strategies (an overview can be found [here](http://www.entityframeworktutorial.net/code-first/inheritance-strategy-in-code-first.aspx)):

| Strategy | Description |
|----------|-------------|
| Table per Hierarchy (TPH) | This approach suggests one table for the entire class inheritance hierarchy. Table includes discriminator column which distinguishes between inheritance classes. This is a default inheritance mapping strategy in Entity Framework. |
| Table per Type (TPT) | This approach suggests a separate table for each domain class. |
| Table per Concrete class (TPC) | This approach suggests one table for one concrete class, but not for the abstract class. So, if you inherit the abstract class in multiple concrete classes, then the properties of the abstract class will be part of each table of the concrete class. |

The default strategy the EF is using is the `Table per Hierarchy`.

### Inheritance example

In MaRVIN's world you may want to extend an existing entity. This chapter describes how you can inherit from an existing context and its entities. We will use `Table per Concrete class` to achieve inheritance. We assume that the inherited data context is part of a new project but the "old" project is referenced to it.

Create a new `Model` project and add the following classes to the project:

An yes, create also a new class for constant values:

````cs
/// <summary>
/// String constants defined by the Products database model.
/// </summary>
public class TestModelInheritanceConstants
{
    /// <summary>
    /// Namespace of the generated code within this model. This can be used for the
    /// ImportAttribute and UseChildAttribute.
    /// </summary>
    public const string Namespace = "Marvin.TestTools.Test.Model.Inheritance";

    /// <summary>
    /// Schema name for the database context
    /// </summary>
    public const string SchemaName = "public";
}
````

The inherited entity:

````cs
public class PlanetOfTheApes : Planet
{
    public virtual int ApeCount { get; set; }
}
````

The inherited context:

````cs
namespace Marvin.TestTools.Test.Model.Inheritance
{
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class SpecializedSolarSystemContext : SolarSystemContext
    {
        public SpecializedSolarSystemContext()
        {
            // Important: Migration functions needing a parameterless constructor
        }

        public SpecializedSolarSystemContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {
        }

        public virtual DbSet<PlanetOfTheApes> Planets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlanetOfTheApes>()
                .ToTable(nameof(PlanetOfTheApes));
        }
    }
}
````

A new migration configuration (Note: you always have to create a new migration configuration in every model project. This is a restriction by the EF.):

````cs
internal sealed class Configuration : DbMigrationConfiguration<SpecializedSolarSystemContext>
{
}
````

A new repository:

````cs
public interface IPlanetOfApesPlanetRepository : IRepository<PlanetOfTheApes>
{
}
````

And last but not least a `UnitOfWorkFactory`:

But before we can add the specialized UnitOfWorkFactory you have to change the SolarSystemContextUnitOfWorkFactory:

````cs
public abstract class SolarSystemModelUnitOfWorkFactory<TContext> : NpgsqlUnitOfWorkFactoryBase<<TContext>>
{
    protected override void Configure()
    {
        RegisterRepository<IPlanetRepository>();
        RegisterRepository<ISatelliteRepository>();
        RegisterRepository<IAsteroidRepository>();
    }
}

[ModelFactory(TestModelInheritanceConstants.Namespace.Namespace)]
public class SolarSystemModelUnitOfWorkFactory : SolarSystemModelUnitOfWorkFactory<SolarSystemContext>
{
    protected override DbMigrationsConfiguration<ProductsContext> MigrationConfiguration => new Migrations.Configuration();
}
````

And now the specialized one.

````cs
[ModelFactory(TestModelInheritanceConstants.Namespace, TestModelConstants.Namespace)]
public class SpecializedSolarSystemContextUnitOfWorkFactory : SolarSystemContextUnitOfWorkFactory<SpecializedSolarSystemContext>
{
    protected override DbMigrationsConfiguration<ProductsContext> MigrationConfiguration => new Migrations.Configuration();

    protected override void Configure()
    {
        RegisterRepository<IPlanetOfApesPlanetRepository>();
    }
}
````

That's all. Now you can use all migration stuff to create migrations.

## A word about the `ModelFactory`

You might wonder about the extended `ModelFactory` call. It's used to create a hierarchy inside the `UnitOfWorkFactory` and to get the right factory when a `IUnitofWorkFactory` gets injected.

So if you want to get a derived factory injected you have to add the `UseChild` attribute to your class:

````cs
public class MyPlanetObserver
{
    ...

    [UseChild(TestModelInheritanceConstants.Namespace)]
    public IUnitOfWorkFactory PlanetOfTheApesFactory { get; set; }

    ...
}
````

## Known Issues

**42P18: could not determine data type of parameter $1:** This exception happens if you use string concatenations or selecting string literals within database queries.

The first sample uses `string.IsNullOrEmpty` within a `Linq` query. `EntityFramework6.Npgsql` with version 3.1.x cannot handle this ([issue 62](https://github.com/npgsql/EntityFramework6.Npgsql/issues/62)). The EntityFramework driver cannot map a `null string` back to a `string` because it binds string parameter as Object.

````cs
var str = (string) null;
var carFilter = carRepo.Linq.Where(c => !string.IsNullOrEmpty(str) && c.Name.Contains(str));
````

A workaround is to souce the linq query independet code out:

````cs
var str = (string) null; // e.g. Method parameter

var carQuery = carRepo.Linq;

if (!string.IsNullOrEmpty(str))
    carQuery = carQuery.Where(c => c.Name.Contains(str));
````

The second sample uses `string` literals within a `Linq` selection. `String` literals are not supported ([issue 60](https://github.com/npgsql/EntityFramework6.Npgsql/issues/60)):

````cs
var testStr = "Hello";
var carFilter = carRepo.Linq.Select(c => new Filter
{
    Name = c.Name,
    Foo = testStr
});
````

A workaround for this problem is to add another iteration after filtering to set foo.

````cs
var carFilter = carRepo.Linq.Select(c => new Filter
{
    Name = c.Name,
}).ToArray();

var cars = carFilter.ForEach(car => car.Foo = "Hello");
````

The third sample uses concatenations within a `Linq` query:

````cs
var a = "C";
var b = " ar";

var carFilter = carRepo.Linq.Where(c => c.Name.Contains(a + b));
````

A workaround is to concat the string before executing the query:

````cs
var a = "C";
var b = " ar";
var carName = a + b;
var carFilter = carRepo.Linq.Where(c => c.Name.Contains(carName));
````

## Freqeuently asked questions

- Is it possible to add several migrations to the same database or schema?
  Yes, that's possible. But you have to take care and avoid name collisions.
- Is it possible to downgrade to an older migration?
  Yes, but you have to take care about your context implementation that will differ from the older migration.
- Is it possible to change the schema later?
  Yes. Just change the schema.
- I need to do some additional data migration tasks. Where should I implement them?
  Every generated migration can be changed later. There you can call custom SQL statements.