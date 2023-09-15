using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Resources.Demo
{
    [ResourceRegistration]
    [EntrySerialize]
    public class DocumentManager : PublicResource
    {

        [EntrySerialize]
        public string FolderPath { get; set; }
    }
}
