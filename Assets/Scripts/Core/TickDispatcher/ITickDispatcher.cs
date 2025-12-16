using System;

namespace TowerDefence.Core
{
    /// <summary>
    /// Centralized update loop. Systems receive Update/FixedUpdate/LateUpdate without MonoBehaviour.
    /// </summary>
    public interface ITickDispatcher : IService
    {
        void Subscribe(Action<float> onUpdate, TickType tickType = TickType.Update);
        void Unsubscribe(Action<float> onUpdate, TickType tickType = TickType.Update);
        void Clear();
    }
}
