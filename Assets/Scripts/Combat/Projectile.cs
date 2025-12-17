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
        private int _damage;
        private float _speed = 20f;
        private Vector3 _direction;
        private string _projectileType;

        public void Launch(Vector3 position, Vector3 direction, int damage, string projectileType)
        {
            transform.position = position;
            _direction = direction.normalized;
            _damage = damage;
            _projectileType = projectileType;

            gameObject.SetActive(true);
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<IVitalitySystem>(out var vitality))
            {
                vitality.TakeDamage(_damage);
            }

            Services.Get<IObjectPooler>().Release(_projectileType, this);
            gameObject.SetActive(false);
        }
    }
}