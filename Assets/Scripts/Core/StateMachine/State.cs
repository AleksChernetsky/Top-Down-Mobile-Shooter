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

            if (Character.Control.IsFiring)
            {
                _stateMachine.Enter<AttackState>();
                return;
            }

            if (Character.Control.MoveDirection.sqrMagnitude > 0.01f)
            {
                _stateMachine.Enter<MoveState>();
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
        public override void OnExit()
        {
            Character.Movement.Stop();
        }
    }

    public class AttackState : BaseState
    {
        private readonly ITargetSearchService _search;

        private Transform _target;

        private const float AttackRadius = 10f;

        public AttackState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character)
        {
            _search = Services.Get<ITargetSearchService>();
        }

        public override void Tick(float deltaTime)
        {
            Character.Animation.Tick();
            Character.Weapon.Tick(deltaTime);

            if (!Character.Control.IsFiring)
            {
                _target = null;
                _stateMachine.Enter<IdleState>();
                return;
            }

            if (!IsTargetValid())
            {
                _target = _search.Target(Character.Body, AttackRadius, LayerMask.GetMask("Enemy"), LayerMask.GetMask("Obstacle"));

                if (_target == null)
                    return;
            }

            RotateToTarget(deltaTime);
            Character.Animation.UpdateCombatLayer(isFiring: true);
            Character.Weapon.Attack(Character.Body, _target);
        }

        public override void OnExit()
        {
            _target = null;
            Character.Animation.UpdateCombatLayer(isFiring: false);
        }

        private bool IsTargetValid()
        {
            if (_target == null || Vector3.Distance(Character.Body.position, _target.position) > AttackRadius)
                return false;

            var targetIdentity = _target.GetComponent<IIdentity>();
            if (targetIdentity == null)
                return false;

            return Character.Identity.IsEnemy(targetIdentity);
        }

        private void RotateToTarget(float deltaTime)
        {
            Vector3 dir = _target.position - Character.Body.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            Character.Body.rotation = Quaternion.RotateTowards(Character.Body.rotation, targetRot, deltaTime * 360f);
        }
    }

    public class DeathState : BaseState
    {
        public DeathState(StateMachine stateMachine, CharacterContext character) : base(stateMachine, character) { }

        public override void OnEnter()
        {
            Debug.Log("Enter DeathState");
            Character.Movement.Stop();
            // TODO: death animation
        }
    }
}