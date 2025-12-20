using System;
using TowerDefence.Core;
using TowerDefence.Game;
using UnityEngine;

namespace TowerDefence.Systems
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
        public event Action OnDeath;

        private void Start()
        {
            _currentHealth = _maxHealth;
        }
        public void TakeDamage(int amount)
        {
            if (IsDead)
                return;

            _currentHealth -= amount;

            if (_currentHealth > 0)
                return;

            _currentHealth = 0;

            OnDeath?.Invoke();

            Services.Get<IEventBus>().Publish(new CharacterDied
            {
                Character = gameObject
            });
        }
    }
}