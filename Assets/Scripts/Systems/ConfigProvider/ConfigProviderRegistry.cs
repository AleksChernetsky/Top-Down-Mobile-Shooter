using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.Data
{
    /// <summary>
    /// Container for all game configs. Must be in Resources folder.
    /// </summary>
    [CreateAssetMenu(fileName = "ConfigProviderRegistry", menuName = "TowerDefence/Data/Config Provider Registry")]
    public class ConfigProviderRegistry : ScriptableObject
    {
        [SerializeField] private List<ConfigEntry> _configs = new List<ConfigEntry>();

        public IReadOnlyList<ConfigEntry> Configs => _configs;
    }
}

