using System;
using UnityEngine;

namespace TowerDefence.Data
{
    [Serializable]
    public class ConfigEntry
    {
        [SerializeField] private string _key;
        [SerializeField] private ScriptableObject _config;

        public string Key => _key;
        public ScriptableObject Config => _config;
    }
}

