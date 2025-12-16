using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Game
{
    /// <summary>
    /// Main game configuration
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TowerDefence/Game/GameConfig")]
    public class GameConfig : ScriptableObject, IService
    {
        [Header("Boot Settings")]
        [SerializeField] private string _menuSceneName = "Menu";
        [SerializeField] private string _gameSceneName = "Gameplay";

        public string MenuSceneName => _menuSceneName;
        public string GameSceneName => _gameSceneName;

        public void Init() { }
    }
}
