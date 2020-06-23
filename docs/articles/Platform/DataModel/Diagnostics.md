# Model Diagnostics

The implementation of `IUnitOfWork` also provides an implementation for [IModelDiagnostics](xref:Moryx.Model.IModelDiagnostics). This interface provides diagnostics features for the `DbContext`.

## Logging

The `UnitOfWork` provides database logging features. All actions executed by the underlying `DbConnection` will be sent to this method.
The following sample shows how the database log will be sent to the `Console`:

````cs
using (var uow = Factory.Create())
{
    ((IModelDiagnostics)uow).Log = Console.Out.WriteLine;
    // Logging is active
}
````

The output prints all actions and can look as follows:

````text
Opened connection at 11.04.2018 11:42:10 +02:00

SELECT  CASE  WHEN (NOT ("Project1"."C1" = TRUE AND "Project1"."C1" IS NOT NULL)) THEN (E'0X') ELSE (E'0X0X') END  AS "C1", "Extent1"."Id", "Extent1"."Name", "Extent1"."Price", "Extent1"."Image", "Extent1"."Created", "Extent1"."Updated", "Extent1"."Deleted",  CASE  WHEN (NOT ("Project1"."C1" = TRUE AND "Project1"."C1" IS NOT NULL)) THEN (CAST (NULL AS int4)) ELSE ("Project1"."Performance") END  AS "C2" FROM "cars"."CarEntity" AS "Extent1" LEFT OUTER JOIN (SELECT "Extent2"."Id", "Extent2"."Performance", TRUE AS "C1" FROM "cars"."SportCarEntity" AS "Extent2") AS "Project1" ON "Extent1"."Id" = "Project1"."Id" WHERE E'Car 1' = "Extent1"."Name"

-- Executing at 11.04.2018 11:42:10 +02:00

-- Completed in 1 ms with result: NpgsqlDataReader

Closed connection at 11.04.2018 11:42:11 +02:00
````