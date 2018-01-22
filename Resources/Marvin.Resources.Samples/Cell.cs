using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Drivers.Marking;
using Marvin.AbstractionLayer.Resources;
using Marvin.Resources.Management;
using Marvin.AbstractionLayer.Hardware;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    public abstract class Cell : PublicResource
    {

        #region Config

        [DataMember]
        public int LastCall { get; set; }
        #endregion

        [EditorVisible, DisplayName("Editor Value")]
        public int EditorValue { get; set; }
       
        [EditorVisible, DisplayName("Do Foo")]
        public int Foo([Description("Very important parameter")]string bla = "Hallo")
        {
            LastCall = bla.Length;
            RaiseResourceChanged();

            return LastCall;
        }

        [EditorVisible, Description("Perform a self test of the resource")]
        public void PerformSelfTest()
        {
        }

        [EditorVisible, Description("Move an axis of the resource")]
        public void Move(Axes axis, AxisPosition position)
        {
        }
    }
}
