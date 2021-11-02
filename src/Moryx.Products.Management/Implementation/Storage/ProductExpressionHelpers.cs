// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Management
{
    internal static class ProductExpressionHelpers
    {
        public static object ExtractExpressionValue(Expression expression)
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

        public static bool IsTypeQuery<TInstance>(Expression<Func<TInstance, bool>> selector, out ProductType productType)
        {
            productType = null;
            var body = selector.Body;
            // Extract the property targeted by the expression
            switch (body)
            {
                case BinaryExpression binary when binary.NodeType == ExpressionType.Equal:
                    if (binary.Left is MemberExpression bLeft && bLeft.Member.Name == nameof(ProductInstance.Type))
                    {
                        productType = ExtractExpressionValue(binary.Right) as ProductType;
                    }
                    if (binary.Right is MemberExpression bRight && bRight.Member.Name == nameof(ProductInstance.Type))
                    {
                        productType = ExtractExpressionValue(binary.Left) as ProductType;
                    }
                    break;
                case MethodCallExpression call:
                    // For now only implement identity check
                    var method = call.Method;
                    if (method.Name == nameof(Equals))
                    {
                        if (call.Object is MemberExpression callMemEx && callMemEx.Expression is ConstantExpression)
                        {
                            productType = ExtractExpressionValue(call.Object) as ProductType;
                        }
                        else
                        {
                            productType = ExtractExpressionValue(call.Arguments.First()) as ProductType;
                        }

                    }
                    break;
            }
            
            return productType != null;
        }
    }
}
