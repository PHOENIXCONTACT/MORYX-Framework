---
uid: Model.RepositoryProxyBuilder
---
# Repository Proxy Builder

The [RepositoryProxyBuilder](../../../../src/Moryx.Model/Repositories/Proxy/RepositoryProxyBuilder.cs) is the fancy component which is responsible for generating classes for given `Repository`-API's and also existing `abstract` classes.

The builder provides runtime generated types for predefined APIs for example:

````cs
public class CarEntity : ModificationTrackedEntityBase
{
    public virtual string Name { get; set; }

    public virtual int Price { get; set; }

    public virtual byte[] Image { get; set; }

    public virtual ICollection<WheelEntity> Wheels { get; set; }
}
````

````cs
public interface ICarEntityRepository : IRepository<CarEntity>
{
    CarEntity Create(string name);
}
````

## General

The builder evaluates the method name in general. The name will be used to select a matching strategy which can implement the method. Each strategy evaluates the method arguments and tries to match it with the entities property name for example the `Create(string name)` will be mapped to the `CarEntity.Name` property.

## Type by interface

A type can be created by only giving an interface like below.

````cs
var repoBuilder = new RepositoryProxyBuilder();
var impl = repoBuilder.Build(typeof(ICarEntityRepository)).
````

With this information the builder creates a type with the name `CarEntityRepository_MoryxProxy` for the given `ICarEntityRepository`. It tries to implement all declared methods otherwise it throws an `InvalidOperationException` if something is not possible or not specific enough.

The builder automatically detects that the `CarEntity` is derived from `ModificationTrackedEntityBase` and uses the special `ModificationTrackedRepositoryBase` as base class (further reading here: [ModificationTracking](ModificationTracking.md))

## Type by abstract class

If you need an implementation with more specific or special implemented methods it is also possible. The builder has another overload of the `Build` method to provide a predefined type.

````cs
public interface ISportCarRepository : IRepository<SportCarEntity>
{
    CarEntity Create(string name);

    SportCarEntity SomeSpecialFilter(string name);
}

public abstract class SportCarRepository : ModificationTrackedRepository<SportCarEntity>, ISportCarRepository
{
    // This method will be implemented by the builder
    public abstract CarEntity Create(string name);

    // This method remains untouched
    public SportCarEntity SomeSpecialFilter(string name)
    {
        return DbSet.Single(s => s.Name == name && s.Name.Contains("Hello"));
    }
}
````

The type will be generated with following call:

````cs
var repoBuilder = new RepositoryProxyBuilder();
var impl = repoBuilder.Build(typeof(ISportCarRepository), typeof(SportCarRepository)).
````

## Method implementation strategies

### Create

The strategy `CreateMethodStrategy` searches for methods where the name is starting with `Create`. The implementation creates an method to create the entity and assigns all properties where the argument name is matching.

````cs
public interface ICarEntityRepository : IRepository<CarEntity>
{
    CarEntity Create(string name);
}
````

The implementation which is done by the builder will be the following:

````cs
public class CarEntityRepository_MoryxProxy : ICarEntityRepository
{
    public CarEntity Create(string name)
    {
        var entity = DbSet.Create();
        entity.Name = name;
        return entity;
    }
}

````

### Filter

The strategy `FilterMethodStrategy` is a bit more complex. At start it is searching for methods which are starting with `Get` in the name. The second keyword will be the filter for example `All` or `First`. The third keyword is the selector which can be `By` or `Contains`.

**Possible Filter Keywords**:

| Keyword | Description |
|---------|-------------|
| All | Will select all entities. It uses the Linq `Where` statement. |
| First | Filters only the first entity which matches. Default will be null. It uses the Linq `First` statement. |
| FirstOrDefault | Filters only the first entity which matches. Default will be null. It uses the Linq `FirstOrDefault` statement. |
| Single | Filters only the a single entity which matches. It uses the Linq `Single` statement. |
| SingleOrDefault | Filters only the a single entity which matches. Default will be null. It uses the Linq `Single` statement. |

An example for it can be `GetFirstBy(string name)`. This method name will be implemented by the builder like the following code sample. The other methods are straight forward:

````cs
public CarEntity GetFirstBy(string name)
{
    return DbSet.First(c => c.Name == name);
}
````

**Possible Selector Keywords**:

| Keyword | Description |
|---------|-------------|
| By | This is also the default. It will be implemented by a default equals comparrison. |
| Contains | If the argument is a `string` it applies the `contains` expression. Otherwise its behavior is like the selector `By`. |

An example for it can be `GetFirstContains(string name)`. This method name will be implemented by the builder like the following code sample:

````cs
public CarEntity GetFirstContains(string name)
{
    return DbSet.First(c => c.Name.Contains(name));
}
````

Other value types like `int` or `long` are implemented by default equals comaprrisons.

````cs
public CarEntity GetFirstContains(string name, int power)
{
    return DbSet.First(c => c.Name.Contains(name) && c.Power == power);
}
````

### Remove

Due missing implementation of the `RemoveMethodStrategy` only the following remove functions will be implemented:

* void Remove(T entity)
* void Remove(T entity, bool permanent)
* void RemoveRange(IEnumerable\<T> entities)
* void RemoveRange(IEnumerable\<T> entities, bool permanent);

## Samples

With all this explainations in the filters and selector section below an repository API could look as follows:

````cs
public interface ICarEntityRepository : IRepository<CarEntity>
{
    // Create Strategy
    CarEntity Create(string name);
    CarEntity Create(string name, int price);
    CarEntity Create(string name, byte[] image);
    //TODO: Navigation property collection
    //CarEntity Create(string name, IEnumerable<WheelEntity> wheels);

    // Filter Strategy: Where
    IEnumerable<CarEntity> GetAllBy(string name);
    List<CarEntity> GetAllBy(int price);
    ICollection<CarEntity> GetAllBy(string name, int price);

    ICollection<CarEntity> GetAllContains(string name);
    ICollection<CarEntity> GetAllContains(string name, int price);

    // Filter Strategy: Single and SingleOrDefault:
    CarEntity GetSingleBy(string name);
    CarEntity GetSingleContains(string name);
    CarEntity GetSingleOrDefaultBy(string name);
    CarEntity GetSingleOrDefaultContains(string name);

    // Filter Strategy: First and FirstOrDefault:
    CarEntity GetFirstBy(string name);
    CarEntity GetFirstContains(string name);
    CarEntity GetFirstOrDefaultBy(string name);
    CarEntity GetFirstOrDefaultContains(string name);

    // Filter Strategy: Default Filter and Defaulf Selector
    CarEntity GetBy(string name); // same as GetFirstOrDefaultBy
    CarEntity GetContains(string name); // same as GetFirstOrDefaultContains
    CarEntity Get(string name); // same as GetFirstOrDefaultBy

    ICollection<CarEntity> GetAllByName(string name); // postfix does not change behavior
    ICollection<CarEntity> GetAllContainsName(string name); // postfix does not change behavior
    ICollection<CarEntity> GetAllByNameAndPrice(string name, int price); // postfix does not change behavior
}
````
