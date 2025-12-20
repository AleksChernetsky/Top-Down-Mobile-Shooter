using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefence.Game
{
    [Serializable]
    public class TeamSpawnData
    {
        public Team Team;
        public Transform[] SpawnPoints;
        public GameObject CharacterPrefab;
    }

    public class MatchController : MonoBehaviour
    {
        [Header("Spawn Values")]
        [SerializeField] private TeamSpawnData[] _teams;
        [SerializeField] private float _respawnDelay = 3f;

        [Header("Patrol points")]
        [SerializeField] private Transform[] _patrolPoints;

        private readonly List<CharacterHandler> _aliveCharacters = new();
        private CharacterHandler _playerCharacter;
        private IEventToken _deathToken;

        private bool _isGameOver = false;

        private void Awake()
        {
            _deathToken = Services.Get<IEventBus>().Subscribe<CharacterDied>(OnCharacterDied);
        }

        private void Start()
        {
            SpawnAllCharacters();
        }

        private void OnDestroy()
        {
            if (Services.TryGet<IEventBus>(out var bus))
            {
                bus.Unsubscribe(_deathToken);
            }
        }

        private void SpawnAllCharacters()
        {
            bool playerAssigned = false;

            foreach (var teamData in _teams)
            {
                foreach (var spawnPoint in teamData.SpawnPoints)
                {
                    bool shouldBePlayer = !playerAssigned && teamData.Team == Team.TeamGreen;

                    SpawnSingleCharacter(teamData.Team, shouldBePlayer, spawnPoint);

                    if (shouldBePlayer) playerAssigned = true;
                }
            }
        }

        private void SpawnSingleCharacter(Team team, bool isPlayer, Transform specificSpawnPoint = null)
        {
            var teamData = GetTeamData(team);
            var enemyData = GetTeamData(GetOppositeTeam(team));
            if (teamData == null || enemyData == null) return;

            Transform spawnTransform = specificSpawnPoint != null
                ? specificSpawnPoint
                : teamData.SpawnPoints[Random.Range(0, teamData.SpawnPoints.Length)];

            var go = Instantiate(teamData.CharacterPrefab, spawnTransform.position, spawnTransform.rotation);
            var character = go.GetComponent<CharacterHandler>();

            if (isPlayer) _playerCharacter = character;

            character.Initialize(isPlayer ? ControlType.Player : ControlType.Bot, _patrolPoints);

            _aliveCharacters.Add(character);
        }

        private IEnumerator RespawnAsEnemyRoutine(GameObject deadObject, Team oldTeam, bool wasPlayer)
        {
            yield return new WaitForSeconds(_respawnDelay);

            if (deadObject != null)
                Destroy(deadObject);

            Team newTeam = GetOppositeTeam(oldTeam);

            SpawnSingleCharacter(newTeam, wasPlayer);
        }

        private void OnCharacterDied(CharacterDied evt)
        {
            if (_isGameOver) return;

            var deadCharacter = evt.Character.GetComponent<CharacterHandler>();
            if (deadCharacter == null) return;

            if (_aliveCharacters.Contains(deadCharacter))
            {
                _aliveCharacters.Remove(deadCharacter);
            }

            var deadIdentity = deadCharacter.GetIdentity();
            bool wasPlayer = (deadCharacter == _playerCharacter);
            Team oldTeam = deadIdentity.Team;

            if (CheckWinCondition())
            {
                return;
            }

            StartCoroutine(RespawnAsEnemyRoutine(evt.Character, oldTeam, wasPlayer));
        }

        private bool CheckWinCondition()
        {
            int team1Count = _aliveCharacters.Count(c => c.GetIdentity().Team == Team.TeamGreen);
            int team2Count = _aliveCharacters.Count(c => c.GetIdentity().Team == Team.TeamRed);

            if (team1Count == 0)
            {
                EndGame(Team.TeamRed);
                return true;
            }

            if (team2Count == 0)
            {
                EndGame(Team.TeamGreen);
                return true;
            }

            return false;
        }

        private TeamSpawnData GetTeamData(Team team)
        {
            foreach (var t in _teams)
                if (t.Team == team) return t;
            return null;
        }

        private Team GetOppositeTeam(Team team)
        {
            return team == Team.TeamGreen ? Team.TeamRed : Team.TeamGreen;
        }

        private void EndGame(Team winner)
        {
            if (_isGameOver)
                return;

            _isGameOver = true;

            StopAllCoroutines();

            Services.Get<IEventBus>().Publish(new GameOverEvent
            {
                Winner = winner
            });
        }
    }
}
