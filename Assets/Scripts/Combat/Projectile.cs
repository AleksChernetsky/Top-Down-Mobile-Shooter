using System.Collections;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Combat
{
    public interface IProjectile
    {
        void Launch(Vector3 position, Vector3 direction, int damage, string projectileType);
    }

    public class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private ParticleSystem _impactEffect;

        private int _damage;
        private float _speed = 15f;
        private Vector3 _direction;
        private string _projectileType;

        private bool _despawned = true;
        private Coroutine _moveRoutine;
        private float _lifetime = 2f;

        public void Launch(Vector3 position, Vector3 direction, int damage, string projectileType)
        {
            _despawned = false;

            if (_moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
                _moveRoutine = null;
            }

            transform.position = position;
            transform.rotation = Quaternion.identity;

            _direction = direction.normalized;
            _damage = damage;
            _projectileType = projectileType;

            gameObject.SetActive(true);

            _moveRoutine = StartCoroutine(UpdateProjectile());
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IVitalitySystem>(out var vitality))
            {
                vitality.TakeDamage(_damage);
            }
            Instantiate(_impactEffect, transform.position, Quaternion.identity); // TODO: Use object pooling for impact effects
            Despawn();
        }

        private IEnumerator UpdateProjectile()
        {
            var elapsedTime = 0f;
            while (elapsedTime < _lifetime)
            {
                transform.position += _direction * _speed * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Despawn();
        }

        private void Despawn()
        {
            if (_despawned)
                return;

            _despawned = true;

            if (_moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
                _moveRoutine = null;
            }
            gameObject.SetActive(false);
            Services.Get<IObjectPooler>().Release(_projectileType, this);
        }
    }
}