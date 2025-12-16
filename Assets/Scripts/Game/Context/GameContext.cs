using TowerDefence.Core;

namespace TowerDefence.Game
{
    public class GameContext : IGameContext
    {
        public int TestInt { get; set; }


        public void Init()
        {
            Reset();
        }

        public void Reset()
        {
            TestInt = 0;
        }
    }
}
