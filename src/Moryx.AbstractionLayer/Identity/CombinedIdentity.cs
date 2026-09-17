// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Identity;

/// <summary>
/// Aggregates multiple identities belonging to the same physical object into a single <see cref="IIdentity"/>.
/// </summary>
/// <remarks>
/// Will always contain at least two instances of <see cref="IIdentity"/> and provide a concatinated <see cref="Identifier"/>,
/// canonically ordered by implementation type name first, then alphabetically by identifier value.
/// Use <see cref="From(IEnumerable{IIdentity})"/> to create instances.
/// </remarks>
public class CombinedIdentity : IIdentity
{
    private readonly List<IIdentity> _identities;
    private static readonly Comparison<IIdentity> _canonicalOrder = (x, y) =>
    {
        var typeComparison = string.Compare(x.GetType().FullName, y.GetType().FullName, StringComparison.Ordinal);
        return typeComparison != 0 ? typeComparison : string.Compare(x.Identifier, y.Identifier, StringComparison.Ordinal);
    };

    private CombinedIdentity(List<IIdentity> flat)
    {
        flat.Sort(_canonicalOrder);
        _identities = flat;
        Identifier = string.Join(" | ", flat.Select(i => i.Identifier));
    }

    /// <summary>
    /// Creates an <see cref="IIdentity"/> from the given <paramref name="identities"/>
    /// </summary>
    /// <remarks>
    /// * Returns <see cref="NullIdentity.Instance"/> when the input is null, empty, or contains only
    /// <see cref="NullIdentity"/> entries.
    /// * In case <paramref name="identities"/> contains a single instance of <see cref="IIdentity"/> the instance is returned directly.
    /// * Returns a <see cref="CombinedIdentity"/> in any other case.
    /// * Nested <see cref="CombinedIdentity"/> instances are flattened.
    /// * Combined <see cref="IIdentity"/>s are canonically ordered by implementation type name first, then alphabetically by identifier value.
    /// </remarks>
    public static IIdentity From(IEnumerable<IIdentity> identities)
    {
        if (identities == null || !identities.Any())
        {
            return NullIdentity.Instance;
        }

        var flat = new List<IIdentity>();
        foreach (var identity in identities)
        {
            if (identity is null or NullIdentity)
            {
                continue;
            }
            else if (identity is CombinedIdentity combined)
            {
                flat.AddRange(combined._identities);
            }
            else
            {
                flat.Add(identity);
            }
        }

        return flat.Count switch
        {
            0 => NullIdentity.Instance,
            1 => flat[0],
            _ => new CombinedIdentity(flat)
        };
    }

    /// <inheritdoc cref="From(IEnumerable{IIdentity})"/>
    public static IIdentity From(params IIdentity[] identities) => From((IEnumerable<IIdentity>)identities);

    /// <inheritdoc />
    public string Identifier { get; }

    /// <inheritdoc />
    public void SetIdentifier(string identifier) =>
        throw new InvalidOperationException($"Cannot set identifier on {nameof(CombinedIdentity)}");

    /// <summary>
    /// The individual identities that together identify the same physical object, in canonical order.
    /// </summary>
    public IReadOnlyList<IIdentity> GetAll() => _identities;

    /// <summary>
    /// Checks whether the given <paramref name="candidate"/> can identify this object,
    /// i.e., all of this' identities are present in the candicate (<code>this</code> ⊆ <paramref name="candidate"/>).
    /// </summary>
    public virtual bool MatchedBy(IIdentity candidate) => candidate is CombinedIdentity combinedCandidate
        ? combinedCandidate.Matches(this) : EqualsSingleIdentity(candidate);

    private bool EqualsSingleIdentity(IIdentity other) => _identities.Count == 1 && _identities.Single().Equals(other);

    /// <summary>
    /// Checks whether this' combined identities satisfy the given <paramref name="required"/> required,
    /// i.e., all required identities are present in this (<code>this</code> ⊇ <paramref name="required"/>).
    /// </summary>
    public virtual bool Matches(IIdentity required)
    {
        IEnumerable<IIdentity> toFind = required is CombinedIdentity c ? c._identities : [required];
        return toFind.All(tf => _identities.Any(id => id.Equals(tf)));
    }

    /// <inheritdoc />
    public bool Equals(IIdentity other) => EqualsSingleIdentity(other) || EqualsCombinedIdentity(other);

    private bool EqualsCombinedIdentity(IIdentity other) => other is CombinedIdentity combined
        && _identities.Count == combined._identities.Count
        && _identities.Zip(combined._identities).All(pair => pair.First.Equals(pair.Second));

    /// <inheritdoc />
    public override bool Equals(object obj) => Equals(obj as IIdentity);

    /// <inheritdoc />
    public override int GetHashCode() => Identifier.GetHashCode();
}
