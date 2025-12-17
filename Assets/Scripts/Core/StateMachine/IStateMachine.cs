namespace TowerDefence.Core
{
    public interface IStateMachine : IService
    {
        IState CurrentState { get; }
        void Enter<TState>() where TState : IState;
        void SetState(IState newState);
        void Tick(float deltaTime);
    }
}
