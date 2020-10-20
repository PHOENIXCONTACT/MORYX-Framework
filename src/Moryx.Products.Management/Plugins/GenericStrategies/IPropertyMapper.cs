// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using System.Reflection;
using Moryx.Modules;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Strategy to map a single property into the database
    /// </summary>
    public interface IPropertyMapper : IConfiguredPlugin<PropertyMapperConfig>, IGenericMapper
    {
        /// <summary>
        /// Name of the property represented by this mapper
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Convert a given property value into a database binary expression searching for that value
        /// </summary>
        Expression ToColumnExpression(ParameterExpression columnParam, ExpressionType type, object value);
    }
}
