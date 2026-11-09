using Moryx.AbstractionLayer.Drivers;
using Moryx.Drivers.Mqtt;

namespace Moryx.Drivers.Mqtt.States
{
    internal class ReconnectBackoffState : DriverMqttState
    {
        public ReconnectBackoffState(MqttDriver context, StateMap stateMap) : base(context, stateMap, StateClassification.Offline)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Context.DelayedConnectionAttempt();
        }

        internal override void ReconnectTimerElapsed()
        {
            base.ReconnectTimerElapsed();
            NextState(StateConnecting);
        }
    }
}
