namespace Marvin.Tests
{
    internal sealed class AState : MyStateBase
    {
        public AState(MyContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override void Initial()
        {
            
        }

        public override void AtoB()
        {
            NextState(StateB);
            Context.HandleAtoB();
        }
    }
}