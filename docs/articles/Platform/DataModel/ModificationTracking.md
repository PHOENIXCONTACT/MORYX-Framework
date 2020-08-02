---
uid: Model.ModificationTracking
---
# Modification Tracking

## Introduction

Sometimes developers want to keep track about modifications of a row in their table. MORYX framework has a built in solution to keep track about creation, update and deletion time. So another implicit feature is that you can mark a row as deleted without to get rid of the row by using the `DbSet.RemoveTracked(entity)`-extension.

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

The example above shows the minimum working implementation.

## Migrations

You do not need to treat modified trackable entities in another way. It will work with no further action.

## A last word

If you are interessted how the modified trackable mechanism is implemented you'll find some deeper information in [Code First](xref:GettingsStarted.CodeFirst) article.