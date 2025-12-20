using UnityEngine;

namespace TowerDefence.Systems
{
    public enum Team { TeamGreen, TeamRed }
    public enum Race { Human, Elf }
    public interface IIdentity
    {
        Team Team { get; }
        Race Race { get; }
        bool IsEnemy(IIdentity other);
    }

    public class CharacterIdentity : MonoBehaviour, IIdentity
    {
        [SerializeField] private Team _team;
        [SerializeField] private Race _race;

        public Team Team => _team;
        public Race Race => _race;

        public bool IsEnemy(IIdentity other)
        {
            if (other == null)
                return false;

            return Team != other.Team;
        }
    }
}