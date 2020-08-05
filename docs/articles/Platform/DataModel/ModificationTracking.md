---
uid: Model.ModificationTracking
---
# Modification Tracking

## Introduction

Sometimes developers want to keep track about modifications of a row in their table. MORYX framework has a built in solution to keep track about creation, update and deletion time.

To make an entity trackable you just need to derive from [IModificationTrackedEntity](xref:Moryx.Model.IModificationTrackedEntity) or the corresponding base class [ModificationTrackedEntityBase](xref:Moryx.Model.ModificationTrackedEntityBase).

````cs
public class PersonEntity : ModificationTrackedEntityBase
{
    public virtual string Name { get; set; }
}
````

The example above defines four aditional columns: Id (from `EntityBase`), Created, Updated and Deleted. The last three properties are introduced by the `ModificationTrackedEntityBase` and will be used in a special way by the framework and the database:

- `Created`: Indicates the time when the row was created
- `Updated`: Shows when the row was updated
- `Deleted`: Is set when the row was deleted.

`Created` and `Updated` are automatically set by the db context base class [MoryxDbContext](xref:Moryx.Model.MoryxDbContext) whenever the entity was modified. Your context must derive from it. The `Deleted` flag is only automatically set, if the [UnitOfWork with Repositories](UnitOfWorkPattern.md) is used, otherwise if plain EntityFramework, the `DbSet`-extension `RemoveSoft` is the way to set the flag. The `RemoveSoft` extension only sets the `Deleted`-property on the entity to the current datetime. To synchronize the DateTimes, the `MoryxDbContext` will modify the `Deleted`-property again to match with the `Updated`-property.

## Migrations

You do not need to treat modified trackable entities in another way. It will work with no further action.

## A last word

If you are interessted how the modified trackable mechanism is implemented you'll find some deeper information in [Code First](xref:GettingsStarted.CodeFirst) article or the [EntityFramework Tutorial](https://www.entityframeworktutorial.net/faq/set-created-and-modified-date-in-efcore.aspx)