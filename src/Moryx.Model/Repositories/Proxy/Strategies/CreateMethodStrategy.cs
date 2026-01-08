// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Moryx.Model.Repositories.Proxy;

internal class CreateMethodStrategy : MethodProxyStrategyBase
{
    //TODO: Collection Properties

    private static readonly Regex CreateRegex = new(@"(?:Create)$");

    public override bool CanImplement(MethodInfo methodInfo)
    {
        var match = CreateRegex.Match(methodInfo.Name);
        return match.Success && methodInfo.GetParameters().Length > 0;
    }

    public override void Implement(TypeBuilder typeBuilder, MethodInfo methodInfo, Type baseType, Type targetType)
    {
        // Check return type
        MatchReturnType(methodInfo, targetType);

        // Map parameters to properties
        var parameterPropertyMap = MapParametersToProperties(methodInfo, targetType);

        // Build Method
        var methodBuilder = DefineMethod(typeBuilder, methodInfo);
        var generator = methodBuilder.GetILGenerator();
        var createMethod = baseType.GetMethods().Single(m => m.Name.Equals("Create") && m.GetParameters().Length == 0);

        generator.DeclareLocal(targetType); // loc_0
        generator.Emit(OpCodes.Ldarg_0); //Stack: this
        generator.Emit(OpCodes.Call, createMethod); //Stack: Entity
        generator.Emit(OpCodes.Stloc_0); //Stack: -

        for (var argumentIndex = 0; argumentIndex < parameterPropertyMap.Length; argumentIndex++)
        {
            var paramProp = parameterPropertyMap[argumentIndex];
            var parameterType = paramProp.Parameter.ParameterType;
            var propertyType = paramProp.Property.PropertyType;

            // Check if type is IEnumerable<>
            var isParameterEnumerable = parameterType.IsGenericType && !parameterType.IsArray &&
                                        typeof(IEnumerable<>).IsAssignableFrom(parameterType.GetGenericTypeDefinition());
            if (isParameterEnumerable)
            {
                var genericArg = parameterType.GetGenericArguments()[0];

                // Check if Property is ICollection<NavigationEntity>
                var requiredType = typeof(ICollection<>).MakeGenericType(genericArg);
                if (requiredType.IsAssignableFrom(propertyType))
                    throw new InvalidOperationException("Create entities with collection is currently not supported!");
                else
                    throw new InvalidOperationException("Method parameter is type of IEnumerable<T> " +
                                                        "but the target property is not of type ICollection<T>");
            }
            else
            {
                generator.Emit(OpCodes.Ldloc_0); //Stack: Entity
                generator.Emit(OpCodes.Ldarg, argumentIndex + 1); //Stack: Entity, arg[index]
                generator.Emit(OpCodes.Callvirt, paramProp.Property.GetSetMethod()); // -
            }
        }

        generator.Emit(OpCodes.Ldloc_0); //Stack: Entity
        generator.Emit(OpCodes.Ret);

        typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
    }
}