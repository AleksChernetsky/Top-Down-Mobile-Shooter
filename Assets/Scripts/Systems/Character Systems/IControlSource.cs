using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public interface IControlSource
    {
        Vector2 MoveDirection { get; }
        bool IsFiring { get; }
        void SwitchWeapon() { }
    }

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

    public class BotControlSource : IControlSource
    {
        private readonly Transform _self;
        private readonly Transform _target;

        public BotControlSource(Transform self, Transform target)
        {
            _self = self;
            _target = target;
        }

        public Vector2 MoveDirection
        {
            get
            {
                if (_target == null)
                    return Vector2.zero;

                Vector3 dir = (_target.position - _self.position);
                return new Vector2(dir.x, dir.z).normalized;
            }
        }

        public bool IsFiring => false; // TODO: Implement firing logic for bot
        public void SwitchWeapon() { }
    }
}