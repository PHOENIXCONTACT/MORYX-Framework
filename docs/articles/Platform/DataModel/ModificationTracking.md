---
uid: Model.ModificationTracking
---
# Modification Tracking

## Introduction

Sometimes developers want to keep track about modifications of a row in their table. MARVIN framework has a built in solution to keep track about creation, update and deletion time. So another implicit feature is that you can mark a row as deleted without to get rid of the row.

To make an entity trackable you just need to derive from [ModificationTrackedEntityBase](Marvin.Model.ModificationTrackedEntityBase).

````cs
public class PersonEntity : ModificationTrackedEntityBase
{
    public string Name { get; set; }
}
````

The example above defines four aditional columns: Id (from `EntityBase`), Created, Updated and Deleted. The last three properties are introduced by the `ModificationTrackedEntityBase` and will be used in a special way by the framework and the database:

- `Created`: Indicates the time when the row was created
- `Updated`: Shows when the row was updated
- `Deleted`: Is set when the row was deleted.

Now you have to define the repository. Usually you define only a new interface and derive it from [IRepository\<T\>](xref:Marvin.Model.IRepository`1). But if your entity is modification trackable you also need to implement this new interface. The implementation has to derive from [ModificationTrackedRepository\<T\>](xref:Marvin.Model.ModificationTrackedRepository`).

````cs
public IPersonRepository : IRepository<Person>
{
}

public class PersonRepository : ModificationTrackedRepository<PersonEntity>, IPersonRepository
{
}
````

Because of the own repository implementation you need to register the interface and the implementation in your UnitOfWorkFactory.

````cs
[ModelFactory(TestModelConstants.Name)]
public class PersonUnitOfWorkFactory : NpgsqlUnitOfWorkFactoryBase<PersonContext>
{
    protected override void Configure()
    {
        RegisterRepository<IPersonRepository, PersonRepository>();
    }

    protected override DbMigrationsConfiguration<PersonContext> MigrationConfiguration => new Migrations.Configuration();
}
````

The example above shows the minimum working implementation.

## Automatic runtime generated repositories

As described in [Repository Proxy Builder](xref:Model.RepositoryProxyBuilder) some functions to access your data are generated automatically. This is also true when you create your own repository implementation. It is also possible by respect to the naming rules to define additional functions.

````cs
public IPersonRepository : IRepository<Person>
{
    PersonEntity GetSingleBy(string name);
}

public abstract class PersonRepository : ModificationTrackedRepository<Person>, IPersonRepository
{
    public abstract PersonEntity GetSingleBy(string name);
}
````

This will add an implementation to GetSingleBy and would result in a Person with the given name or null.

## How to really delete the row

In some cases you want to get really rid of a row. To send the row to the data nirvana just have a look on the following code snippet.

````cs
public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

public void WriteSomethingToDB()
{
    // Create a UnitOfWork instance
    using(var unitOfWork = UnitOfWorkFactory.Create())
    {
        // Get the repository you want to work with
        var personRepo = unitOfWork.GetRepository<IPersonRepository>();

        // Get all persons
        var persons = personRepo.GetAll();

        // Delete the first person and delete it permantly
        houseRepository.Remove(persons.First(), true);

        // save it
        unitOfWork.Save();

        // Done
    }
}
````

## Migrations

You do not need to treat modified trackable entities in another way. It will work with no further action.

## A last word

If you are interessted how the modified trackable mechanism is implemented you'll find some deeper information in [Code First](xref:GettingsStarted.CodeFirst) article.