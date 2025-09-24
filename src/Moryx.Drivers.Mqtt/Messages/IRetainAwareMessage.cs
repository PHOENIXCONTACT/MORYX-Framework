namespace Moryx.Drivers.Mqtt.Messages;

public interface IRetainAwareMessage
{
    public bool Retain { get; set; }
}