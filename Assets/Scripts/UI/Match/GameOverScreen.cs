using TMPro;
using TowerDefence.Core;
using TowerDefence.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefence.UI
{
    public class GameOverScreen : BaseScreen
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _exitButton;

        private IEventToken _token;
        private IEventBus _eventBus;

        protected override void Awake()
        {
            base.Awake();

            _eventBus = Services.Get<IEventBus>();
            _token = _eventBus.Subscribe<GameOverEvent>(OnGameEnded);

            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnExitClicked()
        {
            _eventBus.Publish(new ReturnToMenuRequestedEvent());
        }

        protected override void OnDestroy()
        {
            if (_token != null && Services.TryGet<IEventBus>(out var bus))
            {
                bus.Unsubscribe(_token);
            }

            base.OnDestroy();
        }

        private async void OnGameEnded(GameOverEvent evt)
        {
            _titleText.text = $"{evt.Winner} Wins!";
            await ShowAsync();
        }
    }
}
