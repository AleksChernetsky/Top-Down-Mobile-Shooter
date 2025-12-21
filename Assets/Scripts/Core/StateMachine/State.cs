using TowerDefence.Systems;

namespace TowerDefence.Core
{
    public abstract class State : IState
    {
        protected readonly StateMachine _stateMachine;

        protected State(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void OnEnter() { }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnExit() { }
    }

    public abstract class BaseState : State
    {
        protected readonly CharacterContext Character;

        protected BaseState(StateMachine stateMachine, CharacterContext character) : base(stateMachine)
        {
            Character = character;
        }

        protected bool IsDead => Character.Vitality.IsDead;
    }

    public class IdleState : BaseState
    {
        public IdleState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character) { }

        public override void Tick(float deltaTime)
        {
            Character.Animation.Tick();

            if (Character.Control.IsFiring)
            {
                _stateMachine.Enter<AttackState>();
                return;
            }

            if (Character.Control.MoveDirection.sqrMagnitude > 0.01f)
            {
                _stateMachine.Enter<MoveState>();
            }
        }
    }

    public class MoveState : BaseState
    {
        public MoveState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character) { }

        public override void Tick(float deltaTime)
        {
            Character.Animation.Tick();
            Character.Movement.Move(Character.Control.MoveDirection);
            Character.Movement.Rotate(deltaTime);

            if (Character.Control.IsFiring)
            {
                _stateMachine.Enter<AttackState>();
                return;
            }

            if (Character.Control.MoveDirection.sqrMagnitude < 0.01f)
            {
                _stateMachine.Enter<IdleState>();
            }
        }
    }

    public class AttackState : BaseState
    {
        public AttackState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character) { }

        public override void Tick(float deltaTime)
        {
            CombatTarget target = Character.Control.CurrentTarget;

            if (!Character.Control.IsFiring || !target.IsValid)
            {
                if (Character.Control.MoveDirection.sqrMagnitude > 0.01f)
                    _stateMachine.Enter<MoveState>();
                else
                    _stateMachine.Enter<IdleState>();
                return;
            }

            Character.Movement.Move(Character.Control.MoveDirection);
            Character.Movement.Rotate(deltaTime, target.Transform.position - Character.Body.position);

            Character.Animation.Tick();
            Character.Animation.UpdateCombatLayer(isFiring: true);

            Character.Weapon.StartAttacking(target.Transform, Character.Identity);
        }

        public override void OnExit()
        {
            Character.Weapon.StopAttacking();
            Character.Animation.UpdateCombatLayer(isFiring: false);
        }
    }

    public class DeathState : BaseState
    {
        public DeathState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character) { }

        public override void OnEnter()
        {
            Character.Movement.Stop();
            Character.Control.Disable();

            Character.Animation.PlayDeathAnim();
        }
    }

    public class EndGameState : BaseState
    {
        public EndGameState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character) { }

        public override void OnEnter()
        {
            Character.Movement.Stop();
            Character.Control.Disable();
            Character.Animation.StopAnimations();
        }
    }
}