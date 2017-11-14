using Marvin.StateMachines;

namespace Marvin.Tests
{
    public class MyContext : IStateContext
    {
        public MyStateBase State { get; private set; }

        public bool AtoBTriggered { get; set; }

        public bool BtoCTriggered { get; set; }

        public bool CtoATriggered { get; set; }

        internal void HandleAtoB()
        {
            AtoBTriggered = true;
        }

        internal void HandleBtoC()
        {
            BtoCTriggered = true;
        }

        internal void HandleCtoA()
        {
            CtoATriggered = true;
        }

        public void SetState(IState state)
        {
            State = (MyStateBase)state;
        }
    }
}