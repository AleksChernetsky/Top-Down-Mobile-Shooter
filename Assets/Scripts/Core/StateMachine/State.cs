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

            if (IsDead)
            {
                _stateMachine.SetState(new DeathState(_stateMachine, Character));
                return;
            }

            if (Character.Control.IsFiring)
            {
                _stateMachine.SetState(new AttackState(_stateMachine, Character));
                return;
            }

            if (Character.Control.MoveDirection.sqrMagnitude > 0.01f)
            {
                _stateMachine.SetState(new MoveState(_stateMachine, Character));
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

            if (IsDead)
            {
                _stateMachine.SetState(new DeathState(_stateMachine, Character));
                return;
            }

            Vector2 dir = Character.Control.MoveDirection;

            if (dir.sqrMagnitude < 0.01f)
            {
                Character.Movement.Stop();
                _stateMachine.SetState(new IdleState(_stateMachine, Character));
                return;
            }

            Vector3 target = Character.Movement.Position + new Vector3(dir.x, 0f, dir.y).normalized;

            Character.Movement.SetDestination(target);
            Character.Movement.Rotate(Time.deltaTime);

            if (Character.Control.IsFiring)
            {
                _stateMachine.SetState(new AttackState(_stateMachine, Character));
                return;
            }
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

            if (IsDead)
            {
                _stateMachine.SetState(new DeathState(_stateMachine, Character));
                return;
            }

            if (!Character.Control.IsFiring)
            {
                _target = null;
                _stateMachine.SetState(new IdleState(_stateMachine, Character));
                return;
            }

            if (!IsTargetValid())
            {
                _target = _search.Target(Character.Body, AttackRadius, LayerMask.GetMask("Enemy"), LayerMask.GetMask("Obstacle"));

                if (_target == null)
                    return;
            }

            RotateToTarget(deltaTime);
            Character.Weapon.TryAttack(Character.Body, _target);
        }

        private bool IsTargetValid() => _target != null && Vector3.Distance(Character.Body.position, _target.position) < AttackRadius;

        private void RotateToTarget(float deltaTime)
        {
            Vector3 dir = _target.position - Character.Body.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            Character.Body.rotation = Quaternion.RotateTowards(Character.Body.rotation, targetRot, deltaTime * 360f);
            //Context.Body.LookAt(new Vector3(_target.position.x, Context.Body.position.y, _target.position.z));
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