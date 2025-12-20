using UnityEngine;

namespace TowerDefence.Movement
{
    public interface IControlSource
    {
        Vector2 MoveDirection { get; }
        bool IsFiring { get; }
        void SwitchWeapon() { }
        void Disable() { }
    }
}