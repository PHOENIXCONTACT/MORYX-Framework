// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Identity;
using Moryx.Container;
using Moryx.Products.Model;
using Moryx.Serialization;
using Moryx.Tools;
using Newtonsoft.Json;

namespace Moryx.Products.Management
{
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

        private IPropertyAccessor<IGenericColumns, string> JsonAccessor { get; set; }


        public void Initialize(Type concreteType, IGenericMapperConfiguration config)
        {
            // Get JSON accessor
            var jsonColumn = typeof(IGenericColumns).GetProperty(config.JsonColumn);
            JsonAccessor = ReflectionTool.PropertyAccessor<IGenericColumns, string>(jsonColumn);

            var baseProperties = typeof(TBase).GetProperties()
                .Select(p => p.Name);
            var configuredProperties = config.PropertyConfigs.Select(cm => cm.PropertyName);
            var ignoredProperties = baseProperties.Concat(configuredProperties).ToArray();
            _jsonSettings = JsonSettings.Minimal
                .Overwrite(j => j.ContractResolver = new DifferentialContractResolver<TReference>(ignoredProperties));

            _configuredMappers = config.PropertyConfigs.Select(pc => MapperFactory.Create(pc, concreteType)).ToArray();
        }

        public bool HasChanged(IGenericColumns storage, object instance)
        {
            // Compare JSON and mappers to entity
            var json = JsonConvert.SerializeObject(instance, _jsonSettings);
            return JsonAccessor.ReadProperty(storage) != json || _configuredMappers.Any(m => m.HasChanged(storage, instance));
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

        private static object ExtractExpressionValue(Expression expression)
        {
            if (expression is ConstantExpression constEx)
                return constEx.Value;

            if (expression is MemberExpression memEx)
            {
                var container = (memEx.Expression as ConstantExpression)?.Value;
                if (memEx.Member is FieldInfo field)
                    return field.GetValue(container);
                if (memEx.Member is PropertyInfo prop)
                    return prop.GetValue(container);
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
            var json = JsonAccessor.ReadProperty(source);
            if (!string.IsNullOrEmpty(json))
                JsonConvert.PopulateObject(json, target, _jsonSettings);

        }

        public void WriteValue(object source, IGenericColumns target)
        {
            // Convert and write JSON
            var json = JsonConvert.SerializeObject(source, _jsonSettings);
            JsonAccessor.WriteProperty(target, json);

            // Execute property mappers
            foreach (var mapper in _configuredMappers)
            {
                mapper.WriteValue(source, target);
            }
        }
    }
}
