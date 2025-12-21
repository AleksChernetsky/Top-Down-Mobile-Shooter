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

            Vector3 dir = new Vector3(direction.x, 0f, direction.y).normalized;
            Vector3 rawTarget = _body.position + dir;

            if (!NavMesh.SamplePosition(rawTarget, out var hit, 0.5f, NavMesh.AllAreas))
                return;

            _agent.isStopped = false;

            if (_agent.pathPending)
                return;

            if (!_agent.hasPath || _agent.remainingDistance < 0.2f)
                _agent.SetDestination(hit.position);
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