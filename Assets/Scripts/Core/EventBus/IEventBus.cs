using System;

namespace TowerDefence.Core
{
    /// <summary>
    /// Decoupled event messaging system. Subscribe returns token for safe cleanup.
    /// </summary>
    public interface IEventBus : IService
    {
        IEventToken Subscribe<T>(Action<T> handler) where T : struct;
        void Unsubscribe(IEventToken token);
        void Publish<T>(T eventData) where T : struct;
        void Clear();
    }
}
