// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.Hardware;
using Moryx.Serialization;

namespace Moryx.Resources.Samples
{
    public abstract class Cell : Resource
    {
        #region Config

        [DataMember]
        public int LastCall { get; set; }

        #endregion

        [EntrySerialize, DisplayName("Editor Value")]
        public int EditorValue { get; set; }

        [EntrySerialize, DisplayName("Do Foo")]
        public int Foo([Description("Very important parameter")] string bla = "Hallo")
        {
            LastCall = bla.Length;
            RaiseResourceChanged();

            return LastCall;
        }

        [EntrySerialize, Description("Perform a self test of the resource")]
        public void PerformSelfTest()
        {
        }

        [EntrySerialize, Description("Move an axis of the resource")]
        public void Move(Axes axis, AxisPosition position)
        {
        }
    }
}
