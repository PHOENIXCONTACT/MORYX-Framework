---
uid: Model.UnitOfWorkPattern
---
# UnitOfWork Pattern & Repository Pattern

This document will give you a short view of what unit of work and repositories are about and how them are used in the MORYX framework.

## UnitOfWork Pattern

The main goal of the unit of work pattern is the easy support of transactional database operations ([CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete)). In other words: you want to rollback a bunch of changes made to the content of the database when one operation failed.

## Repository Pattern

If we look back to the old times before Entity Framework, the developer implemented the SQL statements into an IRepository implementation.

Now in times of EF you implement the LINQ expressions into the repository implementation. A very big disadvantage of this approach is that you have to write many boiler plate code to access your data. This leads to typical problems of boiler plate code: It needs to be maintained and may consists of bugs.

## The MORYX way

The MORYX framework follows another strategy: You don't need to implement an own UnitOfWork class becuase it is the DbContext and the repositories are generated at runtime. So no code needs to be maintained.

Let's have a look on an example implementation how you define the UnitOfWork and Repository aproach in the MORYX framework.

````cs
public interface IPersonEntityRepository : IRepository<PersonEntity>
{
    // Add here additional functions to ease your life
}
````

And here is how you use it.

````cs
// Note that this is just an example to show which calls you have to make
// to work with your database.

public void WriteSomethingToDB()
{
    var context = DbContextFactory.Create<PersonContext>();

    // Get the repository you want to work with
    var personRepo = context.GetRepository<IPersonEntityRepository>();

    // Get all persons
    var persons = personRepo.GetAll();

    // Create a person
    var newPerson = houseRepository.Create();

    // Change it
    newPerson.Name = "Spock";

    // save it
    context.SaveChanges();
}
````

As you can see you just need to write the most important things. You might wonder how the magic of runtime code generation in context to the repository works. If you want to know more have a look on the [RepositoryProxyBuilder](xref:Model.RepositoryProxyBuilder).

For a complete example please read the [CodeFirst tutorial](xref:GettingsStarted.CodeFirst#Customizations to the migration process).