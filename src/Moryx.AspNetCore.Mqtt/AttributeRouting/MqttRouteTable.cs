// Modifications Copyright (c) Atlas Lift Tech Inc. All rights reserved.
// Source https://github.com/Atlas-LiftTech/MQTTnet.AspNetCore.AttributeRouting/blob/17684ada11692549253b4e924e5740cb4c012c72/Routing/MqttRouteTable.cs

// Further modifications by Phoenix Contact GmbH & Co. KG. 2026
using System.Runtime.CompilerServices;

// this a Moq specific code 
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Moryx.AspNetCore.Mqtt.AttributeRouting;

/// <summary>
/// Contains all the available MqttRoute instances
/// </summary>
internal class MqttRouteTable(MqttRoute[] routes) : IMqttRouteTable
{

    /// <inheritdoc />
    public virtual IReadOnlyList<MqttRoute> Routes { get; } = routes;

    /// <inheritdoc />
    public MqttRoute? MatchingRoute(MqttRouteContext context)
        => Routes.FirstOrDefault(x => x.Match(context.RequestUrl));
}

