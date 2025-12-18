using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Systems
{
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