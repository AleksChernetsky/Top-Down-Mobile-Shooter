using System.Collections;
using TowerDefence.Combat;
using UnityEngine;

namespace TowerDefence.Systems
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponConfig _config;
        [SerializeField] private Transform _muzzleTransform;

        private IAttackSystem _attackSystem;
        private Coroutine _attackCoroutine;
        private Transform _currentTarget;

        public WeaponConfig Config => _config;

        private void Awake()
        {
            _attackSystem = AttackSystemFactory.Create(_config);
        }

        public void StartAttacking(Transform target)
        {
            _currentTarget = target;

            if (_attackCoroutine != null)
                return;

            _attackCoroutine = StartCoroutine(AttackRoutine());
        }

        public void StopAttacking()
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
            _currentTarget = null;
        }

        private IEnumerator AttackRoutine()
        {
            while (true)
            {
                if (_currentTarget == null)
                {
                    StopAttacking();
                    yield break;
                }

                _attackSystem.PerformAttack(_muzzleTransform, _currentTarget);
                yield return new WaitForSeconds(_config.Cooldown);
            }
        }
    }
}