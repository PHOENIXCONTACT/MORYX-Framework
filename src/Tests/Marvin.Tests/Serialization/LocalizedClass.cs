// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Marvin.Tests.Serialization;

namespace Marvin.Tests
{
    [ClassDisplay(Name = nameof(strings.ClassName), ResourceType = typeof(strings))]
    public class LocalizedClass
    {
        public const string PropDisplayNameAttributeDisplayName = "Display Name";

        [Display(Name = nameof(strings.PropDisplayAttribute_Name), Description = nameof(strings.PropDisplayAttribute_Description), ResourceType = typeof(strings))]
        public string PropDisplayAttribute { get; set; }

        [DisplayName(PropDisplayNameAttributeDisplayName)]
        public string PropDisplayNameAttribute { get; set; }
    }

    public class LocalizedMethodDummyClass
    {
        [Display(Name = nameof(strings.InitiateWorldTermination), Description = nameof(strings.InitiateWorldTerminationDescription), ResourceType = typeof(strings))]
        public bool InitiateWorldTermination([Display(Name = nameof(strings.EvacuatePeopleParam), Description = nameof(strings.EvacuatePeopleParamDescription), ResourceType = typeof(strings))] bool evacuatePeople,
            [Display(Name = nameof(strings.NameOfTerminatorParam), ResourceType = typeof(strings))] string nameOfTerminator)
        {
            return true;
        }
    }
}
