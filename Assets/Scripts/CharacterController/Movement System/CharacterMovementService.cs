using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Movement
{
    public class CharacterMovementService : IMovementService
    {
        private readonly NavMeshAgent _agent;
        private readonly Transform _transform;

        private readonly float _rotationSpeed = 720f;

        public CharacterMovementService(NavMeshAgent agent, Transform transform)
        {
            _agent = agent;
            _transform = transform;

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        public void SetDestination(Vector3 worldPosition)
        {
            if (!_agent.enabled)
                return;

            _agent.isStopped = false;
            _agent.SetDestination(worldPosition);
        }

        public void Stop()
        {
            if (!_agent.enabled)
                return;

            _agent.isStopped = true;
            _agent.ResetPath();
        }

        public void Tick(float deltaTime)
        {
            RotateTowardsVelocity(deltaTime);
        }

        private void RotateTowardsVelocity(float deltaTime)
        {
            Vector3 velocity = _agent.velocity;
            velocity.y = 0f;

            if (velocity.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, _rotationSpeed * deltaTime);
        }
    }
}