---
uid: Bindings
---
# Bindings

The types of the `Moryx.Bindings`-namespace located in _Moryx_ offer the functionality to dynamically resolve or update properties of objects. An [IBindingResolver](/src/Moryx/Bindings/IBindingResolver.cs) is created by an instance of [IBindingResolverFactory](/src/Moryx/Bindings/IBindingResolverFactory.cs) from a string like `"Branch.Name"`. The resolver will then return the value of `Name` for each `object source` passed into the `Resolve(source)`-method.

It is also possible to set the value of the property by calling `Update(object, object)` on the resolver created by the factory.

Example of binding resolution and property update:

````cs
public class Root
{
    public Branch Branch { get; set; }
}

public class Branch
{
    public string Name { get; set; }
}

// Somewhere 
var resolverFactory = new BindingResolverFactory()
var resolver = resolverFactory.Create("Branch.Name");

var root = new Root()
{
    Branch = new Branch { Name = "Alice" }
};

var name = resolver.Resolve(root); // Value of name = "Alice"

// New or modified object
root = new Root()
{
    Branch = new Branch { Name = "Bob" }
};
name = resolver.Resolve(root); // Value of name = "Bob"

// Setting the value
resolver.Update(root, "Marie"); // Value of "root.Branch" = "Marie"
````

## IBindingResolver Chain

Implementations of [IBindingResolver](/src/Moryx/Bindings/IBindingResolver.cs) are build as a recursive double-linked list using their extended [IBindingResolverChain](/src/Moryx/Bindings/IBindingResolver.cs) interface. Each link of the chain resolves a fragment of the string using the source object and passes the result to the next link. When updating a value the resolver chain is executed up to the last link and instead of calling `Resolve` on the last link, `Update` is invoked.

The chain of resolvers is built by the [IBindingResolverFactory](/src/Moryx/Bindings/IBindingResolverFactory.cs) by parsing the string and creating links token by token. A token is text fragment between two dots or an index like __name__ in `Parameters[name]`. Unlike XAML-binding not all tokens directly represent a property. An implementation of [IBindingResolverFactory](/src/Moryx/Bindings/IBindingResolverFactory.cs) might define special keys or short-cuts.

The platform provides a base class [BindingResolverBase](/src/Moryx/Bindings/BindingResolverBase.cs) and four standard implementations of [IBindingResolver](/src/Moryx/Bindings/IBindingResolver.cs).

### BindingResolverBase

This base class implements the [IBindingResolverChain](/src/Moryx/Bindings/IBindingResolver.cs) interface and should be used instead of implementing the interface manually. It provides an explicit `Resolve`-method that continues invocation on the recursive chain. If the object is `null` or this was the last link in the chain it returns the current value.

A simple example for a custom resolver could resolve the type of an object.

````cs
// Resolver that fetches the type of the current object
public class TypeResolver : BindingResolverBase
{
    protected sealed override object Resolve(object source)
    {
        return source.GetType();
    }

    protected sealed override bool Update(object source, object value)
    {
        // We can not set the type of an object
        throw new NotImplementedException();
    }
}
````

### NullResolver

Following the [Null-Object pattern](https://en.wikipedia.org/wiki/Null_Object_pattern) this resolver can be used whenever an instance of [IBindingResolver](/src/Moryx/Bindings/IBindingResolver.cs) is required but no operation should be performed. The [NullResolver](/src/Moryx/Bindings/NullResolver.cs) simply continues the chain by calling proceed with the unmodified source object.

Example in code:

````cs
var resolver = new NullResolver();
var result = resolver.Resolve(10); // Value of result = 10
result = resolver.Resolve("Name"); // Value of result = "Name"
````

### ReflectionResolver

By using reflection [this resolver](/src/Moryx/Bindings/ReflectionResolver.cs) tries to load the value of the property from the source object. The name of the property is usually parsed as the text between dots in the binding string. The example at the top creates two reflection resolvers - one for `"Branch"` and one for `"Name"`. Because this resolver uses reflection it is significantly slower than the others and should not be used to resolve values which could be accessed another way. The resolver also supports updating the value as long as the property `CanWrite`.

Example in code:

````cs
var resolver = new ReflectionResolver("Length");
var result = resolver.Resolve("Name"); // Value of result = 4
````

Example for updating a value:

````cs
class Foo
{
    public string Name {get; set;}
}

var foo = new Foo();
IBindingResolver resolver = new ReflectionResolver("Name");
resolver.Update(foo, "Bob");
````

### DelegateResolver

The [delegate resolver](/src/Moryx/Bindings/DelegateResolver.cs) can be used to implement simple custom resolution rules without creating a new implementation of [IBindingResolver](/src/Moryx/Bindings/IBindingResolver.cs). It is created from a delegate `Func<object, object>` and will call it for each call to `Resolve()`. The result of the callback is returned.

Using the DelegateResolver to provide the `GetType()` behavior:

````cs
var resolver = new DelegateResolver(src => src.GetType());
var result = resolver.Resolve(10); // Value of result = "System.Int32"
result = resolver.Resolve("Name"); // Value of result = "System.String"
````

The DelegateResolver also supports the `Update`-method. To use this feature the second constructor needs to be used. Using the class `Foo` from our previous example:

````cs
var foo = new Foo();
var resolver = new DelegateResolver(src => ((Foo)src)?.Name, (src, value) => ((Foo)src).Name = (string)value);
resolver.Update(foo, "Bob");
````

### FormatBindingResolver

The [format resolver](/src/Moryx/Bindings/FormatBindingResolver.cs) can create format strings for objects implementing `IFormattable`. It is used by the `TextBindingResolverFactory`, that is explained further down in this documentation.

An easy example using the resolver to format a string with fixed length:

````cs
var resolver = new FormatBindingResolver("D3");
var result = resolver.Resolve(3); // Value of result = "003"
````

### IndexResolver

The [index resolver](/src/Moryx/Bindings/IndexResolver.cs) can extract values from collections and dictionaries. Given an index like `"Thomas"` or `10` it tries to interpret those values either as indexes in `IList` or as the key in a dictionary. Because the index resolver expects a collection as `source` object it usually needs to be preceded by another resolver that resolves the collection itself. This is done be the `BindingResolverFactory` that uses a Regex to combine a `ReflectionResolver` with an `IndexResolver` when it finds a string like `Collection[index]`. It also supports the `Update`-method to set values.

Using it stand-alone looks like this:

````cs
var resolver = new IndexResolver("1");
var list = new List<int>{ 10, 42, 100 };
var result = resolver.Resolve(list); // Value of result = 42
resolver.Update(list, 1337); // List = {10, 42, 1337}

resolver = new IndexResolver("Key");
var dict = new Dictionary
{
    ["Key"] = "Value"
};
result = resolver.Resolve(dict); // Value of result = "Value"
resolver.Update(dict, "OtherValue");
````

## IBindingResolverFactory

The resolver factory builds a recursive chain of `IBindingResolver` from a string like `"Root.Branch.Name"`. The default and base implementation is [BindingResolverFactory](/src/Moryx/Bindings/BindingResolverFactory.cs). It is similar to the binding-engine in XAML with the addition that it supports collection resolution. Custom resolver factories are built by creating a type derived from `BindingResolverFactory`. Below is an example for a factory that supports loading the objects type. The example explains the two concepts base key and custom resolution rules.

````cs
public class TypeResolverFactory : BindingResolverFactory
{
    protected override IBindingResolverChain CreateBaseResolver(string baseKey)
    {
        switch (baseKey)
        {
            case "Root":
                return new NullResolver();
            default:
                return base.CreateBaseResolver(baseKey);
        }
    }

    protected override IBindingResolverChain AddToChain(IBindingResolverChain resolver, string property)
    {
        // Custom rule for type
        if (property == "Type")
            return resolver.Extend(new TypeResolver()); // Or `resolver.Extend(new DelegateResolver(src => src.GetType()))`

        // In all other cases use the base logic
        return base.AddToChain(resolver, property);
    }
}
````

### Base Key

The resolver factory, unlike XAML, offers the possibility to define logic for different base keys. The base key is per definition the text before the first dot. Per default the resolver factory ignores the base key and directly starts with a `ReflectionResolver` on the source object. This means to access our name property the binding string needs to be `"Branch.Name"`. 

Base keys can be used for readability like the example for `"Root"` above or work as multiplexers with entry points into the object tree. Starting with a `NullResolver` can be more intuitive for users. Multiplexers make sense when creating a binding factory that always uses the same type of source object and this object is the entry point to a rather big object tree.

Let us assume the following object tree:

````sh
- Root
-- Branch
--- Name
--- EntryA
---- FooList
----- [0]
------ Name
----- [1]
------ Name
-- EntryB
--- Bla
---- Value
---- EntryC
----- LongString
````

If users would usually need values of one of the three entry points, they would have to build rather long binding strings like `"EntryB.Bla.EntryC.LongString"`. In such cases it makes sense to define base keys that work as short cuts and as a side effect also improve performance. For complex object structures this reduces the required knowledge of users about your design. As far as they are concerned these are four independent types.

````cs
protected override IBindingResolverChain CreateBaseResolver(string baseKey)
{
    switch (baseKey)
    {
        case "Root":
            return new NullResolver();
        case "EntryA":
            return new DelegateResolver(src => ((Root)src).Branch.EntryA);
        case "EntryB":
            return new DelegateResolver(src => ((Root)src).EntryB);
        case "EntryC":
            return new DelegateResolver(src => ((Root)src).EntryB.Bla.EntryC);
        default:
            return base.CreateBaseResolver(baseKey);
    }
}
````

### Add To Chain

As previously mentioned the binding resolvers are built as a recursive chain and the `AddToChain`-method is used to do exactly this. It is called for each segment in the binding string between dots. Per default the factory applies a property and indexer regex to this string. Derived types can use this method to include their own resolvers and logic into the resolver chain. 

The new resolvers must be added to the current resolver passed into the method and then return the new end of the chain. The extension method `Extend` does exactly this and is the recommended way to do it in new binding resolver factories. The new end of the chain need to be returned because in some cases the `AddToChain` method might add more than one link to the chain. For example when working with collections, resolving `Collection[Foo]` will add a `ReflectionResolver` and an `IndexResolver` to the chain.

## Illustrated Examples

Applying the previous sections to a few examples the factory and resolver chain can be illustrated with the images below.

![](images/Bindings/Example1.png)
![](images/Bindings/Example2.png)
![](images/Bindings/Example3.png)

## Text Bindings

While there might be some use cases of those bindings by themselves, the more common use is to embed them into text. This is done by instances of [ITextBindingResolver](/src/Moryx/Bindings/ITextBindingResolver.cs) created by the [TextBindingResolverFactory](/src/Moryx/Bindings/TextBindingResolverFactory.cs). The text binding resolver factory is static and builds text resolvers using the given [IBindingResolverFactory](/src/Moryx/Bindings/IBindingResolverFactory.cs). This means all custom bindings can be used within text without any additional effort.

The text resolution also supports formatting by appending `":<format>"` to the binding expression. It supports the default formats of the type it is applied to. This features is implemented by appending a [FormatBindingResolver](/src/Moryx/Bindings/FormatBindingResolver.cs) to the end of the chain.

Using the previous examples using the text resolvers looks like this.

````cs
var text = "For {Root.Branch.Name} the type of 'EntryA' is {EntryA.Type.FullName}".
var resolver = TextBindingResolverFactory.Create(text, new TypeResolverFactory());

// Default EntryA
var root = new Root
{
    Branch = new Branch
    {
        Name = "Foo",
        EntryA = new EntryA()
    }
};
var resolved = resolver.Resolve(root); // Value of 'resolved': "For Foo the type of 'EntryA' is Namespace.EntryA"

// Derived type
root.Branch.EntryA = new DerivedEntryA();
var resolved = resolver.Resolve(root); // Value of 'resolved': "For Foo the type of 'EntryA' is Other.Namespace.DerivedEntryA"

// Using a format
text = "The value '{Root.Branch.Name}' has {Root.Branch.Name.Length:D2} letters!".
resolver = TextBindingResolverFactory.Create(text, new BindingResolverFactory());

resolved = resolver.Resolve(root);     // Value of 'resolved': "The value 'Foo' has 03 letters!"
````