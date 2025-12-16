using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Movement
{
    public class AnimationService
    {
        private readonly NavMeshAgent _agent;
        private readonly Animator _animator;
        private readonly Transform _body;

        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");

        public AnimationService(NavMeshAgent agent, Animator animator, Transform body)
        {
            _agent = agent;
            _animator = animator;
            _body = body;
        }

        public void Tick()
        {
            UpdateMovementAnimation();
        }

        private void UpdateMovementAnimation()
        {
            Vector3 velocity = _agent.velocity;
            velocity.y = 0f;

            if (velocity.sqrMagnitude < 0.01f)
            {
                _animator.SetFloat(MoveX, 0f);
                _animator.SetFloat(MoveY, 0f);
                return;
            }

            Vector3 worldDir = velocity.normalized;
            Vector3 localDir = _body.InverseTransformDirection(worldDir);

            _animator.SetFloat(MoveX, localDir.x);
            _animator.SetFloat(MoveY, localDir.z);
        }
    }
}