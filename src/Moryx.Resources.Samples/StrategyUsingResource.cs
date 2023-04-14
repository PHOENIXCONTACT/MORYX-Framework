using Moryx.AbstractionLayer.Drivers.Rfid;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moryx.Serialization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Moryx.Resources.Demo
{

    [DependencyRegistration(typeof(IWhiteSpaceRemovingStrategy)), ResourceRegistration]
    public class StrategyUsingResource: Resource
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
