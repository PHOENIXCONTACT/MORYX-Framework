---
uid: HowToWriteACustomEntryEncoder
---
# How to write a custom EntryEncoderBase

> [!NOTE]
> This tutorial builds on familiarity with `EntryConvert` and the `ICustomSerialization` interface.
> If you are not yet familiar with those, read [EntryConvert](~/articles/framework/Serialization/entry-convert.md) first.

An implementation of [EntryEncoderBase](~/articles/framework/Serialization/entry-convert.md#entryencoderbase) lets you replace the built-in encode/decode logic for a specific type or member with your own. Use one when the default mapping does not match the representation you need in the entry tree — for example when a type should appear as a compact string rather than as a class entry with sub-entries.

## When to use a custom encoder

Consider writing a encoder when:

- A type has no public parameterless constructor, is an interface, or an abstract type and therefore cannot be decoded by the default logic (like [IIdentity](/src/Moryx.AbstractionLayer/Identity/IIdentity.cs) for which we provide the [](/src/Moryx.AbstractionLayer/Identity/)).
- The natural representation of a type in the entry tree differs from what the default reflection-based encoding produces.
- You want to encode a complex object as a single string value for compact transfer or display.

## Implement the encoder

Derive from `EntryEncoderBase` and implement the three abstract members:

```cs
public class ColorEntryEncoder : EntryEncoderBase
{
    // Tell EntryConvert which member types this encoder handles
    public override bool CanEncode(Type memberType) => memberType == typeof(Color);

    // Encode the value into an Entry. Return only Value, SubEntries and Prototypes;
    // the rest (identifier, display name, ...) is filled by EntryConvert from the member.
    public override Entry Encode(object value, EntryEncoderContext context)
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
    public override object Decode(Entry entry, object currentValue, EntryEncoderContext context)
    {
        var html = entry.Value.Current;
        return string.IsNullOrEmpty(html) ? Color.Empty : ColorTranslator.FromHtml(html);
    }
}
```

> [!IMPORTANT]
> Encoder instances are **cached and shared** across all conversions. The encoder must be **stateless** — do not store per-call state in fields. It also requires a **public parameterless constructor**.

## Apply the encoder to a type

Decorating a class with `[EntrySerialize(EncoderType = ...)]` applies the encoder to every property of that type, in every class that contains it:

```cs
[EntrySerialize(EncoderType = typeof(ColorEntryEncoder))]
public class Color { ... }

public class Theme
{
    public Color Background { get; set; }  // uses ColorEntryEncoder automatically
    public Color Foreground { get; set; }  // uses ColorEntryEncoder automatically
}
```

This is the least intrusive option when the encoder should be the default for the type everywhere.

## Apply the encoder to a single property

Decorating a property with `[EntrySerialize(EncoderType = ...)]` applies the encoder only to that member. A property-level attribute takes precedence over a type-level attribute:

```cs
public class Theme
{
    // Different encoding only for this property
    [EntrySerialize(EncoderType = typeof(RgbColorEncoder))]
    public Color Background { get; set; }

    public Color Foreground { get; set; }
}
```

The same syntax works on method parameters:

```cs
public void SetBackground([EntrySerialize(EncoderType = typeof(RgbColorEncoder))] Color color)
{
    ...
}
```

## Convert the items of a collection

Setting `EncoderTarget = EntryEncoderTarget.Items` applies the encoder to each item in a collection rather than to the whole collection:

```cs
public class Palette
{
    [EntrySerialize(EncoderType = typeof(ColorEntryEncoder), EncoderTarget = EntryEncoderTarget.Items)]
    public List<Color> Colors { get; set; }
}
```

`EntryConvert` still manages the collection structure — adding and removing items. The prototypes for new items are derived from the encoder result for an empty item (`null` passed to `Encode`). Make sure your `Encode` method returns a sensible entry for `null`:

```cs
public override Entry Encode(object value, EntryEncoderContext context)
{
    var color = value as Color? ?? Color.Empty;  // handles null gracefully
    return new Entry { Value = new EntryValue { Type = EntryValueType.String, Current = ColorTranslator.ToHtml(color) } };
}
```

## Use context in the encoder

The `EntryEncoderContext` exposes additional information that you can use inside the encoder:

```cs
public override Entry Encode(object value, EntryEncoderContext context)
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

## Override encoder resolution in a custom serialization

`ICustomSerialization.GetEncoder` and `GetItemEncoder` are called by `EntryConvert` to resolve the encoder for every member. Override them in a custom serialization to suppress or redirect encoders without changing the members themselves:

```cs
public class NoColorEncoderSerialization : DefaultSerialization
{
    // Return null to fall back to the default encoding for Color members
    public override EntryEncoderBase GetEncoder(Type memberType, ICustomAttributeProvider attributeProvider)
        => memberType == typeof(Color) ? null : base.GetEncoder(memberType, attributeProvider);
}
```

## Troubleshooting

| Symptom | Cause |
|---------|-------|
| `InvalidOperationException`: encoder cannot convert members of type X | `CanEncode` returned `false` for the member type, but the encoder was explicitly declared for it via `EncoderType`. |
| `InvalidOperationException`: no public parameterless constructor | The encoder class has only parameterized constructors. Add `public MyEncoder() { }`. |
| `InvalidOperationException`: `EncoderTarget.Items` requires a `EncoderType` | `EncoderTarget = EntryEncoderTarget.Items` was set without specifying `EncoderType`. |
| Encoder is ignored | The active `ICustomSerialization` overrides `GetEncoder` and returns `null`. |
| Collection items are encoded as class entries instead of using the encoder | The encoder was applied to the collection property itself (`EncoderTarget = Member`, the default) instead of to its items (`EncoderTarget = Items`). |
