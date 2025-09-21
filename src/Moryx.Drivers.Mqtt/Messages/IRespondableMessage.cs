namespace Moryx.Drivers.Mqtt.Messages;

public interface IRespondableMessage
{
    public string ResponseIdentifier { get; set; }
}