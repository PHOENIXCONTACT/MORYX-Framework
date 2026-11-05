// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Moryx.Configuration;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.AbstractionLayer.Identity;

/// <summary>
/// <see cref="EntryConverter"/> for members of type <see cref="IIdentity"/>.
/// Identities are encoded as a class entry with the identity type name as current value and the
/// arguments of their constructor as sub-entries. All public identity types assignable to the member,
/// which provide a public constructor with primitive or string parameters, are offered as prototypes.
/// </summary>
public sealed class IdentityEntryConverter : EntryConverter
{
    /// <summary>
    /// Identity types per member type, keyed by type name
    /// </summary>
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, Type>> _candidates = new();

    /// <inheritdoc />
    public override bool CanConvert(Type memberType) => typeof(IIdentity).IsAssignableFrom(memberType);

    /// <inheritdoc />
    public override Entry Encode(object value, EntryConverterContext context)
    {
        var candidates = GetCandidates(context.MemberType);
        var possible = candidates.Values.Select(t => t.ToEntryPossible()).ToArray();
        var identityType = value?.GetType() ?? typeof(NullIdentity);

        var encoded = EncodeIdentity(identityType, context.Serialization, value as IIdentity);
        encoded.Value.Possible = possible;
        // Identities without suitable constructor can not be edited
        encoded.Value.IsReadOnly = !candidates.ContainsKey(identityType.FullName);

        foreach (var candidate in candidates.Values)
        {
            var prototype = EncodeIdentity(candidate, context.Serialization);
            prototype.Identifier = candidate.FullName;
            prototype.DisplayName = candidate.GetDisplayName() ?? candidate.Name;
            prototype.Description = candidate.GetDescription();
            prototype.Value.Possible = possible;
            encoded.Prototypes.Add(prototype);
        }

        return encoded;
    }

    /// <summary>
    /// Get all identity types assignable to the member type
    /// </summary>
    private static IReadOnlyDictionary<string, Type> GetCandidates(Type memberType)
    {
        return _candidates.GetOrAdd(memberType, type =>
        {
            var candidates = ReflectionTool.GetPublicClasses(type, t => !t.IsAbstract && SelectConstructor(t) != null)
                .DistinctBy(t => t.FullName).ToDictionary(t => t.FullName);

            if (type.IsAssignableFrom(typeof(NullIdentity)))
            {
                candidates[typeof(NullIdentity).FullName] = typeof(NullIdentity);
            }

            return candidates;
        });
    }

    /// <summary>
    /// Select the public constructor with the most parameters, which only takes primitives or strings
    /// </summary>
    private static ConstructorInfo SelectConstructor(Type identityType) => identityType.GetConstructors()
        .Where(c => c.GetParameters().All(p => EntryConvert.ValueOrStringType(p.ParameterType)))
        .OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

    /// <summary>
    /// Encode an identity type with the constructor arguments as sub-entries
    /// </summary>
    private static Entry EncodeIdentity(Type identityType, ICustomSerialization serialization, IIdentity identity = default)
    {
        var encoded = new Entry
        {
            Value = new EntryValue
            {
                Type = EntryValueType.Class,
                Current = identityType.FullName,
                Default = identityType.FullName
            }
        };

        var constructor = SelectConstructor(identityType);
        if (constructor == null)
        {
            return encoded;
        }

        var parameters = constructor.GetParameters();
        var arguments = EntryConvert.EncodeMethod(constructor, serialization).Parameters.SubEntries;
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var argument = arguments[i];

            // Read current arguments from the properties matching the parameter names
            var property = identity?.GetType().GetProperty(parameter.Name!, BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var argumentValue = property != null ? property.GetValue(identity)
                : parameter.ParameterType.IsValueType ? Activator.CreateInstance(parameter.ParameterType) : null;

            argument.Value.Current = argumentValue switch
            {
                null => argument.Value.Current,
                IConvertible convertible => convertible.ToString(serialization.FormatProvider ?? CultureInfo.CurrentCulture),
                _ => argumentValue.ToString()
            };
        }

        encoded.SubEntries = arguments;
        return encoded;
    }

    /// <inheritdoc />
    public override object Decode(Entry entry, object currentValue, EntryConverterContext context)
    {
        var key = entry.Value.Current ?? string.Empty;
        if (!GetCandidates(context.MemberType).TryGetValue(key, out var identityType))
        {
            return currentValue?.GetType().FullName == key ? currentValue
                : throw new ArgumentException($"Invalid identity type {key} for entry {entry.DisplayName ?? entry.Identifier}.");
        }

        if (identityType == typeof(NullIdentity))
        {
            return NullIdentity.Instance;
        }

        var constructor = SelectConstructor(identityType);
        var encodedConstructor = new MethodEntry
        {
            Name = constructor.Name,
            IsConstructor = true,
            Parameters = new Entry { SubEntries = entry.SubEntries }
        };
        return EntryConvert.CreateInstance(identityType, encodedConstructor, context.Serialization);
    }
}
