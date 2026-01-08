// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Moryx.Container;
using Moryx.Products.Management.Model;
using Moryx.Serialization;
using Moryx.Tools;
using Newtonsoft.Json;
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

    private IPropertyMapper[] _configuredMappers;

    private JsonSerializerSettings _jsonSettings;

    private IPropertyAccessor<IGenericColumns, string> _jsonAccessor;

    public void Initialize(Type concreteType, IGenericMapperConfiguration config)
    {
        // Get JSON accessor
        var jsonColumn = typeof(IGenericColumns).GetProperty(config.JsonColumn);
        _jsonAccessor = ReflectionTool.PropertyAccessor<IGenericColumns, string>(jsonColumn);

        var baseProperties = typeof(TBase).GetProperties().Select(p => p.Name).ToArray();
        var configuredProperties = config.PropertyConfigs.Select(cm => cm.PropertyName);

        var readOnlyProperties = concreteType.GetProperties()
            .Where(p => p.GetSetMethod() == null).Select(p => p.Name).ToArray();

        // The json should not contain base, configured nor readonly properties
        var jsonIgnoredProperties = baseProperties
            .Concat(configuredProperties)
            .Concat(readOnlyProperties).ToArray();

        _jsonSettings = JsonSettings.Minimal
            .Overwrite(j => j.ContractResolver = new DifferentialContractResolver<TReference>(jsonIgnoredProperties));

        // Properties where no mapper should be created for: base and read only properties
        var mapperIgnoredProperties = baseProperties
            .Concat(readOnlyProperties).ToArray();

        _configuredMappers = config.PropertyConfigs.Where(pc => !mapperIgnoredProperties.Contains(pc.PropertyName))
            .Select(pc => MapperFactory.Create(pc, concreteType)).ToArray();
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

    public void ReadValue(IGenericColumns source, object target)
    {
        // Use all configured mappers
        var properties = source;
        foreach (var mapper in _configuredMappers)
        {
            mapper.ReadValue(properties, target);
        }

        // Fill the rest from JSON
        var json = _jsonAccessor.ReadProperty(source);
        if (!string.IsNullOrEmpty(json))
            JsonConvert.PopulateObject(json, target, _jsonSettings);

    }

    public void WriteValue(object source, IGenericColumns target)
    {
        // Convert and write JSON
        var json = JsonConvert.SerializeObject(source, _jsonSettings);
        _jsonAccessor.WriteProperty(target, json);

        // Execute property mappers
        foreach (var mapper in _configuredMappers)
        {
            mapper.WriteValue(source, target);
        }
    }
}