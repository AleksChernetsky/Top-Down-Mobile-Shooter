using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefence.Movement
{
    public enum ControlType { Player, Bot }

    public class CharacterHandler : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _body;
        [SerializeField] private ControlType _controlType;

        private int _damage = 10;
        private Transform _target;

        private IControlSource _control;
        private IMovementService _moveService;

        private VitalitySystem _vitSystem;
        private AnimationService _animService;
        private StateMachine _stateMachine;

        private void Awake()
        {
            var agent = GetComponent<NavMeshAgent>();
            _vitSystem = GetComponent<VitalitySystem>();
            _vitSystem.Init();

            _moveService = new MovementService(agent, _body);
            _animService = new AnimationService(agent, _animator, _body);

            _control = _controlType == ControlType.Player
                ? new PlayerInputSource(Services.Get<IInputService>())
                : new BotControlSource(transform, _target);

            var context = new CharacterContext(_control, _moveService, _vitSystem, _animService, _body, _damage);
            _stateMachine = new StateMachine();
            _stateMachine.SetState(new IdleState(_stateMachine, context));
        }

        private void Update()
        {
            _stateMachine.Tick(Time.deltaTime);
        }
    }

    public class CharacterContext
    {
        public IControlSource Control { get; }
        public IMovementService Movement { get; }
        public IVitalitySystem Vitality { get; }
        public AnimationService Animation { get; }
        public Transform Transform { get; }
        public int Damage { get; }

        public CharacterContext(
            IControlSource control,
            IMovementService movement,
            IVitalitySystem vitality,
            AnimationService animation,
            Transform body,
            int damage)
        {
            Control = control;
            Movement = movement;
            Vitality = vitality;
            Animation = animation;
            Transform = body;
            Damage = damage;
        }
    }
}