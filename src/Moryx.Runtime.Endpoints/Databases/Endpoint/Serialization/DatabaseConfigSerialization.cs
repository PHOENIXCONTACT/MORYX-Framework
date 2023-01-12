// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Model.Configuration;
using System.Reflection;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Model.Attributes;
using Moryx.Model;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Serialization
{
    public class DatabaseConfigSerialization : DefaultSerialization
    {
        public override string[] PossibleValues(Type memberType, ICustomAttributeProvider attributeProvider)
        {
            if (memberType == typeof(DatabaseConnectionSettings))
            {
                return ReflectionTool
                    .GetPublicClasses(typeof(DatabaseConnectionSettings), (t) =>
                    {
                        return t != typeof(DatabaseConnectionSettings);
                    })
                    .Select(t => t.AssemblyQualifiedName)
                    .ToArray();
            }

            if(attributeProvider.GetCustomAttribute<PossibleConfiguratorsAttribute>() != null) { 
                return ReflectionTool
                    .GetPublicClasses(typeof(IModelConfigurator))
                    .Select(t => t.AssemblyQualifiedName) 
                    .ToArray();
            }

            return Array.Empty<string>();
        }
    }
}
