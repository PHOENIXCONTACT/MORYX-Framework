---
uid: GettingStarted.ModelSetups
---
# Model Configurators

In MORYX, you need `IModelConfigurators` to connect to a database. At the time
of writing this, there are several notable configurators shipped within 
MORYX-Core:

* `NpgsqlModelConfigurator` (`Moryx.Model.PostgreSQL`)
* `SqliteModelConfigurator` (`Moryx.Model.Sqlite`)


# _Custom_ Model Configurators

In case you can't go with the existing configurators, you might want to create
your own. Therefore, create a new project referencing the `Moryx.Model`
namespace.

## `IDatabaseConfig` and `DatabaseConnectionSettings`

First of, you need to create an implementation of `IDatabaseConfig`. To do that
you should derive just from `DatabaseConfig<T>` where `T` is of `DatabaseConnectionSettings`.

A very first implementation, might look like this:

```csharp
[DataContract]
public class CustomDatabaseConfig : DatabaseConfig<DatabaseConnectionSettings>
{
    public  CustomDatabaseConfig()
    {
        ConnectionSettings = new DatabaseConnectionSettings();
    }
}
```

But instead of using the `DatabaseConnectionSettings` you should create a
derived class, that reflects the needed settings of your specific database. 
For example:

```csharp
public class CustomDatabaseConnectionSettings : DatabaseConnectionSettings
{
    [DataMember, Required]
    public override string Database { get; set; }

    [DataMember, Required, DefaultValue("Username=user;Password=pass;Host=127.0.0.1;Port=1234")]
    public override string ConnectionString { get; set; }

    public NpgsqlDatabaseConnectionSettings()
    {
        // Initialize `ConnectionString` with default value
        ConnectionString = ConnectionString?.GetType()
            .GetCustomAttribute<DefaultValueAttribute>().ToString();
    }
}
```

Use the following attributes to make properties available to API users:

* `DataMember` adds a property to the list of configurable properties
* `Required` indicates whether a property is required to be configured
  or not
* `DefaultValue` allows to provide a default value for the given property


## `IModelConfigurator`

Finally, you need to create the *Configurator* itself. Therefore, derive a 
class from `ModelConfiguratorBase<T>` where `T` is the previously created 
`CustomDatabaseConfig` (implementation of `IDatabaseConfig`) in order to
implement the `IModelConfigurator` interface. 

The `ModelConfiguratorBase<T>` provides built-in functionality for initializing
it from a config file, applying migrations etc.

You might give it a `DisplayName` to pass a descriptive name to your API users:

```csharp
[DisplayName("Custom database connector")]
public class CustomModelConfigurator : ModelConfiguratorBase<CustomDatabaseConfig>
{
    // ...
}
```

## Conclusion

These three classes now make the entry point to build up your own database
connection.