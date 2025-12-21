using System.Threading;
using System.Threading.Tasks;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Movement
{
    public class BotControlSource : IControlSource
    {
        [Header("Dependencies")]
        private readonly Transform _selfTransform;
        private readonly CharacterIdentity _selfIdentity;
        private readonly ITargetSearchService _searchService;
        private readonly Transform[] _patrolPoints;

        [Header("Detection Settings")]
        private readonly float _detectionRadius;
        private const int SearchDelayMs = 250;
        private const float PatrolRadius = 3f;

        [Header("Combat Distances")]
        private const float CombatMinDistance = 3.0f;
        private const float CombatMaxDistance = 5.0f;

        [Header("Strafe Settings")]
        private const float StrafeChangeInterval = 2.0f;
        private float _strafeDirection = 1f;
        private float _nextStrafeChangeTime;

        [Header("State")]
        private CombatTarget _currentTarget = CombatTarget.None;
        private Vector3 _patrolTargetPos;
        private int _currentPatrolIndex;
        private bool _enabled = true;

        public bool IsFiring => _currentTarget.IsValid;
        public CombatTarget CurrentTarget => _currentTarget;
        public Vector2 MoveDirection
        {
            get
            {
                if (!_enabled)
                    return Vector2.zero;

                return _currentTarget.IsValid ? CalculateCombatMove() : CalculatePatrolMove();
            }
        }

        public BotControlSource(Transform selfTransform, CharacterIdentity selfIdentity, Transform[] patrolPoints,
            CancellationToken token, float detectionRadius)
        {
            _selfTransform = selfTransform;
            _selfIdentity = selfIdentity;
            _patrolPoints = patrolPoints;
            _detectionRadius = detectionRadius;

            _searchService = Services.Get<ITargetSearchService>();

            _currentPatrolIndex = 0;
            UpdatePatrolPoint();

            _ = RunBotLogic(token);
        }

        public void Disable() => _enabled = false;

        private async Task RunBotLogic(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_enabled)
                    {
                        FindAndSetTarget();
                    }
                    await Task.Delay(SearchDelayMs, token);
                }
            }
            catch (TaskCanceledException) { }
        }

        private void FindAndSetTarget()
        {
            if (_currentTarget.IsValid)
            {
                float dist = Vector3.Distance(_selfTransform.position, _currentTarget.Transform.position);
                if (dist <= _detectionRadius)
                {
                    return;
                }
            }

            Transform bestTarget = _searchService.Target(_selfIdentity, _detectionRadius);

            if (bestTarget != null)
            {
                _currentTarget = new CombatTarget
                {
                    Transform = bestTarget,
                    Vitality = bestTarget.GetComponent<IVitalitySystem>(),
                    Identity = bestTarget.GetComponent<IIdentity>()
                };
            }
            else
            {
                _currentTarget = CombatTarget.None;
            }
        }

        private Vector2 CalculateCombatMove()
        {
            Vector3 toEnemy = _currentTarget.Transform.position - _selfTransform.position;
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
                dir = CalculateStrafeDirection(toEnemy);
            }

            return new Vector2(dir.x, dir.z);
        }

        private Vector2 CalculatePatrolMove()
        {
            Vector3 dir = _patrolTargetPos - _selfTransform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 1.0f)
            {
                UpdatePatrolPoint();
                return Vector2.zero;
            }

            return new Vector2(dir.x, dir.z).normalized;
        }

        private Vector3 CalculateStrafeDirection(Vector3 toEnemy)
        {
            if (Time.time > _nextStrafeChangeTime)
            {
                _strafeDirection = Random.value > 0.5f ? 1f : -1f;
                _nextStrafeChangeTime = Time.time + StrafeChangeInterval;
            }

            Vector3 strafeSide = Vector3.Cross(Vector3.up, toEnemy.normalized);

            return strafeSide * _strafeDirection;
        }

        private void UpdatePatrolPoint()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
                return;

            Vector2 offset = Random.insideUnitCircle * PatrolRadius;

            _patrolTargetPos = _patrolPoints[_currentPatrolIndex].position + new Vector3(offset.x, 0f, offset.y);

            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
        }
    }
}