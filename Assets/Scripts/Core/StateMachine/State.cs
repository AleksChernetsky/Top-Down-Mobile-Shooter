using TowerDefence.Systems;
using UnityEngine;

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

            if (Character.Control.MoveDirection.sqrMagnitude > 0.01f)
            {
                _stateMachine.Enter<MoveState>();
                return;
            }
            if (Character.Control.IsFiring)
            {
                _stateMachine.Enter<AttackState>();
                return;
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

            if (Character.Control.MoveDirection.sqrMagnitude < 0.01f)
            {
                _stateMachine.Enter<IdleState>();
                return;
            }

            if (Character.Control.IsFiring)
            {
                _stateMachine.Enter<AttackState>();
                return;
            }
        }
    }

    public class AttackState : BaseState
    {
        private readonly ITargetSearchService _search;

        private CombatTarget _target;

        public AttackState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character)
        {
            _search = Services.Get<ITargetSearchService>();
        }

        public override void Tick(float deltaTime)
        {
            if (!Character.Control.IsFiring)
            {
                ChangeStateToMoveOrIdle();
                return;
            }

            Character.Movement.Move(Character.Control.MoveDirection);
            Character.Animation.Tick();

            if (!_target.IsValid || Vector3.Distance(Character.Body.position, _target.Transform.position) > Character.Weapon.Config.Range)
            {
                FindNewTarget();

                if (!_target.IsValid)
                {
                    Character.Weapon.StopAttacking();
                    ChangeStateToMoveOrIdle();
                    return;
                }
            }

            Character.Movement.Rotate(deltaTime, _target.Transform.position - Character.Body.position);
            Character.Animation.UpdateCombatLayer(isFiring: true);
            Character.Weapon.StartAttacking(_target.Transform, Character.Identity);
        }

        public override void OnExit()
        {
            Character.Weapon.StopAttacking();
            _target = default;
            Character.Animation.UpdateCombatLayer(isFiring: false);
        }
        private void ChangeStateToMoveOrIdle()
        {
            if (Character.Control.MoveDirection.sqrMagnitude > 0.01f)
                _stateMachine.Enter<MoveState>();
            else
                _stateMachine.Enter<IdleState>();
        }
        private void FindNewTarget()
        {
            _target = default;

            Transform bestTarget = _search.Target(Character.Identity, Character.Weapon.Config.Range);

            if (bestTarget != null)
            {
                _target = new CombatTarget
                {
                    Transform = bestTarget,
                    Vitality = bestTarget.GetComponent<IVitalitySystem>(),
                    Identity = bestTarget.GetComponent<IIdentity>()
                };
            }
        }
        private struct CombatTarget
        {
            public Transform Transform;
            public IVitalitySystem Vitality;
            public IIdentity Identity;

            public bool IsValid => Transform != null && Vitality != null && !Vitality.IsDead;
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