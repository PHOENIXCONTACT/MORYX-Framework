// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.AspNetCore.Mqtt.Builders;
using Moryx.AspNetCore.Mqtt.Components;
using Moryx.AspNetCore.Mqtt.Exceptions;
using Moryx.Serialization;
using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Endpoints;

/// <summary>
/// Endpoint to remote call a procedure/method on a resource
/// </summary>
public class ResourceRpcEndpoint(ILogger<ResourceRpcEndpoint> logger, IResourceManagement resourceManagement, MqttClientUserOptions options) : IMqttEndpoint
{
    public void Map(IMqttRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("resources/{identifier}/invoke/{methodName}", context =>
        {
            var messageBuilder = new MqttApplicationMessageBuilder();
            messageBuilder.WithTopic(context.RequestMessage.ResponseTopic);

            var identifier = context.FromParameterValues<string>("identifier");
            var methodName = context.FromParameterValues<string>("methodName") ?? string.Empty;
            var resource = resourceManagement.GetResource<IResource>(x => x is IIdentifiableObject obj && obj.Identity.Identifier == identifier || x.Name == identifier);
            try
            {
                var methodToInvoke = FindMethod(resource, methodName);
                return InvokeMethod(context, messageBuilder, resource, methodToInvoke);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex);
                return GetExceptionMessage(messageBuilder, ex.Message);
            }
        });
    }

    private static MethodInfo FindMethod(IResource resource, string methodName)
    {
        var interfaces = resource.GetType().GetInterfaces();
        var methodToInvoke = interfaces.SelectMany(i => i.GetMethods().Where(m => m.ReturnType == typeof(Task) || (m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))))
                      .FirstOrDefault(x => x.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
        return resource is null
            ? throw new ResourceNotFoundException()
            : methodToInvoke is null
            ? throw new MethodNotFoundException(resource.Name, methodName)
            : methodToInvoke;
    }

    private MqttApplicationMessage InvokeMethod(MqttEndpointContext context, MqttApplicationMessageBuilder responseBuilder, IResource resource, MethodInfo methodToInvoke)
    {
        object methodResult;

        var parameters = GetMethodParametersFromPayload(methodToInvoke, context);
        methodResult = methodToInvoke.Invoke(resource, parameters);

        if (methodResult?.GetType() == typeof(void))
        {
            responseBuilder.WithPayload(MqttMessageSerialization.GetJsonPayload(new { }, options.JsonSerializerOptions));
            return responseBuilder.Build();
        }

        // TODO: what if the method is a Task or Task<T> ?
        var topic = string.IsNullOrEmpty(context.RequestMessage.ResponseTopic)
                        ? context.RequestMessage.Topic.Replace("invoke", "invoked")
                        : context.RequestMessage.ResponseTopic;
        responseBuilder.WithTopic(topic);
        var payload = methodResult ?? new { };
        var jsonPayload = MqttMessageSerialization.GetJsonPayload(new MethodInvocationResult
        {
            Value = payload,
            ValueType = EntryConvert.TransformType(methodToInvoke.ReturnType)
        }, options.JsonSerializerOptions) ?? string.Empty;
        return responseBuilder.WithPayload(jsonPayload).Build();
    }

    private MqttApplicationMessage GetExceptionMessage(MqttApplicationMessageBuilder builder, string exception)
    {
        var payload = new MethodInvocationResult
        {
            Value = new { Message = exception },
            ValueType = EntryValueType.Exception
        };
        var json = MqttMessageSerialization.GetJsonPayload(payload, options.JsonSerializerOptions);
        return builder.WithPayload(json).Build();
    }

    private static object[] GetMethodParametersFromPayload(MethodInfo methodToInvoke, MqttEndpointContext context)
    {
        var methodToInvokeParameters = methodToInvoke.GetParameters();
        var parameters = new object[methodToInvokeParameters.Length];

        for (var i = 0; i < methodToInvokeParameters.Length; i++)
        {
            try
            {
                // get the values of the method's parameters from the body of the message (payload)
                parameters[i] = context.FromBody(methodToInvokeParameters[i].Name!, methodToInvokeParameters[i].ParameterType);
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Could not convert parameter '{methodToInvokeParameters[i].Name}' to type '{methodToInvokeParameters[i].ParameterType.FullName}'", ex);
            }
        }
        return parameters;
    }
}

