using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.UI
{
    /// <summary>
    /// Base class for UI screens. Handles auto-registration.
    /// Override OnShow/OnHide for custom logic.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseScreen : MonoBehaviour, IScreen
    {
        [SerializeField] private string _screenId;
        
        private CanvasGroup _canvasGroup;
        private Tween _fadeTween;

        public string ScreenId => _screenId;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            RegisterSelf();
        }

        protected virtual void OnDestroy()
        {
            _fadeTween?.Kill();
            
            UnregisterSelf();
        }

        private void RegisterSelf()
        {
            var registry = Services.Get<IUIRegistry>();
            registry.RegisterScreen(this);
        }

        private void UnregisterSelf()
        {
            if (Services.TryGet<IUIRegistry>(out var registry))
            {
                registry.UnregisterScreen(this);
            }
        }

        public virtual async Task ShowAsync(CancellationToken cancellationToken = default)
        {

            gameObject.SetActive(true);

            _fadeTween?.Kill();

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            OnShow();

            _fadeTween = _canvasGroup.DOFade(1f, 0.3f)
                .OnComplete(() =>
                {
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                });

            await _fadeTween.AsyncWaitForCompletion();
        }

        public virtual async Task HideAsync(CancellationToken cancellationToken = default)
        {
            _fadeTween?.Kill();

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            OnHide();

            _fadeTween = _canvasGroup.DOFade(0f, 0.3f)
                .OnComplete(() => gameObject.SetActive(false));

            await _fadeTween.AsyncWaitForCompletion();
        }

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }
    }
}