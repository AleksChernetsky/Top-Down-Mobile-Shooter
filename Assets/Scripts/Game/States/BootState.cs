using System.Threading;
using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefence.Game
{
    /// <summary>
    /// Initial state. Loads Menu scene and transitions to MenuState.
    /// </summary>
    public class BootState : IState
    {
        private bool _isLoading;

        public void OnEnter()
        {
            LoadMenuScene();
        }

        public void OnExit() { }

        public void Tick(float deltaTime) { }

        private async void LoadMenuScene()
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;

            // Just use Services.Get - no parameters!
            var sceneLoader = Services.Get<ISceneLoader>();
            var config = Services.Get<GameConfig>();

            await sceneLoader.LoadSceneAsync(config.MenuSceneName, LoadSceneMode.Single, CancellationToken.None);

            var stateMachine = Services.Get<IStateMachine>();
            stateMachine.SetState(new MenuState());
        }
    }
}
