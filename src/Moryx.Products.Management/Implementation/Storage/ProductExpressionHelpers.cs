// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using System.Reflection;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Model;

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

        public static bool IsTypeQuery<TInstance>(Expression<Func<TInstance, bool>> selector, out MemberInfo typeMember, out object memberValue)
        {
            typeMember = null;
            memberValue = null;

            var body = selector.Body;
            // Extract the property targeted by the expression
            switch (body)
            {
                case BinaryExpression binary when binary.NodeType == ExpressionType.Equal:
                    // Extract member and value
                    if (binary.Left is MemberExpression bLeft && IsTypeExpression(bLeft, out typeMember))
                    {
                        memberValue = ExtractExpressionValue(binary.Right);
                    }
                    else if (binary.Right is MemberExpression bRight && IsTypeExpression(bRight, out typeMember))
                    {
                        memberValue = ExtractExpressionValue(binary.Left);
                    }
                    break;
                case MethodCallExpression call:
                    // For now only implement identity check
                    var method = call.Method;
                    if (method.Name == nameof(Equals))
                    {
                        if (call.Object is MemberExpression callMemEx && IsTypeExpression(callMemEx, out typeMember))
                        {
                            memberValue = ExtractExpressionValue(call.Arguments.First());
                        }
                        else if (call.Arguments.First() is MemberExpression argMemEx && IsTypeExpression(argMemEx, out typeMember))
                        {
                            memberValue = ExtractExpressionValue(call.Object);
                        }

                    }
                    break;
            }

            return memberValue is not null;
        }

        private static bool IsTypeExpression(MemberExpression expression, out MemberInfo typeMember)
        {
            typeMember = null;
            do
            {
                if (expression.Member is PropertyInfo propertyInfo && typeof(ProductType).IsAssignableFrom(propertyInfo.PropertyType))
                    return true;
                typeMember = expression.Member;
                expression = expression.Expression as MemberExpression;
            } while (expression is not null);

            return false;
        }

        internal static Expression<Func<ProductTypeEntity, bool>> AsVersionExpression(Expression<Func<IGenericColumns, bool>> expression)
        {
            // Extract lamda expression body and column
            var lambda = (LambdaExpression)expression;
            var binaryExpression = (BinaryExpression)lambda.Body;
            var columnExpression = (MemberExpression)binaryExpression.Left;

            // Build new parameter expression
            var rootEntity = Expression.Parameter(typeof(ProductTypeEntity));
            var versionExpression = Expression.Property(rootEntity, nameof(ProductTypeEntity.CurrentVersion));
            var versionColumn = Expression.Property(versionExpression, columnExpression.Member.Name);

            // Build new binary expression
            var versionBinary = Expression.MakeBinary(binaryExpression.NodeType, versionColumn, binaryExpression.Right);
            return Expression.Lambda(versionBinary, rootEntity) as Expression<Func<ProductTypeEntity, bool>>;
        }
    }
}
