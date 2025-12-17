using TowerDefence.Combat;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Game
{
    public sealed class BattleEntry : MonoBehaviour
    {
        [SerializeField] private Projectile _rifleProjectilePrefab;
        [SerializeField] private Projectile _shotgunProjectilePrefab;
        [SerializeField] private int _prewarmCount = 20;

        private void Awake()
        {
            var pooler = Services.Get<IObjectPooler>();

            pooler.CreatePool(
                key: "RifleProjectile",
                factory: () =>
                {
                    var go = Instantiate(_rifleProjectilePrefab);
                    go.gameObject.SetActive(false);
                    return go;
                },
                onGet: p => p.gameObject.SetActive(true),
                onRelease: p => p.gameObject.SetActive(false),
                prewarmCount: _prewarmCount
            );

            pooler.CreatePool(
                key: "ShotGunProjectile",
                factory: () =>
                {
                    var go = Instantiate(_shotgunProjectilePrefab);
                    go.gameObject.SetActive(false);
                    return go;
                },
                onGet: p => p.gameObject.SetActive(true),
                onRelease: p => p.gameObject.SetActive(false),
                prewarmCount: _prewarmCount
            );
        }
    }
}
