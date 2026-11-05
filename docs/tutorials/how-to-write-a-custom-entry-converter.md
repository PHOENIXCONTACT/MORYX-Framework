---
uid: HowToWriteACustomEntryConverter
---
# How to write a custom EntryConverter

> [!NOTE]
> This tutorial builds on familiarity with `EntryConvert` and the `ICustomSerialization` interface.
> If you are not yet familiar with those, read [EntryConvert](~/articles/framework/Serialization/entry-convert.md) first.

An [EntryConverter](~/articles/framework/Serialization/entry-convert.md#entryconverter) lets you replace the built-in encode/decode logic for a specific type or member with your own. Use one when the default mapping does not match the representation you need in the entry tree — for example when a type should appear as a compact string rather than as a class entry with sub-entries.

## When to use a custom converter

Consider writing a converter when:

- A type has no public parameter-less constructor, is an interface, or an abstract type and therefore cannot be decoded by the default logic (like [IIdentity](/src/Moryx.AbstractionLayer/Identity/IIdentity.cs) for which we provide the [](/src/Moryx.AbstractionLayer/Identity/)).
- The natural representation of a type in the entry tree differs from what the default reflection-based encoding produces.
- You want to encode a complex object as a single string value for compact transfer or display.

## Implement the converter

Derive from `EntryConverter` and implement the three abstract members:

```cs
public class ColorEntryConverter : EntryConverter
{
    // Tell EntryConvert which member types this converter handles
    public override bool CanConvert(Type memberType) => memberType == typeof(Color);

    // Encode the value into an Entry. Return only Value, SubEntries and Prototypes;
    // the rest (identifier, display name, ...) is filled by EntryConvert from the member.
    public override Entry Encode(object value, EntryConverterContext context)
    {
        var color = value as Color? ?? Color.Empty;
        return new Entry
        {
            Value = new EntryValue
            {
                Type = EntryValueType.String,
                Current = ColorTranslator.ToHtml(color),
                Default = "#000000"
            }
        };
    }

    // Decode the Entry back into a value to assign to the member
    public override object Decode(Entry entry, object currentValue, EntryConverterContext context)
    {
        var html = entry.Value.Current;
        return string.IsNullOrEmpty(html) ? Color.Empty : ColorTranslator.FromHtml(html);
    }
}
```

> [!IMPORTANT]
> Converter instances are **cached and shared** across all conversions. The converter must be **stateless** — do not store per-call state in fields. It also requires a **public parameterless constructor**.

## Apply the converter to a type

Decorating a class with `[EntrySerialize(ConverterType = ...)]` applies the converter to every property of that type, in every class that contains it:

```cs
[EntrySerialize(ConverterType = typeof(ColorEntryConverter))]
public class Color { ... }

public class Theme
{
    public Color Background { get; set; }  // uses ColorEntryConverter automatically
    public Color Foreground { get; set; }  // uses ColorEntryConverter automatically
}
```

This is the least intrusive option when the converter should be the default for the type everywhere.

## Apply the converter to a single property

Decorating a property with `[EntrySerialize(ConverterType = ...)]` applies the converter only to that member. A property-level attribute takes precedence over a type-level attribute:

```cs
public class Theme
{
    // Different encoding only for this property
    [EntrySerialize(ConverterType = typeof(RgbColorConverter))]
    public Color Background { get; set; }

    public Color Foreground { get; set; }
}
```

The same syntax works on method parameters:

```cs
public void SetBackground([EntrySerialize(ConverterType = typeof(RgbColorConverter))] Color color)
{
    ...
}
```

## Convert the items of a collection

Setting `ConverterTarget = EntryConverterTarget.Items` applies the converter to each item in a collection rather than to the whole collection:

```cs
public class Palette
{
    [EntrySerialize(ConverterType = typeof(ColorEntryConverter), ConverterTarget = EntryConverterTarget.Items)]
    public List<Color> Colors { get; set; }
}
```

`EntryConvert` still manages the collection structure — adding and removing items. The prototypes for new items are derived from the converter result for an empty item (`null` passed to `Encode`). Make sure your `Encode` method returns a sensible entry for `null`:

```cs
public override Entry Encode(object value, EntryConverterContext context)
{
    var color = value as Color? ?? Color.Empty;  // handles null gracefully
    return new Entry { Value = new EntryValue { Type = EntryValueType.String, Current = ColorTranslator.ToHtml(color) } };
}
```

## Use context in the converter

The `EntryConverterContext` exposes additional information that you can use inside the converter:

```cs
public override Entry Encode(object value, EntryConverterContext context)
{
    // Use the format provider from the active serialization for culture-aware formatting
    var str = Convert.ToString(value, context.Serialization.FormatProvider);

    // Read an attribute from the property to adjust encoding
    var unit = context.AttributeProvider.GetCustomAttribute<UnitAttribute>();

    // Access the containing object instance (null when encoding a type without an instance)
    if (context.Instance is IColorProvider provider)
    {
        ...
    }

    return new Entry { Value = new EntryValue { Type = EntryValueType.String, Current = str } };
}
```

## Override converter resolution in a custom serialization

`ICustomSerialization.GetConverter` and `GetItemConverter` are called by `EntryConvert` to resolve the converter for every member. Override them in a custom serialization to suppress or redirect converters without changing the members themselves:

```cs
public class NoColorConverterSerialization : DefaultSerialization
{
    // Return null to fall back to the default encoding for Color members
    public override EntryConverter GetConverter(Type memberType, ICustomAttributeProvider attributeProvider)
        => memberType == typeof(Color) ? null : base.GetConverter(memberType, attributeProvider);
}
```

## Troubleshooting

| Symptom | Cause |
|---------|-------|
| `InvalidOperationException`: converter cannot convert members of type X | `CanConvert` returned `false` for the member type, but the converter was explicitly declared for it via `ConverterType`. |
| `InvalidOperationException`: no public parameterless constructor | The converter class has only parameterized constructors. Add `public MyConverter() { }`. |
| `InvalidOperationException`: `ConverterTarget.Items` requires a `ConverterType` | `ConverterTarget = EntryConverterTarget.Items` was set without specifying `ConverterType`. |
| Converter is ignored | The active `ICustomSerialization` overrides `GetConverter` and returns `null`. |
| Collection items are encoded as class entries instead of using the converter | The converter was applied to the collection property itself (`ConverterTarget = Member`, the default) instead of to its items (`ConverterTarget = Items`). |
