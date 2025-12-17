using TowerDefence.Core;
using TowerDefence.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Systems
{
    public enum ControlType { Player, Bot }

    public class CharacterHandler : MonoBehaviour
    {
        [SerializeField] private ControlType _controlType;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _weaponHand;

        private Transform _target;

        [Header("Components")]
        private NavMeshAgent _agent;
        private VitalitySystem _vitalitySystem;
        private CharacterIdentity _identity;

        [Header("Services")]
        private IControlSource _control;
        private IMovementService _movementService;
        private AnimationService _animService;
        private StateMachine _stateMachine;
        private Weapon _currentWeapon;

        private void Awake()
        {
            GetComponents();
            InitServices();

            _vitalitySystem.OnDeath += () => _stateMachine.Enter<DeathState>();

            _control = _controlType == ControlType.Player
                ? new PlayerInputSource(Services.Get<IInputService>())
                : new BotControlSource(transform, _target);

            _currentWeapon = _weaponHand.GetComponentInChildren<Weapon>();

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
    }

    public class CharacterContext
    {
        public Weapon Weapon { get; private set; }
        public IControlSource Control { get; }
        public IMovementService Movement { get; }
        public IVitalitySystem Vitality { get; }
        public AnimationService Animation { get; }
        public Transform Body { get; }
        public IIdentity Identity { get; }

        public CharacterContext(
            Weapon weapon,
            IControlSource control,
            IMovementService movement,
            IVitalitySystem vitality,
            AnimationService animation,
            Transform body,
            IIdentity identity)
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