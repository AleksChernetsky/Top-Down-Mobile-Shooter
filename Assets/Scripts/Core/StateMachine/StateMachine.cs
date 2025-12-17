using System;
using System.Collections.Generic;

namespace TowerDefence.Core
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        public IState CurrentState { get; private set; }

        public void Init()
        {
            // State machine doesn't need initialization
        }

        public void RegisterState(IState state)
        {
            _states[state.GetType()] = state;
        }

        public void Enter<TState>() where TState : IState
        {
            var type = typeof(TState);

            if (_states.TryGetValue(type, out var newState))
            {
                SetState(newState);
            }
        }

        public void SetState(IState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();
        }

        public void Tick(float deltaTime)
        {
            CurrentState?.Tick(deltaTime);
        }
    }
}
