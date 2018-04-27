using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
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
