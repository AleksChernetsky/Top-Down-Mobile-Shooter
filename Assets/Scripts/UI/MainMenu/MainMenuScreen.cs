using UnityEngine;
using UnityEngine.UI;
using TowerDefence.Core;
using TowerDefence.Game;

namespace TowerDefence.UI
{
    public class MainMenuScreen : BaseScreen
    {
        [SerializeField] private Button _playButton;

        private IEventBus _eventBus;

        protected override void Awake()
        {
            base.Awake();
            _eventBus = Services.Get<IEventBus>();
            _playButton.onClick.AddListener(OnPlayClicked);
        }

        private void OnPlayClicked()
        {
            _eventBus.Publish(new StartGameRequestedEvent());
        }

        private async void OnSettingsClicked()
        {
            var uiRegistry = Services.Get<IUIRegistry>();
            if (uiRegistry.TryGetScreen<IScreen>("Settings", out var settingsScreen))
            {
                var screenRouter = Services.Get<IScreenRouter>();
                await screenRouter.ShowModalAsync(settingsScreen);
            }
            else
            {
                Debug.LogWarning("Settings screen not found");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_playButton != null) _playButton.onClick.RemoveListener(OnPlayClicked);
        }
    }
}
