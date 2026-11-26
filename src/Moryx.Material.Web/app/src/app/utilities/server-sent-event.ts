import { Observable } from 'rxjs';

export interface ServerSentEventOptions {
  withCredentials?: boolean;
  parseJson?: boolean;
}

export interface ServerSentEventMessage<T = unknown> {
  data: T;
  raw: MessageEvent;
  eventType: string;
  lastEventId?: string;
}

export function fromEventStream<T = unknown>(
  url: string,
  options: ServerSentEventOptions = {},
  event?: string | string[]
): Observable<ServerSentEventMessage<T>> {
  const { withCredentials = false, parseJson = true } = options;
  const events = Array.isArray(event) ? event : event ? [event] : [];

  return new Observable<ServerSentEventMessage<T>>((subscriber) => {
    const es = new EventSource(url, { withCredentials });

    const parse = (text: string) => {
      if (!parseJson) return text;
      try { return JSON.parse(text); } catch { return text; }
    };

    const handler = (ev: MessageEvent) => {
      subscriber.next({
        data: parse(ev.data) as T,
        raw: ev,
        eventType: ev.type,
        lastEventId: (ev as any).lastEventId,
      });
    };

    // Attach listeners
    if (events.length > 0) {
      for (const name of events) {
        if (name === 'message') {
          es.onmessage = handler;
        } else {
          es.addEventListener(name, handler as EventListener);
        }
      }
    } else {
      // default channel only
      es.onmessage = handler;
    }

    es.onerror = (ev: Event) => {
      // Allow auto-reconnect; only error out when the stream is closed
      if (es.readyState === EventSource.CLOSED) {
        subscriber.error(ev);
      }
    };

    // Teardown
    return () => {
      if (events.length > 0) {
        for (const name of events) {
          if (name === 'message') {
            es.onmessage = null;
          }
          else {
            es.removeEventListener(name, handler as EventListener);
          }
        }
      } else {
        es.onmessage = null;
      }
      es.onerror = null;
      es.close();
    };
  });
}