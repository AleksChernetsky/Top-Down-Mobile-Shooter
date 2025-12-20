using UnityEngine;

namespace TowerDefence.Movement
{
    public class EmptyControlSource : IControlSource
    {
        public Vector2 MoveDirection => Vector2.zero;
        public bool IsFiring => false;

        public void SwitchWeapon() { }
        public void Disable() { }
    }
}