---
uid: Model.MoryxDbContext
---
# MoryxDbContext

The `MoryxDbContext` is an inherited `DbContext` which brings a set of additional features:

- [Modification Tracking](modification-tracking.md)
- [Automatic DateTimeKind conversion](#DateTimeKind-conversion)
- Support of [`DefaultSchemaAttribute`](/src/Moryx.Model/Attributes/DefaultSchemaAttribute.cs)

## DateTimeKind conversion

Sometimes it is necessary to have a consistent DateTime conversion in the database. MORYX brings an model annotation to configure the DateTime properties. For its simplest application, you need to add a call to `ApplyDateTimeKindConverter()` in `OnModelCreating`. By default, UTC is used for all DateTime properties. Exceptions can be implemented via the `DateTimeKindAttribute` or the fluent api method `HasDateTimeKind`

````cs
modelBuilder
    .Entity<Foo>()
    .Property(e => e.SomeDate)
    .HasDateTimeKind(DateTimeKind.Local);
````
