using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public class EmptyControlSource : IControlSource
    {
        public Vector2 MoveDirection => Vector2.zero;
        public bool IsFiring => false;

        public CombatTarget CurrentTarget => throw new System.NotImplementedException();

        public void SwitchWeapon() { }
        public void Disable() { }
    }
}