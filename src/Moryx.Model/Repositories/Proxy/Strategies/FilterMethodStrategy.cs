// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Moryx.Model
{
    internal class FilterMethodStrategy : MethodProxyStrategyBase
    {
        private static readonly IDictionary<Filter, MethodInfo> FilterMethods = new Dictionary<Filter, MethodInfo>();
        private static readonly Regex FilterRegex;
        private static MethodInfo _typeGetTypeHandleMethod;
        private static MethodInfo _expParameterMethod;
        private static MethodInfo _expPropertyMethod;
        private static MethodInfo _expConstantMethod;
        private static MethodInfo _expEqualMethod;
        private static MethodInfo _expAndAlsoMethod;
        private static MethodInfo _typeGetMethodMethod;
        private static MethodInfo _expCallMethod;
        private static MethodInfo _expLambdaMethod;
        private static MethodInfo _enumerableToListMethod;

        static FilterMethodStrategy()
        {
            // Create regex for filter methods
            var filters = string.Join("|", Enum.GetNames(typeof(Filter)));
            var selectors = string.Join("|", Enum.GetNames(typeof(Selector)));
            FilterRegex = new Regex($@"(?:Get)(?<filter>(?:{filters})?)(?<selector>(?:{selectors})?)\w*$");

            LoadFilterMethods();
            LoadExpressionMethods();
            LoadEnumerableMethods();
        }

        public override bool CanImplement(MethodInfo methodInfo)
        {
            var match = FilterRegex.Match(methodInfo.Name);
            return match.Success && methodInfo.GetParameters().Length > 0;
        }

        public override void Implement(TypeBuilder typeBuilder, MethodInfo methodInfo, Type baseType, Type targetType)
        {
            // Check return type
            MatchReturnType(methodInfo, targetType);

            // Map parameters to properties
            var parameterPropertyMap = MapParametersToProperties(methodInfo, targetType);

            // Load Filter
            var methodFilter = FilterFromMethod(methodInfo);

            // Build method
            var methodBuilder = DefineMethod(typeBuilder, methodInfo);
            var generator = methodBuilder.GetILGenerator();

            // Expression.Lambda<Func<EntityType, bool>>(BinaryExpression, ParameterExpression[])
            var delegateType = typeof(Func<,>).MakeGenericType(targetType, typeof(bool));
            var lambdaFuncMethod = _expLambdaMethod.MakeGenericMethod(delegateType);

            // Create ToList<Entity>
            var toListMethod = _enumerableToListMethod.MakeGenericMethod(targetType);

            // Declare locals
            generator.DeclareLocal(typeof(ParameterExpression)); // local 0
            generator.DeclareLocal(typeof(Expression<>).MakeGenericType(delegateType)); // local 1 | typeof(Expression<Func<EntityType, bool>>)
            generator.DeclareLocal(typeof(BinaryExpression)); // local 2
            generator.DeclareLocal(typeof(Expression)); // local 3

            // typeof(EntityType)
            generator.Emit(OpCodes.Ldtoken, targetType); // Stack: RuntimeTypeHandle
            generator.Emit(OpCodes.Call, _typeGetTypeHandleMethod); // Stack: typeof(CarEntity)

            // Load string "entity" -> create ParameterExpression -> Store to 0
            generator.Emit(OpCodes.Ldstr, "e"); // Stack: typeof(EntityType), "e"
            generator.Emit(OpCodes.Call, _expParameterMethod); // Stack: ParameterExpression
            generator.Emit(OpCodes.Stloc_0); // Stack: -

            // Property && Property
            for (var argumentIndex = 0; argumentIndex < parameterPropertyMap.Length; argumentIndex++)
            {
                var paramProp = parameterPropertyMap[argumentIndex];

                // Property
                generator.Emit(OpCodes.Ldloc_0); // Stack: ParameterExpression
                generator.Emit(OpCodes.Ldstr, paramProp.Property.Name); // Stack: ParameterExpression, propertyName
                generator.Emit(OpCodes.Call, _expPropertyMethod); // Stack: IndexExpression

                // Constant
                generator.Emit(OpCodes.Ldarg, argumentIndex + 1); // Stack: IndexExpression, (int)arg[index]

                //TODO: write better
                // Expression.Constant uses object as argument. ValueTypes have to be boxed
                if (paramProp.Parameter.ParameterType.IsValueType) 
                {
                    generator.Emit(OpCodes.Box, paramProp.Parameter.ParameterType); // Stack: IndexExpression, (ref)arg[index]
                }

                generator.Emit(OpCodes.Call, _expConstantMethod); // Stack: IndexExpression, ConstantExpression

                var isStringType = paramProp.Parameter.ParameterType == typeof(string);
                if (methodFilter.Selector == Selector.By || methodFilter.Selector == Selector.Contains && !isStringType)
                {
                    generator.Emit(OpCodes.Call, _expEqualMethod); // Stack: BinaryExpression
                    generator.Emit(OpCodes.Castclass, typeof(Expression)); // Stack: Expression
                }
                else if (methodFilter.Selector == Selector.Contains && isStringType)
                {
                    //TODO: this is not done yet -> comments
                    generator.Emit(OpCodes.Castclass, typeof(Expression)); // Stack: IndexExpression, Expression
                    generator.Emit(OpCodes.Stloc_3); // Stack: IndexExpression
                    generator.Emit(OpCodes.Ldtoken, typeof(string)); // Stack: IndexExpression, RuntimeTypeHandle
                    generator.Emit(OpCodes.Call, _typeGetTypeHandleMethod); // Stack: IndexExpression, typeof(string)
                    generator.Emit(OpCodes.Ldstr, nameof(string.Contains)); // Stack: IndexExpression, typeof(string), "Contains"
                    generator.Emit(OpCodes.Call, _typeGetMethodMethod); // Stack: IndexExpression, MethodInfo

                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Newarr, typeof(Expression));
                    generator.Emit(OpCodes.Dup);
                    generator.Emit(OpCodes.Ldc_I4_0);
                    generator.Emit(OpCodes.Ldloc_3);
                    generator.Emit(OpCodes.Castclass, typeof(Expression));
                    generator.Emit(OpCodes.Stelem_Ref); // Stack: IndexExpression, MethodInfo, Expression[1]
                    generator.Emit(OpCodes.Call, _expCallMethod); // Stack: MethodCallExpression
                    generator.Emit(OpCodes.Castclass, typeof(Expression)); // Stack: Expression
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (argumentIndex > 0)
                {
                    // Load both
                    generator.Emit(OpCodes.Ldloc_2); // Stack: BinaryExpression, BinaryExpression
                    generator.Emit(OpCodes.Call, _expAndAlsoMethod); // Stack: BinaryExpression
                    generator.Emit(OpCodes.Castclass, typeof(Expression)); // Stack: Expression
                }

                generator.Emit(OpCodes.Stloc_2); // Stack: -
            }

            generator.Emit(OpCodes.Ldloc_2); //Stack: BinaryExpression

            // Expression.Lambda<Func<CarEntity, bool>>(equalsExp, argParam)
            generator.Emit(OpCodes.Ldc_I4_1); // Stack: BinaryExpression, 1
            generator.Emit(OpCodes.Newarr, typeof(ParameterExpression)); // Stack: BinaryExpression, ParameterExpression[1]
            generator.Emit(OpCodes.Dup); // Stack: BinaryExpression, ParameterExpression[1], ParameterExpression[1]
            generator.Emit(OpCodes.Ldc_I4_0); // Stack: BinaryExpression, ParameterExpression[1], ParameterExpression[1], 0
            generator.Emit(OpCodes.Ldloc_0); // Stack: BinaryExpression, ParameterExpression[1], ParameterExpression[1], 0, ParameterExpression
            generator.Emit(OpCodes.Stelem_Ref); // Stack: BinaryExpression, ParameterExpression[1]
            generator.Emit(OpCodes.Call, lambdaFuncMethod); //Stack: LambdaExpression

            // Store to local 1
            generator.Emit(OpCodes.Stloc_1); //Stack: -

            // Load DbSet Property
            generator.Emit(OpCodes.Ldarg_0); //Stack: this

            // DbSet Property
            var dbSetProperty = baseType.GetProperty("DbSet", BindingFlags.Instance | BindingFlags.NonPublic);
            // ReSharper disable once PossibleNullReferenceException
            generator.Emit(OpCodes.Call, dbSetProperty.GetGetMethod(true)); //Stack: this, DbSet

            // Reload LambdaExpression
            generator.Emit(OpCodes.Ldloc_1); //Stack: this, DbSet, LambdaExpression

            // Apply filter
            if (methodFilter.Filter == Filter.All)
            {
                var whereMethod = FilterMethods[Filter.All].MakeGenericMethod(targetType);
                generator.Emit(OpCodes.Call, whereMethod); //Stack: IQueryable<Entity>
                generator.Emit(OpCodes.Call, toListMethod); //Stack: List<Entity>
            }
            else 
            {
                // Call filter method on Queryable
                var filterMethod = FilterMethods[methodFilter.Filter].MakeGenericMethod(targetType);
                generator.Emit(OpCodes.Call, filterMethod); //Stack: Entity
            }

            //End invocation
            generator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
        }

        private class MethodFilter
        {
            public Filter Filter { get; set; }
            public Selector Selector { get; set; }
        }

        private static MethodFilter FilterFromMethod(MethodInfo methodInfo)
        {
            var match = FilterRegex.Match(methodInfo.Name);
            var filterMatch = match.Groups["filter"];
            var selectorMatch = match.Groups["selector"];
            var propertyMatch = match.Groups["property"];

            var filterMap = new MethodFilter();

            if (filterMatch.Success && !string.IsNullOrEmpty(filterMatch.Value))
            {
                filterMap.Filter = (Filter) Enum.Parse(typeof(Filter), filterMatch.Value);
            }
            else
            {
                filterMap.Filter = Filter.FirstOrDefault;
            }

            if (selectorMatch.Success && !string.IsNullOrEmpty(selectorMatch.Value))
            {
                filterMap.Selector = (Selector)Enum.Parse(typeof(Selector), selectorMatch.Value);
            }
            else
            {
                filterMap.Selector = Selector.By;
            }

            if (propertyMatch.Success)
            {
                //TODO implement property match
            }

            return filterMap;
        }
        
        private enum Filter
        {
            First,
            FirstOrDefault,
            Single,
            SingleOrDefault,
            All,
        }

        private enum Selector
        {
            By,
            Contains
        }

        private static void LoadEnumerableMethods()
        {
            var enumerableMethods = typeof(Enumerable).GetMethods();

            _enumerableToListMethod = enumerableMethods.Single(m =>
                m.Name.Equals(nameof(Enumerable.ToList)));
        }

        private static void LoadFilterMethods()
        {
            var queryableMethods = typeof(Queryable).GetMethods();

            // Queryable.FirstOrDefault(IQueryable, Expression)
            var methodInfo = queryableMethods.Single(m =>
                m.Name.Equals(nameof(Queryable.FirstOrDefault)) && m.GetParameters().Length == 2);
            FilterMethods[Filter.FirstOrDefault] = methodInfo;

            // Queryable.First(IQueryable, Expression)
            methodInfo = queryableMethods.Single(m =>
                m.Name.Equals(nameof(Queryable.First)) && m.GetParameters().Length == 2);
            FilterMethods[Filter.First] = methodInfo;

            // Queryable.Single(IQueryable, Expression)
            methodInfo = queryableMethods.Single(m =>
                m.Name.Equals(nameof(Queryable.Single)) && m.GetParameters().Length == 2);
            FilterMethods[Filter.Single] = methodInfo;

            // Queryable.SingleOrDefault(IQueryable, Expression)
            methodInfo = queryableMethods.Single(m =>
                m.Name.Equals(nameof(Queryable.SingleOrDefault)) && m.GetParameters().Length == 2);
            FilterMethods[Filter.SingleOrDefault] = methodInfo;

            // Queryable.Where(IQueryable, Expression)
            methodInfo = queryableMethods.First(m => //TODO: first is not a good idea, filter on expression
                m.Name.Equals(nameof(Queryable.Where)) && m.GetParameters().Length == 2);
            FilterMethods[Filter.All] = methodInfo;
        }

        private static void LoadExpressionMethods()
        {
            var typeType = typeof(Type);
            var expressionType = typeof(Expression);
            var expressionMethods = expressionType.GetMethods();
            
            // GetType
            _typeGetTypeHandleMethod = typeType.GetMethod(nameof(Type.GetTypeFromHandle));

            // Type.GetMethod
            _typeGetMethodMethod = typeType.GetMethods().Single(m =>
                m.Name.Equals(nameof(Type.GetMethod)) &&
                m.GetParameters().Length == 1);

            // Expression.Parameter(typeof(EntityType), "entity");
            _expParameterMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.Parameter)) &&
                m.GetParameters().Length == 2);

            // Expression.Property(ParameterExpression, string);
            _expPropertyMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.Property)) &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == expressionType &&
                m.GetParameters()[1].ParameterType == typeof(string));

            // Expression.Constant(object)
            _expConstantMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.Constant)) &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == typeof(object));

            // Expression.Equal(Expression, Expression)
            _expEqualMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.Equal)) &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == expressionType &&
                m.GetParameters()[1].ParameterType == expressionType);

            // Expression.Equal(Expression, Expression)
            _expEqualMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.Equal)) &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == expressionType &&
                m.GetParameters()[1].ParameterType == expressionType);

            // Expression.AndAlso(Expression, Expression)
            _expAndAlsoMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.AndAlso)) &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == expressionType &&
                m.GetParameters()[1].ParameterType == expressionType);

            // Expression.Call(Expression, MethodInfo, Expression[])
            _expCallMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.Call)) &&
                m.GetParameters().Length == 3 &&
                m.GetParameters()[0].ParameterType == expressionType &&
                m.GetParameters()[1].ParameterType == typeof(MethodInfo) &&
                m.GetParameters()[2].ParameterType == typeof(Expression[]));

            // Expression.Lambda(Expression, ParameterExpression[])
            _expLambdaMethod = expressionMethods.Single(m =>
                m.Name.Equals(nameof(Expression.Lambda)) &&
                m.ContainsGenericParameters && m.GetGenericArguments().Length == 1 &&
                m.GetGenericArguments()[0].DeclaringType == expressionType &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == expressionType &&
                m.GetParameters()[1].ParameterType == typeof(ParameterExpression[]));
        }
    }
}
