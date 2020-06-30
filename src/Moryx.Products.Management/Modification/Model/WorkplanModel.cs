// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Products.Model;
using Moryx.Workflows;

namespace Moryx.Products.Management.Modification
{
    [DataContract(IsReference = true)]
    internal class WorkplanModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public WorkplanState State { get; set; }

        internal static WorkplanModel FromWorkplan(IWorkplan wp)
        {
            return new WorkplanModel
            {
                Id = wp.Id,
                Name = wp.Name,
                Version = wp.Version,
                State = wp.State
            };   
        }
    }
}
