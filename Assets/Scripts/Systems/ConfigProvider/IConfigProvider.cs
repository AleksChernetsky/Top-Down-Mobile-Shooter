using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Data
{
    public interface IConfigProvider : IService
    {
        void Register<T>(string key, T config) where T : ScriptableObject;
        T Get<T>(string key) where T : ScriptableObject;
        T Get<T>() where T : ScriptableObject;
        bool TryGet<T>(string key, out T config) where T : ScriptableObject;
        bool TryGet<T>(out T config) where T : ScriptableObject;
        void Clear();
    }
}
