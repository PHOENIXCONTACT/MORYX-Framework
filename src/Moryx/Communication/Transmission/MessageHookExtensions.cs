// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication;

/// <summary>
/// Extensions to improve use of message hooks in your application
/// </summary>
public static class MessageHookExtensions
{
    /// <summary>
    /// Iterate over all hooks and call <see cref="IBinaryMessageHook.SendingMessage"/>
    /// </summary>
    public static bool SendingMessage<T>(this IEnumerable<IBinaryMessageHook> hooks, ref T payload)
        where T : class
    {
        object message = payload;
        var result = hooks.Aggregate(true, (current, hook) => current & hook.SendingMessage(ref message));
        payload = (T)message;
        return result;
    }

    /// <summary>
    /// Iterate over all hooks and call <see cref="IBinaryMessageHook.MessageSent"/>
    /// </summary>
    public static void MessageSent(this IEnumerable<IBinaryMessageHook> hooks, BinaryMessage message)
    {
        foreach (var hook in hooks)
        {
            hook.MessageSent(message);
        }
    }

    /// <summary>
    /// Iterate over all hooks and call <see cref="IBinaryMessageHook.MessageReceived"/>
    /// </summary>
    public static void MessageReceived(this IEnumerable<IBinaryMessageHook> hooks, BinaryMessage message)
    {
        foreach (var hook in hooks)
        {
            hook.MessageReceived(message);
        }
    }

    /// <summary>
    /// Iterate over all hooks and call <see cref="IBinaryMessageHook.PublishingMessage"/>
    /// </summary>
    public static bool PublishingMessage<T>(this IEnumerable<IBinaryMessageHook> hooks, ref T payload)
        where T : class
    {
        object message = payload;
        var result = hooks.Aggregate(true, (current, hook) => current & hook.PublishingMessage(ref message));
        payload = (T)message;
        return result;
    }
}