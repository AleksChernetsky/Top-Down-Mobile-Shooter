namespace TowerDefence.Core
{
    public enum Team { Team1, Team2 }
    public enum Race { Race1, Race2 }

    public interface IIdentity
    {
        Team Team { get; }
        Race Race { get; }
        bool IsEnemy(IIdentity other);
    }
}

