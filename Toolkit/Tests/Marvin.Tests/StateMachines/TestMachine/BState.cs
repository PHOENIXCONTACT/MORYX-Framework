namespace Marvin.Tests
{
    internal sealed class BState : MyStateBase
    {
        public BState(MyContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void BtoC()
        {
            NextState(StateC);
            Context.HandleBtoC();
        }
    }
}