using TowerDefence.Combat;
using UnityEngine;

namespace TowerDefence.Systems
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponConfig _config;
        [SerializeField] private Transform _muzzleTransform;

        private IAttackSystem _attackSystem;

        public IAttackSystem AttackSystem => _attackSystem;

        private void Awake()
        {
            _attackSystem = AttackSystemFactory.Create(_config);
        }

        public void Tick(float deltaTime)
        {
            _attackSystem.Tick(deltaTime);
        }

        public void Attack(Transform owner, Transform target)
        {
            _attackSystem.TryAttack(_muzzleTransform, owner, target);
        }
    }
}