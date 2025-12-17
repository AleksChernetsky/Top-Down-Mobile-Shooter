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

        private Transform _target;

        private IControlSource _control;
        private IMovementService _moveService;

        private VitalitySystem _vitSystem;
        private AnimationService _animService;
        private StateMachine _stateMachine;

        private NavMeshAgent _agent;

        private void Awake()
        {
            GetComponents();
            InitServices();

            _control = _controlType == ControlType.Player
                ? new PlayerInputSource(Services.Get<IInputService>())
                : new BotControlSource(transform, _target);

            var character = new CharacterContext(InitWeapons(), _control, _moveService, _vitSystem, _animService, _body);

            _stateMachine = new StateMachine();
            _stateMachine.SetState(new IdleState(_stateMachine, character));

        }

        private void Update()
        {
            _stateMachine.Tick(Time.deltaTime);
        }

        private void GetComponents()
        {
            _agent = GetComponent<NavMeshAgent>();
            _vitSystem = GetComponent<VitalitySystem>();
        }

        private void InitServices()
        {
            _moveService = new MovementService(_agent, _body);
            _animService = new AnimationService(_agent, _animator, _body);
        }

        private IAttackSystem[] InitWeapons()
        {
            var weapons = new IAttackSystem[] {
                new Rifle(damage: 5, cooldown: 0.25f),
                new ShotGun(damage: 10, cooldown: 1.5f)
            };
            return weapons;
        }
    }

    public class CharacterContext
    {
        private readonly IAttackSystem[] _weapons;
        private int _currentWeaponIndex;

        public IControlSource Control { get; }
        public IMovementService Movement { get; }
        public IVitalitySystem Vitality { get; }
        public AnimationService Animation { get; }
        public IAttackSystem Weapon => _weapons[_currentWeaponIndex];
        public Transform Body { get; }

        public CharacterContext(
            IAttackSystem[] weapons,
            IControlSource control,
            IMovementService movement,
            IVitalitySystem vitality,
            AnimationService animation,
            Transform body)
        {
            _weapons = weapons;
            _currentWeaponIndex = 0;
            Control = control;
            Movement = movement;
            Vitality = vitality;
            Animation = animation;
            Body = body;
        }

        public void SwitchWeapon(int index)
        {
            if (index < 0 || index >= _weapons.Length)
                return;

            _currentWeaponIndex = index;
        }
        public void NextWeapon()
        {
            _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Length;
        }
    }
}