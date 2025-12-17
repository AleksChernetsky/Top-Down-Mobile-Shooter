using TowerDefence.Combat;
using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Systems
{
    public interface IAttackSystem
    {
        bool CanAttack { get; }
        void Tick(float deltaTime);
        void TryAttack(Transform muzzle, Transform attacker, Transform target);
    }

    public abstract class AttackSystem : IAttackSystem
    {
        protected readonly WeaponConfig _config;
        protected readonly IObjectPooler _pool;

        protected float _timer;

        public bool CanAttack => _timer <= 0f;

        protected AttackSystem(WeaponConfig config)
        {
            _config = config;
            _pool = Services.Get<IObjectPooler>();
        }

        public virtual void Tick(float deltaTime)
        {
            if (_timer > 0f)
                _timer -= deltaTime;
        }

        public void TryAttack(Transform muzzle, Transform attacker, Transform target)
        {
            if (!CanAttack || !IsTargetValid(target))
                return;

            PerformAttack(muzzle, attacker, target);
            _timer = _config.Cooldown;
        }

        protected bool IsTargetValid(Transform target)
        {
            if (target == null)
                return false;

            var vitality = target.GetComponent<IVitalitySystem>();
            return vitality != null && !vitality.IsDead;
        }

        protected abstract void PerformAttack(Transform muzzle, Transform attacker, Transform target);
    }

    public class RifleAttackSystem : AttackSystem
    {
        public RifleAttackSystem(WeaponConfig config) : base(config) { }

        protected override void PerformAttack(Transform muzzle, Transform attacker, Transform target)
        {
            var projectile = _pool.Get<Projectile>(_config.PoolKey);

            Vector3 dir = (target.position - attacker.position).normalized;

            projectile.Launch(muzzle.position, dir, _config.Damage, _config.PoolKey);
        }
    }

    public class ShotgunAttackSystem : AttackSystem
    {
        public ShotgunAttackSystem(WeaponConfig config) : base(config) { }

        protected override void PerformAttack(Transform muzzle, Transform attacker, Transform target)
        {
            Vector3 baseDir = (target.position - attacker.position).normalized;

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