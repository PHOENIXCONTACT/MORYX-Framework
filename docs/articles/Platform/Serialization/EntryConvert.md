---
uid: EntryConvert
---
# Entry Convert

The static class [EntryConvert](xref:Moryx.Serialization.EntryConvert) transforms classes or objects into the MORYX [entry format](xref:Moryx.Serialization.Entry) and back. If you are not familiar with this format, please read this section first: [EntryFormat](xref:EntryFormat)

Because the `EntryConvert`-API can be confusing on first sight, we will split it into three sections. Each of these sections is explained in detail and supported with numerous examples.

* **Serialize:** Serialization of types, objects and properties is provided by all methods and overloads starting with `Encode` - `EncodeProperty`, `EncodeClass` and `EncodeObject`
* **Deserialize:** The encoded entry tree can be converted into objects after modification by the client with the overloads of `CreateInstance` and `UpdateInstance`
* **Customization:** The behavior of both encoding and decoding can be customized to specific needs by providing a strategy implementing `ICustomSerialization`

## Limitations

The `EntryConvert` API can convert objects and types as long as they comply with a few basic rules:

* Properties not fields: All attributes of a type must be defined as properties, not public fields. Therefor `public int Foo { get; set; }` instead of `public int Foo;`
* Public parameterless constructor: All types within the class hierarchy need to offer a public constructor without parameters. In Generics this would be defined as `new()` or in code `public Foo() { }`
* Primitives or classes: The reflection approach used to deserialize the entry tree to objects requires reference access. Otherwise the modifications will only take part on a copy. Therefor properties need to be either of a primitive type like int, string, enum or a another class.
* Dictionaries of `<Primitive, Class`: Dictionaries are only supported if the key is a primitive type like `int` or `string` and the value is a class.

## Serialize Objects

The return value of `EncodeClass` or `EncodeObject` is a collection of entries, that contain the properties of the given argument and their recursive children. It does not return a root entry for two reasons - information like identifier (key) are missing and in most cases the root object already has a DTO and only needs a generic way to include properties of derived classes.

Let's look at this with an example:

````cs
public class Foo
{
    public int Id { get; set; }
}

public class DerivedFoo : Foo
{
    public string SomeName { get; set; }
}

[DataContract]
public class FooDto
{
    public int FooId { get; set; }

    public Entry[] Properties { get; set; }
}

public void Serialize()
{
    var fooObj = new DerivedFoo
    {
        Id = 42,
        SomeName = "Bob"
    };
    var dto = new FooDto
    {
        Id = fooObj.Id,
        Properties = EntryConvert.EncodeObject(fooObj).ToArray()
    };
}
````

In the example we would not need to know the definition of `Derived` and could still send it to the client for modification by the user. If we do not have an object `fooObj` yet, we could also encode the type and later create an object from the clients response.

````cs
public void SerializeType()
{
    var type = typeof(DerivedFoo);
    var creator = new FooDto
    {
        Properties = EntryConvert.EncodeClass(type).ToArray()
    };
}
````

If you try this examples yourself, you will notice, that properties also contains an entry with key `Id`, which is redundant with `FooDto` and part of the possibilities described for `ICustomSerialization`.

## Deserialize Objects

When converting the modified `IEnumerable<Entry>` back into an object there are two choices - creating a new object or updating an existing one. Using the `FooDto` from the previous example both options are shown below.

````cs
public void Deserialize()
{
    FooDto dto = FromService();
    dto.Properties[0].Value.Current = "Joe";

    var fooObj = EntryConvert.CreateInstance<DerivedFoo>(dto.Properties);

    dto.Properties[0].Value.Current = "Michael";
    EntryConvert.UpdateInstance(fooObj, dto.Properties);
}
````

## Serialize and deserialize Streams

The EntryConvert also supports serialization and deserialization of [Streams](xref:System.IO.Stream). Therefore the stream is serialized by converting the content to a Base64 encoded string. Many stream types are supported like [MemoryStream](xref:System.IO.MemoryStream) or [FileStream](xref:System.IO.FileStream).

### Limitations on serializing and deserializing Streams

* The conversion to a Base64 string is done in memory, so memory is the limiting factor. Don't convert hundred of megabytes of data.
* Deserialize tries to keep the origin stream instance. If the stream is seekable the deserializer seeks to the beginning of the stream. If the source stream is a `MemoryStream` the deserializer uses only the same instance if the `MemoryStream` is at least as big as the data to be applied. Otherwise a new instance is created.
* If a target stream is not writeable in general a new `MemoryStream` instance is created.
* If your target stream is a `FileStream` the file has to be opened in write mode to work correctly.
* If the origin buffer is greater than new data the origin buffer gets truncated.

### Examples

The following example shows a sample class containing a `FileStream` instance. The function `Serialize` shows what to call to encode to a new `Entry`. The function `Deserialize` shows how an existing instance is updated with a corresponding `Entry`.

````cs
// Sample class that contains a FileStream object
public class FileStreamDummy
{
    public FileStreamDummy(string filePath, FileMode mode)
    {
        FileStream = new FileStream(filePath, mode);
    }

    public FileStream FileStream { get; set; }
}

// Serialize the FileStream to an Entry
public Entry Serialize()
{
    // Boilerplate, only for this example
    var dummy = new FileStreamDummy(..., FileMode.Create);

    var data = Encoding.UTF8.GetBytes("Some information about something");
    dummy.FileStream.Write(testBytes, 0, testBytes.Length);

    // Magc is done here
    return EntryConvert.EncodeObject(dummy);
}

// Fill the data of Entry to dummy. Note that the FileStream within dummy is reused.
public void Deserialize(Entry entry, FileStreamDummy dummy)
{
    // Apply entry data
    EntryConvert.UpdateInstance(dummy, entry);
}
````

## ICustomSerialization

All public methods of `EntryConvert` have overloads that expect an instance of [ICustomSerialization](xref:Moryx.Serialization.ICustomSerialization) to modify the behavior of the serializer where necessary. The overloads without the parameter use a Singleton instance of `DefaultSerialization`. When implementing a new version of `ICustomSerialization` it is recommended to derive from [DefaultSerialization](xref:Moryx.Serialization.DefaultSerialization) and only override what shall behave differently.

````cs
public class FooSerialization : DefaultSerialization
{
    public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
    {
        // Ignore the Id property. This example is bad practice because it will ignore every Id, not just on Foo class
        return base.GetProperties(sourceType).Where(property => property.Name != nameof(Foo.Id));
    }

    public override string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
    {
        if (memberType.Name == nameof(DerivedFoo.SomeName))
            return new [] { "Thomas", "Dennis", "Slawa", "Robert", "Moryx", "Michael", "Sascha" };

        return base.PossibleValues(property);
    }
}
````

After you customized the behavior you can then apply it to the serializer by passing it into the `Encode` or `Create`/`Update` methods like shown in the two modified examples.

````cs
var serialization = new FooSerialization();
var dto = new FooDto
{
    Id = fooObj.Id,
    Properties = EntryConvert.EncodeObject(fooObj, serialization).ToArray()
};
dto.Properties[0].Value.Current = "Michael";
EntryConvert.UpdateInstance(fooObj, dto.Properties, serialization);
````

## Serialize Methods

In the other sections you have learned that `EntryConvert` is able to serialize and deserialize objects. With the `GetMethods` and `InvokeMethod` features of `EntryConvert` you are able to build your own `RPC (Remote Procedure Call)` service.

To enable the `RPC` features of `EntryConvert` you need to use the [EditorBrowsableSerialization](xref:Moryx.Serialization.EditorBrowsableSerialization) serializer on `EntryConvert`. Then you add the [EditorBrowsableAttribute](xref:Moryx.Serialization.EditorBrowsableAttribute) to all private/public methods or properties you want to expose.

````cs
public class MyLittleRPC
{
    [EditorBrowsable, Description("Does soemthing parameterized")]
    public bool DoSomething(MyParams parameters)
    {
        return true;
    }
}
````

The serialization will be done by the method ``

````cs
public MethodEntry[] GetMethods(string moduleName)
{
    return EntryConvert.EncodeMethods(MyLittleRPC, CreateSerialization()).ToArray();
}

private ICustomSerialization CreateSerialization()
{
    return new AdvancedEditorBrowsableSerialization(Container, ConfigManager);
}
````

## Invoke Methods

The following code allows you to expose and invoke you `RPC` methods or properties.

````cs
public Entry InvokeMethod(MethodEntry method)
{
    var result = EntryConvert.InvokeMethod(MyLittleRPC, method, CreateSerialization());
    return result;
}
````