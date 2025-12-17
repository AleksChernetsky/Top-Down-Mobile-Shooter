using TowerDefence.Combat;
using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Systems
{
    public interface IAttackSystem
    {
        bool CanAttack { get; }
        void Tick(float deltaTime);
        void TryAttack(Transform attacker, Transform target);
    }

    public abstract class AttackSystem : IAttackSystem
    {
        protected readonly int _damage;
        protected readonly float _cooldown;
        protected readonly IObjectPooler _pool;

        protected float _timer;

        public bool CanAttack => _timer <= 0f;

        protected AttackSystem(int damage, float cooldown)
        {
            _damage = damage;
            _cooldown = cooldown;
            _pool = Services.Get<IObjectPooler>();
        }

        public virtual void Tick(float deltaTime)
        {
            if (_timer > 0f)
                _timer -= deltaTime;
        }

        public void TryAttack(Transform attacker, Transform target)
        {
            if (!CanAttack || !IsTargetValid(target))
                return;

            PerformAttack(attacker, target);
            _timer = _cooldown;
        }

        protected bool IsTargetValid(Transform target)
        {
            if (target == null)
                return false;

            var vitality = target.GetComponent<IVitalitySystem>();
            return vitality != null && !vitality.IsDead;
        }

        protected abstract void PerformAttack(Transform attacker, Transform target);
    }

    public class Rifle : AttackSystem
    {
        private const string ProjectileKey = "RifleProjectile";

        public Rifle(int damage, float cooldown) : base(damage, cooldown) { }

        protected override void PerformAttack(Transform attacker, Transform target)
        {
            var projectile = _pool.Get<Projectile>(ProjectileKey);

            Vector3 dir = (target.position - attacker.position).normalized;
            Vector3 spawnPos = new Vector3(attacker.position.x, 1.5f, attacker.position.z + 0.5f);

            projectile.Launch(spawnPos, dir, _damage, ProjectileKey);

            Debug.Log($"Rifle fired a projectile towards {target.name}");
        }
    }

    public class ShotGun : AttackSystem
    {
        private const string ProjectileKey = "ShotGunProjectile";
        private const int Count = 3;
        private const float SpreadAngle = 10f;

        public ShotGun(int damage, float cooldown) : base(damage, cooldown) { }

        protected override void PerformAttack(Transform attacker, Transform target)
        {
            Vector3 baseDir = (target.position - attacker.position).normalized;

            for (int i = 0; i < Count; i++)
            {
                float angle = SpreadAngle * (i - 1);
                Vector3 dir = Quaternion.Euler(0f, angle, 0f) * baseDir;

                var projectile = _pool.Get<Projectile>(ProjectileKey);
                projectile.Launch(attacker.position + dir, dir, _damage, ProjectileKey);
            }

            Debug.Log($"ShotGun fired a projectiles towards {target.name}");
        }
    }
}