using System.ComponentModel;
using Marvin.AbstractionLayer.Hardware;
using Marvin.AbstractionLayer.Resources;
using Marvin.Resources.Management;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [ResourceRegistration(nameof(GateResource))]
    public class GateResource : Cell
    {
        public void MoveAxes(Axes axes, AxisPosition position)
        {
        }

        public override object Descriptor => new AxisControls(this);

        [EditorVisible]
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