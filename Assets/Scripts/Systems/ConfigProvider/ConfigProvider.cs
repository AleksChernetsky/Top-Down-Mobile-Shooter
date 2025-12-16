using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Data
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly Dictionary<string, ScriptableObject> _configsByKey = new Dictionary<string, ScriptableObject>();
        private readonly Dictionary<Type, ScriptableObject> _configsByType = new Dictionary<Type, ScriptableObject>();
        private ConfigProviderRegistry _registry;

        public void Init()
        {
            _registry = Resources.Load<ConfigProviderRegistry>("ConfigProviderRegistry");
            if (_registry == null)
            {
                Debug.LogWarning("ConfigProviderRegistry not found in Resources folder.");
                return;
            }

            foreach (var entry in _registry.Configs)
            {
                if (entry.Config != null)
                {
                    Register(entry.Key, entry.Config);
                }
            }
        }

        public void Register<T>(string key, T config) where T : ScriptableObject
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            _configsByKey[key] = config;

            var type = typeof(T);
            _configsByType.TryAdd(type, config);
        }

        public T Get<T>(string key) where T : ScriptableObject
        {
            if (_configsByKey.TryGetValue(key, out var config))
            {
                return config as T;
            }

            throw new InvalidOperationException($"Config with key '{key}' not found.");
        }

        public T Get<T>() where T : ScriptableObject
        {
            var type = typeof(T);
            if (_configsByType.TryGetValue(type, out var config))
            {
                return config as T;
            }

            throw new InvalidOperationException($"Config of type '{type.Name}' not found.");
        }

        public bool TryGet<T>(string key, out T config) where T : ScriptableObject
        {
            if (_configsByKey.TryGetValue(key, out var obj))
            {
                config = obj as T;
                return config != null;
            }

            config = null;
            return false;
        }

        public bool TryGet<T>(out T config) where T : ScriptableObject
        {
            var type = typeof(T);
            if (_configsByType.TryGetValue(type, out var obj))
            {
                config = obj as T;
                return config != null;
            }

            config = null;
            return false;
        }

        public void Clear()
        {
            _configsByKey.Clear();
            _configsByType.Clear();
        }
    }
}
