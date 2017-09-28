namespace Marvin.PlatformTools.Tests
{
    internal sealed class CState : MyStateBase
    {
        public CState(MyContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void CtoA()
        {
            NextState(StateA);
            Context.HandleCtoA();
        }
    }
}