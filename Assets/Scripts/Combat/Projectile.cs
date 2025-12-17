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
            Debug.Log($"Launching projectile of type {projectileType} from {position} towards {direction} with damage {damage}");

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
                Debug.Log($"Projectile hit {collision.gameObject.name} for {_damage} damage.");
            }
            Debug.Log($"Projectile of type {_projectileType} collided with {collision.gameObject.name} and will be released back to pool.");
            Services.Get<IObjectPooler>().Release(_projectileType, this);

            gameObject.SetActive(false);
        }
    }
}