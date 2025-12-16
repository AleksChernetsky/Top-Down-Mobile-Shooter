using TowerDefence.Core;
using TowerDefence.Systems;
using TowerDefence.UI;
using UnityEngine;

namespace TowerDefence.Game
{
    public class GameplayState : IState
    {
        private IEventBus _eventBus;
        private IEventToken _pauseToken;
        private IEventToken _resumeToken;
        private IEventToken _gameOverToken;
        private IEventToken _returnToMenuToken;

        public async void OnEnter()
        {
            var screenRouter = Services.Get<IScreenRouter>();
            screenRouter.Clear();

            _eventBus = Services.Get<IEventBus>();
            _pauseToken = _eventBus.Subscribe<PauseGameRequestedEvent>(OnPauseRequested);
            _resumeToken = _eventBus.Subscribe<ResumeGameRequestedEvent>(OnResumeRequested);
            _gameOverToken = _eventBus.Subscribe<GameOverEvent>(OnGameOver);
            _returnToMenuToken = _eventBus.Subscribe<ReturnToMenuRequestedEvent>(OnReturnToMenu);

            var uiRegistry = Services.Get<IUIRegistry>();
            if (uiRegistry.TryGetScreen<IScreen>("GameplayHUD", out var hud))
            {
                await screenRouter.PushAsync(hud);
            }
            else
            {
                Debug.LogWarning("GameplayHUD not found. Make sure it exists in Gameplay scene with ScreenId='GameplayHUD'");
            }
        }

        public void OnExit()
        {
            if (_eventBus == null)
            {
                return;
            }

            if (_pauseToken != null) _eventBus.Unsubscribe(_pauseToken);
            if (_resumeToken != null) _eventBus.Unsubscribe(_resumeToken);
            if (_gameOverToken != null) _eventBus.Unsubscribe(_gameOverToken);
            if (_returnToMenuToken != null) _eventBus.Unsubscribe(_returnToMenuToken);
        }

        public void Tick(float deltaTime){}

        private async void OnPauseRequested(PauseGameRequestedEvent evt)
        {
            Time.timeScale = 0f;

            var uiRegistry = Services.Get<IUIRegistry>();
            if (!uiRegistry.TryGetScreen<IScreen>("Pause", out var pauseScreen))
            {
                return;
            }

            var screenRouter = Services.Get<IScreenRouter>();
            await screenRouter.ShowModalAsync(pauseScreen);
        }

        private async void OnResumeRequested(ResumeGameRequestedEvent evt)
        {
            Time.timeScale = 1f;

            var screenRouter = Services.Get<IScreenRouter>();
            await screenRouter.HideModalAsync();
        }

        private async void OnGameOver(GameOverEvent evt){}

        private async void OnReturnToMenu(ReturnToMenuRequestedEvent evt)
        {
            Time.timeScale = 1f;

            var sceneLoader = Services.Get<ISceneLoader>();
            var config = Services.Get<GameConfig>();
            await sceneLoader.LoadSceneAsync(config.MenuSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

            var stateMachine = Services.Get<IStateMachine>();
            stateMachine.SetState(new MenuState());
        }
    }
}
