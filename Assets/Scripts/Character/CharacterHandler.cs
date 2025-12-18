using System;
using TowerDefence.Core;
using TowerDefence.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Systems
{
    public enum ControlType { Player, Bot, Test }

    public class CharacterHandler : MonoBehaviour
    {
        [Header("Base Settings")]
        [SerializeField] private ControlType _controlType;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _weaponHand;
        [Header("Bot Settings")]
        [SerializeField] private float _botDetectionRadius = 20f;

        [Header("Components")]
        private NavMeshAgent _agent;
        private VitalitySystem _vitalitySystem;
        private CharacterIdentity _identity;
        private Weapon _currentWeapon;

        [Header("Services")]
        private IControlSource _control;
        private IMovementService _movementService;
        private AnimationService _animService;
        private StateMachine _stateMachine;

        private void Awake()
        {
            GetComponents();
            InitServices();

            _vitalitySystem.OnDeath += () => _stateMachine.Enter<DeathState>();

            _control = _controlType switch
            {
                ControlType.Player => new PlayerInputSource(Services.Get<IInputService>()),
                ControlType.Bot => new BotControlSource(transform, _identity, destroyCancellationToken, _currentWeapon.Config.Range, _botDetectionRadius),
                _ => throw new ArgumentOutOfRangeException(nameof(_controlType), _controlType, null)
            };

            var character = new CharacterContext(_currentWeapon, _control, _movementService, _vitalitySystem, _animService, _body, _identity);

            InitStateMachine(character);
        }

        private void Update()
        {
            _stateMachine.Tick(Time.deltaTime);
        }

        private void InitStateMachine(CharacterContext character)
        {
            _stateMachine = new StateMachine();

            var idleState = new IdleState(_stateMachine, character);
            var moveState = new MoveState(_stateMachine, character);
            var attackState = new AttackState(_stateMachine, character);
            var deathState = new DeathState(_stateMachine, character);

            _stateMachine.RegisterState(idleState);
            _stateMachine.RegisterState(moveState);
            _stateMachine.RegisterState(attackState);
            _stateMachine.RegisterState(deathState);

            _stateMachine.Enter<IdleState>();
        }

        private void GetComponents()
        {
            _agent = GetComponent<NavMeshAgent>();
            _vitalitySystem = GetComponent<VitalitySystem>();
            _identity = GetComponent<CharacterIdentity>();
            _currentWeapon = _weaponHand.GetComponentInChildren<Weapon>();
        }

        private void InitServices()
        {
            _movementService = new MovementService(_agent, _body);
            _animService = new AnimationService(_agent, _animator, _body);
        }

        private void OnDestroy()
        {
            if (_vitalitySystem != null)
                _vitalitySystem.OnDeath -= () => _stateMachine.Enter<DeathState>();
        }

        private void OnDrawGizmosSelected()
        {
            if (_botDetectionRadius <= 0) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _botDetectionRadius);
        }
    }

    public class CharacterContext
    {
        public Weapon Weapon { get; private set; }
        public IControlSource Control { get; }
        public IMovementService Movement { get; }
        public IVitalitySystem Vitality { get; }
        public AnimationService Animation { get; }
        public Transform Body { get; }
        public CharacterIdentity Identity { get; }

        public CharacterContext(
            Weapon weapon,
            IControlSource control,
            IMovementService movement,
            IVitalitySystem vitality,
            AnimationService animation,
            Transform body,
            CharacterIdentity identity)
        {
            Weapon = weapon;
            Control = control;
            Movement = movement;
            Vitality = vitality;
            Animation = animation;
            Body = body;
            Identity = identity;
        }

        public void SetWeapon(Weapon newWeapon)
        {
            Weapon = newWeapon;
        }
    }
}