---
uid: Identity
---
# Identity

[Identities](/src/Moryx.AbstractionLayer/Identity/IIdentity.cs) represent unique properties of physical objects and production instances — serial numbers, MAC addresses, batch numbers, QR codes, and similar.
Each identity is defined by its implementing type, an `Identifier` string (and possibly typed properties of the specific identity type); both together establish uniqueness within the system.

Any domain object that can be identified implements [IIdentifiableObject](/src/Moryx.AbstractionLayer/Identity/IIdentifiableObject.cs), which exposes a single `Identity` property:

## Single Identity

Implement `IIdentity` directly to define a custom identity:

````cs
public class SerialNumberIdentity : IIdentity
{
    public string Identifier { get; private set; }

    public SerialNumberIdentity(string serialNumber)
    {
        Identifier = serialNumber;
    }

    public void SetIdentifier(string identifier) => Identifier = identifier;

    public bool Equals(IIdentity other) =>
        other is SerialNumberIdentity s && s.Identifier == Identifier;
}
````

For identities backed by numeric values, the abstract base class [NumberIdentity](/src/Moryx.AbstractionLayer/Identity/NumberIdentity.cs) reduces boilerplate. A `NumberType` discriminator distinguishes between identity kinds that both express as a number, for example serial vs. MAC address.

[BatchIdentity](/src/Moryx.AbstractionLayer/Identity/BatchIdentity.cs) is a ready-made `IIdentity` for batch-tracked `ProductInstance`s.

## No Identity

[NullIdentity](/src/Moryx.AbstractionLayer/Identity/NullIdentity.cs) is the null object for `IIdentity`. Use `NullIdentity.Instance` to represent a non-identifiable object instead of `null`:

````cs
resource.Identity = NullIdentity.Instance;
````

## Combined Identity

A physical object may carry more than one identifying property simultaneously — for example both a serial number and a QR code label. [CombinedIdentity](/src/Moryx.AbstractionLayer/Identity/CombinedIdentity.cs) aggregates multiple `IIdentity` instances into one while still implementing `IIdentity`, so it can be assigned directly to `IIdentifiableObject.Identity`:

````cs
resource.Identity = CombinedIdentity.From(
    new SerialNumberIdentity("SN-0042"),
    new QrCodeIdentity("QR-20250101-7F3A"));
````

`CombinedIdentity.From` is the intended factory method, flatening nested `CombinedIdentity` instances and filtering `NullIdentity` entries.
Passing null, an empty collection, or a collection that reduces to nothing returns `NullIdentity.Instance` not creating a `CombinedIdentity`.
Similarly, passing a single non-null identity returns that identity directly without wrapping.

The constituent identities are stored in canonical order (by implementation type name, then alphabetically by identifier value).

### Checking identity coverage

Two methods express set-membership queries across combined identities, mirroring the `Provides`/`ProvidedBy` pattern of capabilities:

- **`Matches(required)`** — answers *"does this object satisfy the required identity?"* Returns `true` when all identities in `required` are present in `this`.
- **`MatchedBy(candidate)`** — the inverse: answers *"is this object fully covered by the candidate?"* Returns `true` when all of `this`'s identities are present in `candidate`.

They are proper inverses: `a.Matches(b) ↔ b.MatchedBy(a)`.

````cs
IIdentity required = new SerialNumberIdentity("SN-0042");
IIdentity provided = CombinedIdentity.From(
    new SerialNumberIdentity("SN-0042"),
    new QrCodeIdentity("QR-20250101-7F3A"));

provided.Matches(required);    // true — provided contains the required serial
required.MatchedBy(provided);  // true — provided covers all of required
````

## Extension Methods

[IdentityExtensions](/src/Moryx.AbstractionLayer/Identity/IdentityExtensions.cs) brings `Combine`, `GetAll`, `Matches`, and `MatchedBy` to any `IIdentity` variable, including plain single identities:

````cs
IIdentity serial = new SerialNumberIdentity("SN-0042");
IIdentity qr     = new QrCodeIdentity("QR-20250101-7F3A");

// Build a combined identity from any starting point
IIdentity combined = serial.Combine(qr);

// Retrieve all constituent identities regardless of type
IReadOnlyList<IIdentity> all = combined.GetAll(); // works on singles too → [identity]

// Set-membership queries through the IIdentity interface
combined.Matches(serial);    // true
serial.MatchedBy(combined);  // true
````

`NullIdentity` participates with consistent semantics: `NullIdentity.Matches(nonNull)` is `false`; `any.Matches(NullIdentity)` is `true` (requiring nothing is satisfied by anything); `NullIdentity.MatchedBy(any)` is `true`; `any.MatchedBy(NullIdentity)` is `false` (absence covers no real requirement).
