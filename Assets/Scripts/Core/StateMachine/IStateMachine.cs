namespace TowerDefence.Core
{
    public interface IStateMachine : IService
    {
        IState CurrentState { get; }
        void SetState(IState newState);
        void Tick(float deltaTime);
    }
}
