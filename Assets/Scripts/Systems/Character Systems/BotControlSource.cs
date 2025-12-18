using System.Threading;
using System.Threading.Tasks;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Movement
{
    public class BotControlSource : IControlSource
    {
        private readonly Transform _selfTransform;
        private readonly CharacterIdentity _selfIdentity;
        private readonly ITargetSearchService _searchService;

        private readonly float _wanderRadius;
        private readonly float _detectionRadius;

        private Vector3 _currentTargetPos;
        private bool _isFiring;

        private const int SearchDelayMs = 500;

        public bool IsFiring => _isFiring;
        public Vector2 MoveDirection
        {
            get
            {
                Vector3 direction = _currentTargetPos - _selfTransform.position;

                if (direction.sqrMagnitude < 1f)
                {
                    SetNewRandomDestination();
                    return Vector2.zero;
                }

                return new Vector2(direction.x, direction.z).normalized;
            }
        }

        public BotControlSource(Transform selfTransform, CharacterIdentity selfIdentity, CancellationToken token, float wanderRadius = 10f, float detectionRadius = 15f)
        {
            _selfTransform = selfTransform;
            _selfIdentity = selfIdentity;

            _wanderRadius = wanderRadius;
            _detectionRadius = detectionRadius;

            _searchService = Services.Get<ITargetSearchService>();

            SetNewRandomDestination();
            _ = RunBotLogic(token);
        }

        private async Task RunBotLogic(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Transform target = _searchService.Target(_selfIdentity, _detectionRadius);
                    _isFiring = target != null;

                    await Task.Delay(SearchDelayMs, token);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, exit gracefully
            }
        }

        public void SwitchWeapon() { }

        private void SetNewRandomDestination()
        {
            Vector3 randomPoint = _selfTransform.position + Random.insideUnitSphere * _wanderRadius;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _wanderRadius * 1.5f, NavMesh.AllAreas))
            {
                _currentTargetPos = hit.position;
            }
            else
            {
                _currentTargetPos = _selfTransform.position;
            }
        }
    }
}