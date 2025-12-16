using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _body;
        [SerializeField] private Joystick _joystick;

        private IMovementService _movement;
        private CharacterAnimationService _animation;

        private void Awake()
        {
            var agent = GetComponent<NavMeshAgent>();
            _movement = new CharacterMovementService(agent, _body);
            _animation = new CharacterAnimationService(agent, _animator, _body);
        }

        private void Update()
        {
            Vector2 input = _joystick.Direction;
            _animation.Tick();

            if (input.sqrMagnitude < 0.01f)
            {
                _movement.Stop();
                return;
            }

            Vector3 dir = new Vector3(input.x, 0f, input.y);
            Vector3 target = transform.position + dir.normalized;

            _movement.SetDestination(target);
            _movement.Tick(Time.deltaTime);
        }
    }
}