// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Reflection.Emit;

namespace Moryx.Model.Repositories.Proxy
{
    internal abstract class MethodProxyStrategyBase : IMethodProxyStrategy
    {
        protected MethodBuilder DefineMethod(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Final |
                                                      MethodAttributes.Virtual | MethodAttributes.NewSlot |
                                                      MethodAttributes.HideBySig;

            var argumentTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            return typeBuilder.DefineMethod(methodInfo.Name, methodAttributes, methodInfo.ReturnType, argumentTypes);
        }

        protected static ParameterPropertyMap[] MapParametersToProperties(MethodInfo methodInfo, Type targetType)
        {
            return methodInfo.GetParameters().Select(p => MapParameterToProperty(p, targetType)).ToArray();
        }

        protected static ParameterPropertyMap MapParameterToProperty(ParameterInfo parameterInfo, Type targetType)
        {
            var propertyName = char.ToUpper(parameterInfo.Name[0]) + parameterInfo.Name.Substring(1);
            var targetProperty = targetType.GetProperty(propertyName);

            if (targetProperty == null)
                throw new InvalidOperationException($"Cannot find property with name '{propertyName}' on target type '{targetType.Name}'.");

            if (!targetProperty.PropertyType.IsAssignableFrom(parameterInfo.ParameterType))
                throw new InvalidOperationException($"Types doesn't match: Property '{parameterInfo.Name}', Target type: {targetProperty.PropertyType}, Source type: {parameterInfo.ParameterType}.");

            return new ParameterPropertyMap(parameterInfo, targetProperty);
        }

        protected static void MatchReturnType(MethodInfo methodInfo, Type targetType)
        {
            var methodReturnType = methodInfo.ReturnType;
            var exception = new InvalidOperationException($"The return type '{methodInfo.ReturnType.Name}' of the method '{methodInfo.Name}' " +
                                                          $"is not assignable from target type '{targetType.Name}'.");
            // On generic types
            if (methodReturnType.IsGenericType)
            {
                //TODO: Find more / better ways to check generic return type
                // More than one generic argument
                if (methodReturnType.GetGenericArguments().Length != 1)
                    throw exception;

                var genericArg = methodReturnType.GetGenericArguments()[0];
                if (!genericArg.IsAssignableFrom(targetType))
                    throw exception;
            }
            else
            {
                // If type is not assignable
                if (!methodInfo.ReturnType.IsAssignableFrom(targetType))
                    throw exception;
            }
        }

        public abstract bool CanImplement(MethodInfo methodInfo);

        public abstract void Implement(TypeBuilder typeBuilder, MethodInfo methodInfo, Type baseType, Type targetType);
    }
}
