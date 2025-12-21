using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Movement
{
    public interface IMovementService
    {
        Vector3 Position { get; }

        void Move(Vector2 direction);
        void Stop();
        void Rotate(float deltaTime, Vector3? direction = null);
    }

    public class MovementService : IMovementService
    {
        private readonly NavMeshAgent _agent;
        private readonly Transform _body;

        private readonly float _rotationSpeed = 720f;

        public Vector3 Position => _body.position;

        public MovementService(NavMeshAgent agent, Transform body)
        {
            _agent = agent;
            _body = body;

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        public void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.01f)
            {
                Stop();
                return;
            }

            _agent.isStopped = false;
            Vector3 offset = new Vector3(direction.x, 0f, direction.y).normalized;
            Vector3 target = _body.position + offset;

            _agent.SetDestination(target);
        }

        public void Stop()
        {
            if (_agent == null || !_agent.isActiveAndEnabled)
                return;

            _agent.isStopped = true;
            _agent.ResetPath();
        }

        public void Rotate(float deltaTime, Vector3? direction = null)
        {
            Vector3 dir = direction ?? _agent.velocity;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            _body.rotation = Quaternion.RotateTowards(
                _body.rotation,
                targetRotation,
                _rotationSpeed * deltaTime
            );
        }
    }
}