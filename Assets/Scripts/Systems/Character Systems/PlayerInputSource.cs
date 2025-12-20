using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public class PlayerInputSource : IControlSource
    {
        private readonly IInputService _input;
        private readonly Transform _characterTransform;
        private bool _enabled = true;

        public bool IsFiring => _input.IsFiring;
        public Vector2 MoveDirection
        {
            get
            {
                if (!_enabled)
                    return Vector2.zero;

                Vector2 inputDir = _input.GetMoveDirection();
                Vector3 worldDirection = _characterTransform.TransformDirection(new Vector3(inputDir.x, 0, inputDir.y));

                return new Vector2(worldDirection.x, worldDirection.z);
            }
        }
        public PlayerInputSource(IInputService input, Transform characterTransform)
        {
            _input = input;
            _characterTransform = characterTransform;
            _input.OnWeaponSwitch += SwitchWeapon;
        }

        public void SwitchWeapon()
        {
            Debug.Log("Player switched weapon");
            // TODO: Implement weapon switching logic
        }

        public void Disable()
        {
            _enabled = false;
        }
    }
}