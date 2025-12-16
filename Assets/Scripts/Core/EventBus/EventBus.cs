using System;
using System.Collections.Generic;

namespace TowerDefence.Core
{
    public sealed class EventBus : IEventBus
    {
        private class EventToken : IEventToken
        {
            public Type EventType { get; }
            public Delegate Handler { get; }

            public EventToken(Type eventType, Delegate handler)
            {
                EventType = eventType;
                Handler = handler;
            }
        }

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();
        private readonly List<Delegate> _invokeBuffer = new List<Delegate>();

        public void Init()
        {
            Clear();
        }

        public IEventToken Subscribe<T>(Action<T> handler) where T : struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var eventType = typeof(T);
            if (!_subscribers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Delegate>();
                _subscribers[eventType] = handlers;
            }

            handlers.Add(handler);
            return new EventToken(eventType, handler);
        }

        public void Unsubscribe(IEventToken token)
        {
            if (token == null)
            {
                return;
            }

            if (!_subscribers.TryGetValue(token.EventType, out var handlers))
            {
                return;
            }

            if (token is EventToken eventToken)
            {
                handlers.Remove(eventToken.Handler);
            }
        }

        public void Publish<T>(T eventData) where T : struct
        {
            var eventType = typeof(T);
            if (!_subscribers.TryGetValue(eventType, out var handlers) || handlers.Count == 0)
            {
                return;
            }

            _invokeBuffer.Clear();
            _invokeBuffer.AddRange(handlers);

            foreach (var handler in _invokeBuffer)
            {
                try
                {
                    ((Action<T>)handler).Invoke(eventData);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error invoking event handler for {eventType.Name}: {ex}");
                }
            }

            _invokeBuffer.Clear();
        }

        public void Clear()
        {
            _subscribers.Clear();
            _invokeBuffer.Clear();
        }
    }
}
