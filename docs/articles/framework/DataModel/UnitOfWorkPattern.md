---
uid: Model.UnitOfWorkPattern
---
# UnitOfWork Pattern & Repository Pattern

This document will give you a short overview of what unit of work and repositories are about and how they are used in the MORYX framework.

## UnitOfWork Pattern

The main goal of the unit of work pattern is the easy support of transactional database operations ([CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete)). In other words: you want to rollback a bunch of changes made to the content of the database when one operation failed.

## Repository Pattern

If you are unfamiliar with the Repository and Unit of Work pattern we recommend you start with an explanation of your choice from the Web. 

As the logic in MORYX is separated in modules each of them might need their own database to execute its tasks.
With a growing number of modules you will, however, find that a disadvantage of the pattern in general is that you have to write a lot of boiler plate code to access your data. 
This leads to typical problems of boiler plate code: It needs to be maintained and may consists of bugs.

## The MORYX way

The MORYX framework follows another strategy: You don't need to implement an own UnitOfWork class because MORYX has a generic one for your DbContext and the repositories are generated at runtime. So no code needs to be maintained. 
Also have a look at the [repository proxy builder](RepositoryProxyBuilder.md) for more information on how the code generation works.

Let's have a look at an example implementation. How do you define the UnitOfWork and Repository approach in the MORYX framework?

````cs
public interface IPersonEntityRepository : IRepository<PersonEntity>
{
    // Add here additional functions to make your life easier
}
````

And here is how you use it.

````cs
// Note that this is just an example to show which calls you have to make
// to work with your database.

// Injected
public IUnitOfWorkFactory<PersonContext> UnitOfWorkFactory { get; set; }

public async Task WriteSomethingToDB()
{
    using var uow = UnitOfWorkFactory.Create()

    // Get the repository you want to work with
    var personRepo = uow.GetRepository<IPersonEntityRepository>();

    // Get all persons
    var persons = await personRepo.GetAllAsync();

    // Create a person
    var newPerson = await personRepo.Create();

    // Change it
    newPerson.Name = "Spock";

    // save it
    await uow.SaveChangesAsync();
}

public async Task UseExtensionMethodsToFurtherSimplifyYouLife(Person person)
{
    // Create an entity for the existing business object 
    var personEntity = UnitOfWorkFactory.CreateEntity<PersonEntity>(person)
    
    // Make sure ID updates for the entity are reflected in the business object.
    // Usually this is only needed for new business objects
    var personEntity = UnitOfWorkFactory.GetEntity<PersonEntity>(person)
    

    // Find the existing entity that belongs to the business object 
    var personEntity = UnitOfWorkFactory.FindEntity<PersonEntity>(person)
    
    // Find the existing entity or create a new one for the business object 
    var personEntity = UnitOfWorkFactory.GetEntity<PersonEntity>(person)
    
    // Do something more...

    // Save it
    await uow.SaveChangesAsync();
}
````

As you can see you just need to write the most important things. 
The magic of the runtime code generation takes care of the rest.
For a complete example please read the [CodeFirst tutorial](/docs/tutorials/data-model/CodeFirst.md).
