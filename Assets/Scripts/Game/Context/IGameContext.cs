using TowerDefence.Core;

namespace TowerDefence.Game
{
    /// <summary>
    /// Game-wide runtime state. Reset on new session.
    /// </summary>
    public interface IGameContext : IService
    {
        int TestInt { get; set; }
        void Reset();
    }
}

