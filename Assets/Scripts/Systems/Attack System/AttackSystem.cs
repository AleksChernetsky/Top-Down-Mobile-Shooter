using TowerDefence.Combat;
using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Systems
{
    public interface IAttackSystem
    {
        void PerformAttack(Transform muzzle, Transform target);
    }

    public abstract class AttackSystem : IAttackSystem
    {
        protected readonly WeaponConfig _config;
        protected readonly IObjectPooler _pool;

        protected AttackSystem(WeaponConfig config)
        {
            _config = config;
            _pool = Services.Get<IObjectPooler>();
        }

        public abstract void PerformAttack(Transform muzzle, Transform target);
    }

    public class RifleAttackSystem : AttackSystem
    {
        public RifleAttackSystem(WeaponConfig config) : base(config) { }

        public override void PerformAttack(Transform muzzle, Transform target)
        {
            var projectile = _pool.Get<Projectile>(_config.PoolKey);

            Vector3 direction = target.position - muzzle.position;
            direction.y = 0;

            Vector3 dir = direction.normalized;
            projectile.Launch(muzzle.position, dir, _config.Damage, _config.PoolKey);
        }
    }

    public class ShotgunAttackSystem : AttackSystem
    {
        public ShotgunAttackSystem(WeaponConfig config) : base(config) { }

        public override void PerformAttack(Transform muzzle, Transform target)
        {
            Vector3 direction = target.position - muzzle.position;
            direction.y = 0;
            Vector3 baseDir = direction.normalized;

            for (int i = 0; i < _config.PelletCount; i++)
            {
                float angle = _config.SpreadAngle * (i - (_config.PelletCount - 1) * 0.5f);
                Vector3 dir = Quaternion.Euler(0f, angle, 0f) * baseDir;

                var projectile = _pool.Get<Projectile>(_config.PoolKey);
                projectile.Launch(muzzle.position, dir, _config.Damage, _config.PoolKey);
            }
        }
    }
}