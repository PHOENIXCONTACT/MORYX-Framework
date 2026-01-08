// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Drivers.Axis;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples
{
    [ResourceRegistration]
    public class GateResource : Cell
    {
        public void MoveAxes(Axes axes, AxisPosition position)
        {
        }

        public override object Descriptor => new AxisControls(this);

        [DisplayName("WPCs")]
        [ReferenceOverride(nameof(Children))]
        public IReferences<Wpc> Wpcs { get; set; }

        [EntrySerialize]
        public class AxisControls
        {
            private readonly GateResource _target;

            public AxisControls(GateResource target)
            {
                _target = target;
            }

            [DisplayName("Rotate clockwise"), Description("Rotate the door in clockwise direction")]
            public void RotateClockwise()
            {
                _target.MoveAxes(Axes.Door, AxisPosition.RotateClockwise);
            }

            [DisplayName("Rotate counter clockwise"), Description("Rotate the door in counte clockwise direction")]
            public void RotateCounterClockwise()
            {
                _target.MoveAxes(Axes.Door, AxisPosition.RotateCounterClockwise);
            }
        }
    }
}
