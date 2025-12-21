using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public interface IControlSource
    {
        CombatTarget CurrentTarget { get; }
        Vector2 MoveDirection { get; }
        bool IsFiring { get; }
        void SwitchWeapon() { }
        void Disable() { }
    }
}