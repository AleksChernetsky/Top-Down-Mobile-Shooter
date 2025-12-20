using TowerDefence.Core;
using TowerDefence.Game;
using TowerDefence.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Systems
{
    public enum ControlType { Player, Bot, Test }

    public class CharacterHandler : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private GameObject _cameraObject;

        [Header("Base Settings")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _weaponHand;

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

        private IEventToken _gameOverToken;

        private void Awake()
        {
            GetComponents();
        }

        public void Initialize(ControlType controlType, Transform[] patrolPoints)
        {
            InitServices();
            SetupCamera(controlType);

            _vitalitySystem.OnDeath += () => OnDeath();
            _gameOverToken = Services.Get<IEventBus>().Subscribe<GameOverEvent>(OnEndGame);

            _control = controlType switch
            {
                ControlType.Player => new PlayerInputSource(Services.Get<IInputService>(), transform),
                ControlType.Bot => new BotControlSource(transform, _identity, patrolPoints, destroyCancellationToken, _currentWeapon.Config.Range),
                _ => new EmptyControlSource(),
            };

            var character = new CharacterContext(_currentWeapon, _control, _movementService, _vitalitySystem, _animService, _body, _identity);

            InitStateMachine(character);
        }

        private void Update()
        {
            _stateMachine?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (_gameOverToken != null && Services.TryGet<IEventBus>(out var bus))
            {
                bus.Unsubscribe(_gameOverToken);
            }
        }

        private void InitStateMachine(CharacterContext character)
        {
            _stateMachine = new StateMachine();

            var idleState = new IdleState(_stateMachine, character);
            var moveState = new MoveState(_stateMachine, character);
            var attackState = new AttackState(_stateMachine, character);
            var deathState = new DeathState(_stateMachine, character);
            var endGameState = new EndGameState(_stateMachine, character);

            _stateMachine.RegisterState(idleState);
            _stateMachine.RegisterState(moveState);
            _stateMachine.RegisterState(attackState);
            _stateMachine.RegisterState(deathState);
            _stateMachine.RegisterState(endGameState);

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

        private void OnDeath()
        {
            _stateMachine.Enter<DeathState>();
        }
        private void OnEndGame(GameOverEvent evt)
        {
            if (this == null || gameObject == null)
                return;

            _stateMachine.Enter<EndGameState>();
        }

        private void SetupCamera(ControlType type)
        {
            _cameraObject.SetActive(type == ControlType.Player);
        }

        public CharacterIdentity GetIdentity()
        {
            return _identity;
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