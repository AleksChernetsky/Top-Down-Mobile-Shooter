namespace TowerDefence.Core
{
    public class StateMachine : IStateMachine
    {
        public IState CurrentState { get; private set; }

        public void Init()
        {
            // State machine doesn't need initialization
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
