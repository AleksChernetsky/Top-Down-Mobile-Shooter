using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public class PlayerInputSource : IControlSource
    {
        [Header("Dependencies")]
        private readonly IInputService _input;
        private readonly ITargetSearchService _search;
        private readonly CharacterIdentity _identity;

        [Header("State")]
        private CombatTarget _currentTarget = CombatTarget.None;
        private bool _enabled = true;

        public bool IsFiring => _input.IsFiring;
        public CombatTarget CurrentTarget
        {
            get
            {
                if (IsFiring)
                {
                    UpdateAutoAim();
                }
                return _currentTarget;
            }
        }
        public Vector2 MoveDirection
        {
            get
            {
                if (!_enabled)
                    return Vector2.zero;

                Vector2 inputDir = _input.GetMoveDirection();
                Vector3 worldDirection = _identity.transform.TransformDirection(new Vector3(inputDir.x, 0, inputDir.y));

                return new Vector2(worldDirection.x, worldDirection.z);
            }
        }

        public PlayerInputSource(IInputService input, CharacterIdentity identity)
        {
            _input = input;
            _identity = identity;
            _search = Services.Get<ITargetSearchService>();
        }

        public void Disable() => _enabled = false;

        private void UpdateAutoAim()
        {
            Transform target = _search.Target(_identity, 5f);

            if (target != null && target != _currentTarget.Transform)
            {
                _currentTarget = new CombatTarget
                {
                    Transform = target,
                    Vitality = target.GetComponent<IVitalitySystem>(),
                    Identity = target.GetComponent<IIdentity>()
                };
            }
            else if (target == null)
            {
                _currentTarget = CombatTarget.None;
            }
        }
    }
}