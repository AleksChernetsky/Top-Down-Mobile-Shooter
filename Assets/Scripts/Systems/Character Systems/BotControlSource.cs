using System.Threading;
using System.Threading.Tasks;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public class BotControlSource : IControlSource
    {
        private enum BotMoveMode { Patrol, Combat }

        private readonly Transform _selfTransform;
        private readonly CharacterIdentity _selfIdentity;
        private readonly ITargetSearchService _searchService;

        private readonly float _detectionRadius;

        private Vector3 _currentTargetPos;
        private bool _isFiring;
        private bool _enabled = true;

        private const float CombatMinDistance = 3.0f;
        private const float CombatMaxDistance = 5.0f;
        private const int SearchDelayMs = 500;

        private readonly Transform[] _patrolPoints;
        private Transform _currentEnemy;
        private BotMoveMode _mode;

        private int _currentPatrolIndex;

        public bool IsFiring => _isFiring;
        public Vector2 MoveDirection
        {
            get
            {
                if (!_enabled)
                    return Vector2.zero;

                return _mode switch
                {
                    BotMoveMode.Patrol => PatrolMove(),
                    BotMoveMode.Combat => CombatMove(),
                    _ => Vector2.zero
                };
            }
        }

        public BotControlSource(Transform selfTransform, CharacterIdentity selfIdentity, Transform[] patrolPoints,
            CancellationToken token, float detectionRadius = 15f)
        {
            _selfTransform = selfTransform;
            _selfIdentity = selfIdentity;
            _patrolPoints = patrolPoints;

            _detectionRadius = detectionRadius;

            _searchService = Services.Get<ITargetSearchService>();

            _mode = BotMoveMode.Patrol;
            _currentPatrolIndex = GetClosestPatrolIndex();
            SetNextPatrolPoint();

            _ = RunBotLogic(token);
        }

        public void SwitchWeapon()
        {
            // TODO: Implement weapon switching logic
        }

        public void Disable()
        {
            _enabled = false;
        }

        private async Task RunBotLogic(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Transform enemy = _searchService.Target(_selfIdentity, _detectionRadius);

                    if (enemy != null)
                    {
                        _currentEnemy = enemy;
                        _isFiring = true;
                        _mode = BotMoveMode.Combat;
                    }
                    else
                    {
                        _currentEnemy = null;
                        _isFiring = false;
                        _mode = BotMoveMode.Patrol;
                    }

                    await Task.Delay(SearchDelayMs, token);
                }
            }
            catch (TaskCanceledException) { }
        }

        private Vector2 PatrolMove()
        {
            Vector3 dir = _currentTargetPos - _selfTransform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 1.5f)
            {
                SetNextPatrolPoint();
                return Vector2.zero;
            }

            return new Vector2(dir.x, dir.z).normalized;
        }
        private Vector2 CombatMove()
        {
            if (_currentEnemy == null)
                return Vector2.zero;

            Vector3 toEnemy = _currentEnemy.position - _selfTransform.position;
            toEnemy.y = 0f;

            float distance = toEnemy.magnitude;
            Vector3 dir = Vector3.zero;

            if (distance < CombatMinDistance)
            {
                dir = -toEnemy.normalized;
            }
            else if (distance > CombatMaxDistance)
            {
                dir = toEnemy.normalized;
            }

            else
            {
                Vector3 strafe = Vector3.Cross(Vector3.up, toEnemy).normalized;
                float side = Random.value > 0.5f ? 1f : -1f;
                dir = strafe * side;
            }

            return new Vector2(dir.x, dir.z);
        }

        private void SetNextPatrolPoint()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
                return;

            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            _currentTargetPos = _patrolPoints[_currentPatrolIndex].position;
        }

        private int GetClosestPatrolIndex()
        {
            int bestIndex = 0;
            float bestDist = float.MaxValue;

            for (int i = 0; i < _patrolPoints.Length; i++)
            {
                float d = (_patrolPoints[i].position - _selfTransform.position).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
    }
}