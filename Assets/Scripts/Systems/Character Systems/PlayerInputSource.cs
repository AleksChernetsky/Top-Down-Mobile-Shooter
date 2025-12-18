using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public class PlayerInputSource : IControlSource
    {
        private readonly IInputService _input;

        public PlayerInputSource(IInputService input)
        {
            _input = input;
            _input.OnWeaponSwitch += SwitchWeapon;
        }

        public Vector2 MoveDirection => _input.GetMoveDirection();
        public bool IsFiring => _input.IsFiring;
        public void SwitchWeapon()
        {
            Debug.Log("Player switched weapon");
        }
    }
}