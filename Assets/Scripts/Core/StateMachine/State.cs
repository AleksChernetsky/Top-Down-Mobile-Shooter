using TowerDefence.Movement;
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
        protected readonly CharacterContext Context;

        protected BaseState(StateMachine stateMachine, CharacterContext context) : base(stateMachine)
        {
            Context = context;
        }

        protected bool IsDead => Context.Vitality.IsDead;
    }

    public class IdleState : BaseState
    {
        public IdleState(StateMachine stateMachine, CharacterContext context) : base(stateMachine, context) { }

        public override void Tick(float deltaTime)
        {

            Context.Animation.Tick();

            if (IsDead)
            {
                _stateMachine.SetState(new DeathState(_stateMachine, Context));
                return;
            }

            if (Context.Control.IsFiring)
            {
                _stateMachine.SetState(new AttackState(_stateMachine, Context));
                return;
            }

            if (Context.Control.MoveDirection.sqrMagnitude > 0.01f)
            {
                _stateMachine.SetState(new MoveState(_stateMachine, Context));
                return;
            }
        }
    }

    public class MoveState : BaseState
    {
        public MoveState(StateMachine stateMachine, CharacterContext context) : base(stateMachine, context) { }

        public override void Tick(float deltaTime)
        {
            Context.Animation.Tick();

            if (IsDead)
            {
                _stateMachine.SetState(new DeathState(_stateMachine, Context));
                return;
            }

            Vector2 dir = Context.Control.MoveDirection;

            if (dir.sqrMagnitude < 0.01f)
            {
                Context.Movement.Stop();
                _stateMachine.SetState(new IdleState(_stateMachine, Context));
                return;
            }

            Vector3 target = Context.Movement.Position + new Vector3(dir.x, 0f, dir.y).normalized;

            Context.Movement.SetDestination(target);
            Context.Movement.Rotate(Time.deltaTime);

            if (Context.Control.IsFiring)
            {
                _stateMachine.SetState(new AttackState(_stateMachine, Context));
                return;
            }
        }
    }

    public class AttackState : BaseState
    {
        private readonly ITargetSearchService _search;
        private Transform _target;

        private float _fireTimer;

        private const float AttackRadius = 10f;
        private const float FireCooldown = 0.5f;

        public AttackState(StateMachine stateMachine, CharacterContext context) : base(stateMachine, context)
        {
            _search = Services.Get<ITargetSearchService>();
        }

        public override void Tick(float deltaTime)
        {
            Context.Animation.Tick();

            if (IsDead)
            {
                _stateMachine.SetState(new DeathState(_stateMachine, Context));
                return;
            }

            if (!Context.Control.IsFiring)
            {
                ResetTarget();
                _stateMachine.SetState(new IdleState(_stateMachine, Context));
                return;
            }

            if (!IsTargetValid())
            {
                _target = _search.Target(Context.Transform, AttackRadius, LayerMask.GetMask("Enemy"), LayerMask.GetMask("Obstacle"));

                if (_target == null)
                    return;
            }

            RotateToTarget(deltaTime);

            _fireTimer -= deltaTime;
            if (_fireTimer <= 0f)
            {
                Fire();
                _fireTimer = FireCooldown;
            }
        }
        private bool IsTargetValid()
        {
            if (_target == null || Vector3.Distance(Context.Transform.position, _target.position) > AttackRadius)
                return false;

            var vitality = _target.GetComponent<IVitalitySystem>();
            if (vitality == null || vitality.IsDead)
                return false;

            return true;
        }
        private void RotateToTarget(float deltaTime)
        {
            Vector3 dir = _target.position - Context.Transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            Context.Transform.rotation = Quaternion.RotateTowards(Context.Transform.rotation, targetRot, deltaTime * 360f);
            //Context.Transform.LookAt(new Vector3(target.position.x, Context.Transform.position.y, target.position.z));
        }
        private void Fire()
        {
            var vitality = _target.GetComponent<IVitalitySystem>();
            if (vitality == null)
                return;

            vitality.TakeDamage(Context.Damage);
        }
        private void ResetTarget()
        {
            _target = null;
            _fireTimer = 0f;
        }
    }

    public class DeathState : BaseState
    {
        public DeathState(StateMachine stateMachine, CharacterContext context) : base(stateMachine, context) { }

        public override void OnEnter()
        {
            Debug.Log("Enter DeathState");
            Context.Movement.Stop();
            // TODO: death animation
        }
    }
}