// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples
{
    [ResourceRegistration]
    public class Machine : Resource
    {

        [DataMember, EntrySerialize]
        public string CurrentState { get; set; }

        [DataMember, EntrySerialize]
        public bool InOperation { get; set; }

        [DataMember, EntrySerialize]
        [Description("Maximum production hours per year")]
        public int ProductionHours { get; set; }

        [DataMember, EntrySerialize]
        public double Power { get; set; }

        [DataMember, EntrySerialize]
        public MachineType MachineType { get; set; }

        [DataMember, EntrySerialize]
        public MachineInfos AdditionalInformation { get; set; }

        [DataMember, EntrySerialize]
        [PossibleStaff]
        public IList<TechnicalStaff> PossibleTechnicalStaffs { get; set; }

        [DataMember, EntrySerialize]
        public IList<int> Values { get; set; }

        [DataMember, EntrySerialize, ReadOnly(true)]
        public int Count { get; private set; }

        [EntrySerialize]
        [DisplayName("Nothing happens")]
        [Description("Return value of this method is void. ProductionHours are set to zero")]
        public void NothingHappens()
        {
            ProductionHours = 0;
        }

        [EntrySerialize]
        [DisplayName("Increase by 1")]
        [Description("Value will be increased by 1")]
        public int Increase()
        {
            Count++;
            return Count;
        }

        [EntrySerialize]
        [Description("Value will be decreased by 1")]
        public int Decrease()
        {
            Count--;
            return Count;
        }

        [EntrySerialize]
        [Description("Value will be increased by the selected amount")]
        public int IncreaseByAmount(int amount)
        {
            Count += amount;
            return Count;
        }

        [EntrySerialize]
        public void MethodWithArray(int[] parameter)
        {

        }

        [EntrySerialize]
        public void MethodWithList(List<int> parameter)
        {

        }

        [EntrySerialize]
        [Description("Value will be increased by the selected amount. This is a very long description in order to have a description over several lines." +
            "So here is a little bit more text. And so on.")]
        public List<int> IncreaseByAmountTestParameters(int amount, bool whyNot, string alsoAString, MachineType whyNotAnEnum, List<int> andAList)
        {
            Count += amount;
            return new List<int> { Count, 2, 3, 4, 1, 2, 5, 2 };
        }
    }

    public enum MachineType
    {
        manual,
        halfAutomatic,
        automatic
    }

    public class MachineInfos
    {
        [DataMember, EntrySerialize]
        [Description("Current technical staff")]
        public string TechnicalStaff { get; set; }

        [DataMember, EntrySerialize]
        public int MaximumNumberOperator { get; set; }
    }

    public class TechnicalStaff
    {
        [DataMember, EntrySerialize]
        public string FirstName { get; set; }

        [DataMember, EntrySerialize]
        public string LastName { get; set; }

        [DataMember, EntrySerialize]
        public int StaffNumber { get; set; }
    }

    public class ExpertStaff : TechnicalStaff
    {
        [DataMember, EntrySerialize]
        public int Level { get; set; }
    }

    public class PossibleStaffAttribute : PossibleValuesAttribute
    {
        public override bool OverridesConversion => true;

        public override bool UpdateFromPredecessor => false;

        public override IEnumerable<string> GetValues(Container.IContainer container)
        {
            return new[] { nameof(TechnicalStaff), nameof(ExpertStaff) };
        }

        public override object Parse(Container.IContainer container, string value)
        {
            switch (value)
            {
                case nameof(ExpertStaff):
                    return new ExpertStaff();
            }
            return new TechnicalStaff();
        }
    }
}
