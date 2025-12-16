using System;
using System.Threading;
using TowerDefence.Core;
using TowerDefence.Systems;
using TowerDefence.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefence.Game
{
    public class MenuState : IState
    {
        private IEventBus _eventBus;
        private IEventToken _startGameToken;

        public async void OnEnter()
        {
            var screenRouter = Services.Get<IScreenRouter>();
            screenRouter.Clear();

            _eventBus = Services.Get<IEventBus>();
            _startGameToken = _eventBus.Subscribe<StartGameRequestedEvent>(OnStartGameRequested);

            var uiRegistry = Services.Get<IUIRegistry>();
            if (uiRegistry.TryGetScreen<MainMenuScreen>("MainMenu", out var mainMenu))
            {
                await screenRouter.PushAsync(mainMenu);
            }
            else
            {
                Debug.LogError("MainMenuScreen not found in UIRegistry. Make sure it exists in Menu scene with ScreenId='MainMenu'");
            }
        }

        public void OnExit()
        {
            if (_eventBus != null && _startGameToken != null)
            {
                _eventBus.Unsubscribe(_startGameToken);
            }
        }

        public void Tick(float deltaTime) { }

        private async void OnStartGameRequested(StartGameRequestedEvent evt)
        {
            try
            {
                var sceneLoader = Services.Get<ISceneLoader>();
                var config = Services.Get<GameConfig>();

                await sceneLoader.LoadSceneAsync(
                    config.GameSceneName,
                    LoadSceneMode.Single,
                    CancellationToken.None
                );

                var stateMachine = Services.Get<IStateMachine>();
                stateMachine.SetState(new GameplayState());
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to start game: {ex}");
            }
        }
    }
}
