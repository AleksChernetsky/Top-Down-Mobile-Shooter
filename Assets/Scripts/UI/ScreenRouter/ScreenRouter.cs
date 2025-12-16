using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TowerDefence.UI
{
    public class ScreenRouter : IScreenRouter
    {
        private readonly Stack<IScreen> _screenStack = new Stack<IScreen>();
        private readonly Stack<IScreen> _modalStack = new Stack<IScreen>();
        private readonly HashSet<string> _activeScreenIds = new HashSet<string>();

        public void Init() => Clear();

        public async Task PushAsync(IScreen screen, CancellationToken cancellationToken = default)
        {
            if (screen == null)
            {
                throw new ArgumentNullException(nameof(screen));
            }

            if (_activeScreenIds.Contains(screen.ScreenId))
            {
                Debug.LogWarning($"Screen '{screen.ScreenId}' is already in the stack. Ignoring duplicate push.");
                return;
            }

            if (_screenStack.Count > 0)
            {
                var current = _screenStack.Peek();
                await current.HideAsync(cancellationToken);
            }

            _screenStack.Push(screen);
            _activeScreenIds.Add(screen.ScreenId);
            await screen.ShowAsync(cancellationToken);
        }

        public async Task PopAsync(CancellationToken cancellationToken = default)
        {
            if (_screenStack.Count == 0)
            {
                Debug.LogWarning("Cannot pop screen. Stack is empty.");
                return;
            }

            var current = _screenStack.Pop();
            _activeScreenIds.Remove(current.ScreenId);
            await current.HideAsync(cancellationToken);

            if (_screenStack.Count > 0)
            {
                var previous = _screenStack.Peek();
                await previous.ShowAsync(cancellationToken);
            }
        }

        public async Task PopToRootAsync(CancellationToken cancellationToken = default)
        {
            if (_screenStack.Count == 0)
            {
                return;
            }

            var current = _screenStack.Peek();
            await current.HideAsync(cancellationToken);

            var root = _screenStack.Count > 0 ? _screenStack.ToArray()[_screenStack.Count - 1] : null;
            _screenStack.Clear();
            _activeScreenIds.Clear();

            if (root != null)
            {
                _screenStack.Push(root);
                _activeScreenIds.Add(root.ScreenId);
                await root.ShowAsync(cancellationToken);
            }
        }

        public async Task ShowModalAsync(IScreen modal, CancellationToken cancellationToken = default)
        {
            if (modal == null)
            {
                throw new ArgumentNullException(nameof(modal));
            }

            if (_activeScreenIds.Contains(modal.ScreenId))
            {
                Debug.LogWarning($"Modal '{modal.ScreenId}' is already active. Ignoring duplicate show.");
                return;
            }

            _modalStack.Push(modal);
            _activeScreenIds.Add(modal.ScreenId);
            await modal.ShowAsync(cancellationToken);
        }

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modalStack.Count == 0)
            {
                Debug.LogWarning("Cannot hide modal. No modals are active.");
                return;
            }

            var modal = _modalStack.Pop();
            _activeScreenIds.Remove(modal.ScreenId);
            await modal.HideAsync(cancellationToken);
        }

        public IScreen GetCurrentScreen()
        {
            return _screenStack.Count > 0 ? _screenStack.Peek() : null;
        }

        public void Clear()
        {
            _screenStack.Clear();
            _modalStack.Clear();
            _activeScreenIds.Clear();
        }
    }
}
