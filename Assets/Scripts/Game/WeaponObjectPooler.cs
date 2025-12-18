using System.Collections.Generic;
using TowerDefence.Combat;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Game
{
    public sealed class WeaponObjectPooler : MonoBehaviour
    {
        [SerializeField] private List<WeaponConfig> weaponConfigs = new List<WeaponConfig>();
        [SerializeField] private int _prewarmCount = 20;

        private void Awake()
        {
            var pooler = Services.Get<IObjectPooler>();
            foreach (var config in weaponConfigs)
            {
                pooler.CreatePool(
                    key: config.PoolKey,
                    factory: () =>
                    {
                        var go = Instantiate(config.ProjectilePrefab);
                        go.gameObject.SetActive(false);
                        return go;
                    },
                    onGet: null,
                    onRelease: p => p.gameObject.SetActive(false),
                    prewarmCount: _prewarmCount
                );
            }
        }
    }
}
