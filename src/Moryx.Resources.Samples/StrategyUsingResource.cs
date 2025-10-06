// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Serialization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Moryx.Resources.Demo
{

    [DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy)), ResourceRegistration]
    public class StrategyUsingResource : Resource
    {
        [PluginNameSelector(typeof(IWhiteSpaceRemovingStrategy))]
        [DataMember, EntrySerialize]
        public string Strategy { get; set; }
    }

    public interface IWhiteSpaceRemovingStrategy
    {
        string DropSpaces(string s);
    }

    [Plugin(LifeCycle.Singleton, typeof(IWhiteSpaceRemovingStrategy), Name = "Regex Remover")]
    public class RegexWhiteSpaceRemovingStrategy : IWhiteSpaceRemovingStrategy
    {
        public string DropSpaces(string s) => Regex.Replace(s, @"\s+", "");
    }

    [Plugin(LifeCycle.Singleton, typeof(IWhiteSpaceRemovingStrategy), Name = "Linq Remover")]
    public class LinqWhiteSpaceRemovingStrategy : IWhiteSpaceRemovingStrategy
    {
        public string DropSpaces(string s) => string.Concat(s.Where(c => !char.IsWhiteSpace(c)));
    }
}

