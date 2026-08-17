// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Products.Management.Model;
using Moryx.Serialization;
using Moryx.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Moryx.Products.Management.ProductExpressionHelpers;

namespace Moryx.Products.Management;

/// <summary>
/// Reusable component to map business objects onto entities of type
/// <see cref="IGenericColumns"/>
/// </summary>
[Component(LifeCycle.Transient, typeof(GenericEntityMapper<,>))]
internal class GenericEntityMapper<TBase, TReference> : IGenericMapper
    where TReference : class
{
    /// <summary>
    /// Injected factory for property mappers
    /// </summary>
    public IPropertyMapperFactory MapperFactory { get; set; }

    public ILogger Logger { get; set; }

    private IPropertyMapper[] _configuredMappers;


    private JsonSerializerSettings _jsonSettings;

    private IPropertyAccessor<IGenericColumns, string> _jsonAccessor;

    public void Initialize(Type concreteType, IGenericMapperConfiguration config)
    {
        // Get JSON accessor
        var jsonColumn = typeof(IGenericColumns).GetProperty(config.JsonColumn);
        _jsonAccessor = ReflectionTool.PropertyAccessor<IGenericColumns, string>(jsonColumn);

        var baseProperties = typeof(TBase).GetProperties().Select(p => p.Name).ToArray();

        // Legacy compatibility:
        // Historically some configurations used the JsonColumn both as JSON storage
        // and as a regular property mapping. This remains supported until the next
        // major release but should no longer be generated automatically.
        // TODO: MORYX 12: Maybe remove support for using JsonColumn as a regular property mapping.
        if (config.PropertyConfigs.Any(pc => string.Equals(pc.PropertyName, config.JsonColumn, StringComparison.OrdinalIgnoreCase)))
        {
            Logger.LogWarning(
                "Detected a PropertyConfig for JsonColumn '{JsonColumn}'. " +
                "This configuration is deprecated and may no longer be supported in a future major release.",
                config.JsonColumn);
        }

        var configuredProperties = config.PropertyConfigs
            .Select(cm => cm.PropertyName)
            .ToArray();

        var readOnlyProperties = concreteType.GetProperties()
            .Where(p => p.GetSetMethod() == null)
            .Select(p => p.Name)
            .ToArray();

        var jsonIgnoredProperties = baseProperties
            .Concat(configuredProperties)
            .Concat(readOnlyProperties)
            .ToArray();

        _jsonSettings = JsonSettings.Minimal
            .Overwrite(j => j.ContractResolver =
                new DifferentialContractResolver<TReference>(jsonIgnoredProperties));

        // Properties where no mapper should be created for: base and read only properties
        var mapperIgnoredProperties = baseProperties
            .Concat(readOnlyProperties)
            .ToArray();

        _configuredMappers = config.PropertyConfigs
            .Where(pc => !mapperIgnoredProperties.Contains(pc.PropertyName))
            .Select(pc => MapperFactory.Create(pc, concreteType))
            .ToArray();
    }

    public bool HasChanged(IGenericColumns storage, object instance)
    {
        // Compare JSON and mappers to entity
        var json = JsonConvert.SerializeObject(instance, _jsonSettings);
        return _jsonAccessor.ReadProperty(storage) != json || _configuredMappers.Any(m => m.HasChanged(storage, instance));
    }

    public Expression<Func<IGenericColumns, bool>> TransformSelector<T>(Expression<Func<T, bool>> selector)
    {
        var body = selector.Body;
        // Extract the property targeted by the expression
        switch (body)
        {
            case MemberExpression memEx:
                // For single member expression assume
                return Convert(memEx.Member.Name, ExpressionType.Equal, true);
            case BinaryExpression binary:
                object value;
                if (binary.Left is MemberExpression bLeft && bLeft.Expression is ParameterExpression)
                {
                    value = ExtractExpressionValue(binary.Right);
                    return Convert(bLeft.Member.Name, binary.NodeType, value);
                }
                if (binary.Right is MemberExpression bRight && bRight.Expression is ParameterExpression)
                {
                    value = ExtractExpressionValue(binary.Left);
                    return Convert(bRight.Member.Name, binary.NodeType, value);
                }
                break;
            case MethodCallExpression call:
                // For now only implement identity check
                var method = call.Method;
                if (method.Name == nameof(Equals))
                {
                    object callValue;
                    if (call.Object is MemberExpression callMemEx && callMemEx.Expression is ConstantExpression)
                    {
                        callValue = ExtractExpressionValue(call.Object);
                        return Convert(((MemberExpression)call.Arguments.First()).Member.Name, ExpressionType.Equal, callValue);
                    }

                    callValue = ExtractExpressionValue(call.Arguments.First());
                    return Convert(((MemberExpression)call.Object).Member.Name, ExpressionType.Equal, callValue);
                }
                break;
        }
        throw new NotSupportedException("Expression type not supported yet");
    }

    private Expression<Func<IGenericColumns, bool>> Convert(string memberName, ExpressionType type, object value)
    {
        var mapper = _configuredMappers.FirstOrDefault(cm => cm.Property.Name == memberName);

        var columnParam = Expression.Parameter(typeof(IGenericColumns));
        var body = mapper.ToColumnExpression(columnParam, type, value);
        return Expression.Lambda(body, columnParam) as Expression<Func<IGenericColumns, bool>>;
    }

    /// <inheritdoc />
    public void ReadValue(IGenericColumns source, object target)
    {
        // Read all directly mapped properties
        foreach (var mapper in _configuredMappers)
        {
            mapper.ReadValue(source, target);
        }

        // Reading JSON from the JsonColumn
        var json = _jsonAccessor.ReadProperty(source);
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        // Legacy compatibility:
        // Older systems may contain plain text in the JsonColumn.
        // In this case simply ignore the content and continue loading
        // the directly mapped properties.
        if (!(json.StartsWith("{") || json.StartsWith("[")))
        {
            Logger.LogWarning("Ignoring non-JSON content in JsonColumn '{Column}'. Value: '{Value}'", _jsonAccessor.Property.Name, json);
            return;
        }

        // Populate remaining properties from the JsonColumn JSON payload
        try
        {
            JsonConvert.PopulateObject(json, target, _jsonSettings);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize JSON from column '{_jsonAccessor.Property.Name}' for target type '{target.GetType().FullName}'. " +
                $"Stored value: '{json}'", ex);
        }
    }

    /// <inheritdoc/>>
    public void WriteValue(object source, IGenericColumns target)
    {
        // First, write all the directly mapped properties
        foreach (var mapper in _configuredMappers)
        {
            mapper.WriteValue(source, target);
        }

        // Generate JSON from the rest of the object
        var mappedNames = _configuredMappers
            .Select(m => m.Property.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // JSON for all other props
        var jsonObject = new JObject();

        foreach (var prop in source.GetType().GetProperties())
        {
            if (!prop.CanRead)
            {
                continue;
            }

            if (!prop.CanWrite)
            {
                continue;
            }

            // Do not include base, read-only, or directly mapped properties in the JSON
            if (mappedNames.Contains(prop.Name))
            {
                continue;
            }

            // jump Indexer 
            if (prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            // jump Read-only Props
            if (prop.GetSetMethod() == null)
            {
                continue;
            }

            var value = prop.GetValue(source);

            // "Not set yet" -> null in JSON
            if (IsDefaultOrNull(prop.PropertyType, value))
            {
                jsonObject[prop.Name] = JValue.CreateNull();
            }
            else
            {
                jsonObject[prop.Name] = JToken.FromObject(value, JsonSerializer.Create(_jsonSettings));
            }
        }

        // Write JSON to the Json - Column
        _jsonAccessor.WriteProperty(target, jsonObject.ToString(Formatting.None));
    }

    private static bool IsDefaultOrNull(Type propertyType, object value)
    {
        if (value == null)
        {
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        var effectiveType = underlyingType ?? propertyType;

        // Reference type
        if (!effectiveType.IsValueType)
        {
            return false;
        }

        // Value type: handle Default as "not set yet"
        var defaultValue = Activator.CreateInstance(effectiveType);
        return Equals(value, defaultValue);
    }
}
