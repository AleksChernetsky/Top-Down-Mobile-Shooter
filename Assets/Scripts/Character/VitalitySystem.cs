using UnityEngine;

namespace TowerDefence.Movement
{
    public interface IVitalitySystem
    {
        bool IsDead { get; }
        void TakeDamage(int amount);
    }

    public class VitalitySystem : MonoBehaviour, IVitalitySystem
    {
        [SerializeField] private int _maxHealth = 100;
        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        public void Init()
        {
            _currentHealth = _maxHealth;
        }
        public void TakeDamage(int amount)
        {
            Debug.Log($"[{gameObject.name}] Taking damage: {amount}");

            _currentHealth -= amount;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Destroy(gameObject);
            }
        }
    }
}