---
uid: EntryFormat
---
# Entry Format

## Introduction

The MORYX entry format enables serialization and deserialization of objects and types. However unlike formats like JSON or XML it includes modification and creation information. This allows modification of custom properties and unknown types in a generic way. It enables serialization of complex types using the recursive `SubEntries` property. Subentries can be either properties of a class or items in a collection. It also supports inheritance and polymorphism. Classes may be created or replaced by derived types based on `Prototypes`.

Serialized entries can be then stored in JSON or send over WCF.

## Entry

The `Entry` class represents property definition. It consists of the properties `Identifier`, `DisplayName` and `EntryValue Value`. `Identifier` is set and used by the serializer to map the entry value to a property or a collection item. Depending on the entry type it is either a property name, collection index or dictionary key. The `DisplayName` is intended for the user to understand the meaning of the entry value. It can be identical with the `Identifier`, but does not have to be and unlike the `Identifier` it is not required to be unique within its context. Both collections `Prototypes` and `Subentries` as well as `EntryValidation Validation` are optional properties. They are only used for some of the possible types of entry types. Entry class can have tree structure where `Entry` instances are located in SubEntries and/or Prototypes.

We can decorate property with attributes:

- `DisplayNameAttribute` when provided then _DisplayName_ will be taken from this attribute
- `DescriptionAttribute` provides general description for Entry
- `ReadOnlyAttribute` when provided, the `EntryValue.IsReadOnly` will be set to `true`

## EntryValue

It holds the value of the `Entry`, its type and necessary modification information like `Possible` values. The type is defined by the [EntryValueType](../../../../src/Moryx/Serialization/EntryConvert/EntryValueType.cs) enum. It specifies the common primitives like `Int16`, `Double` or `String` and the two special types `Class` or `Collection`.

`EntryValueType.Class` is used either for properties that reference another object, items in a collection or as object prototypes. 

- `Current` contains the value of the property as a string. For `EntryValueType.Class` this contains the type name of the object to replace it from a prototype or detect inheritance.
- `Default` property holds value that should be put into `Current` when creating new instance of Entry or when the users wants to reset the value.
- `Possible` is a collection of possible values. If this collection is not empty, the current value must be on this list.
- `IsReadOnly` when set to true we can't change `Current` value.

## EntryValidation

Based on property attributes EntryValidation class can be created.

Possible attributes are:

- `PasswordAttribute` Property stores password
- `MinLengthAttribute` Minimum length
- `MaxLengthAttribute` Maximum length
- `RegularExpressionAttribute` Specifies that `Current` value must match provided regular expression 
- `StringLengthAttribute` Specifies the length of the value that should be provided 
- `RequiredAttribute` Specifies if Entry current value is required to be filled

Entry validation enables to check if changed property value fit validation criteria.

## SubEntries

The recursive `SubEntries` structure represents properties of a `EntryValueType.Class` entry or items of `EntryValueType.Collection` entry. Some class properties can be set with different derived types. For collections the `SubEntries` collection can be modified by the client. Items are deleted by simply removing from the collection and added by choosing a prototype entry to instantiate. Collections without polymorphism contain only one prototype.

## Examples

Example class:

````cs
public class Root
{
    [DisplayName("Bob")]
    public Foo Blah { get; set; }
}
public class Foo
{
    public int ValueA { get; set; }

    [DefaultValue("Hello")]
    public string ValueB { get; set; }
}
````

The root object as a entry tree:

````sh
Entry
- Key { Identifier = "Blah", Name = "Bob" }
- Value { Current = "Foo" }
- SubEntries
-- Entry-A
--- Key { Identifier = "ValueA", Name = "ValueA" }
--- Value { Current = "10" }
-- Entry-B
--- Key { Identifier = "ValueB", Name = "ValueB" }
--- Value { Current = "SomeString", Default = "Hello" }
````

## Prototypes

As the name suggests prototypes can be used to create instances by cloning the structure. This way the client can add items to collections without any additional calls to the server. If a collection can contain objects of subclasses, it will have different prototypes, that can be used.

Example with sub-classes:

````cs
// Reference to the collection entry
Entry collection = GetCollection(..);
// Fetch and instantiate prototype, possible by displaying possible values as drop-down box
var prototype = collection.GetPrototype("SubClass");
var instance = prototype.Instantiate();
// Add to collection
collection.SubEntries.Add(instance);
````
