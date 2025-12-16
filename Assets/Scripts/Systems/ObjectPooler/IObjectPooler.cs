using System;
using TowerDefence.Core;

namespace TowerDefence.Systems
{
    public interface IObjectPooler : IService
    {
        void CreatePool<T>(string key, Func<T> factory, Action<T> onGet = null, Action<T> onRelease = null, int prewarmCount = 0) where T : class;
        T Get<T>(string key) where T : class;
        void Release<T>(string key, T instance) where T : class;
        void Clear(string key);
        void ClearAll();
    }
}
