using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Core
{
    public sealed class TickDispatcher : MonoBehaviour, ITickDispatcher
    {
        private readonly List<Action<float>> _updateListeners = new List<Action<float>>();
        private readonly List<Action<float>> _fixedUpdateListeners = new List<Action<float>>();
        private readonly List<Action<float>> _lateUpdateListeners = new List<Action<float>>();

        private readonly List<Action<float>> _invokeBuffer = new List<Action<float>>();

        public void Init()
        {
            Clear();
        }

        public void Subscribe(Action<float> onUpdate, TickType tickType = TickType.Update)
        {
            if (onUpdate == null)
            {
                return;
            }

            var listeners = GetListenerList(tickType);
            if (!listeners.Contains(onUpdate))
            {
                listeners.Add(onUpdate);
            }
        }

        public void Unsubscribe(Action<float> onUpdate, TickType tickType = TickType.Update)
        {
            if (onUpdate == null)
            {
                return;
            }

            var listeners = GetListenerList(tickType);
            listeners.Remove(onUpdate);
        }

        public void Clear()
        {
            _updateListeners.Clear();
            _fixedUpdateListeners.Clear();
            _lateUpdateListeners.Clear();
        }

        private void Update() => InvokeListeners(_updateListeners, Time.deltaTime);

        private void FixedUpdate() => InvokeListeners(_fixedUpdateListeners, Time.fixedDeltaTime);

        private void LateUpdate() => InvokeListeners(_lateUpdateListeners, Time.deltaTime);

        private void InvokeListeners(List<Action<float>> listeners, float deltaTime)
        {
            if (listeners.Count == 0)
            {
                return;
            }

            _invokeBuffer.Clear();
            _invokeBuffer.AddRange(listeners);

            foreach (var listener in _invokeBuffer)
            {
                try
                {
                    listener?.Invoke(deltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error in tick listener: {ex}");
                }
            }

            _invokeBuffer.Clear();
        }

        private List<Action<float>> GetListenerList(TickType tickType)
        {
            return tickType switch
            {
                TickType.Update => _updateListeners,
                TickType.FixedUpdate => _fixedUpdateListeners,
                TickType.LateUpdate => _lateUpdateListeners,
                _ => _updateListeners
            };
        }
    }
}
