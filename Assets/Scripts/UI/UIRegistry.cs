using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence.UI
{
    public class UIRegistry : IUIRegistry
    {
        private readonly Dictionary<string, IScreen> _screens = new Dictionary<string, IScreen>();

        public void Init() => Clear();

        public void RegisterScreen(IScreen screen)
        {
            if (screen == null || string.IsNullOrEmpty(screen.ScreenId))
            {
                Debug.LogWarning("Cannot register null screen or screen with empty ID.");
                return;
            }

            if (_screens.TryAdd(screen.ScreenId, screen))
            {
                return;
            }

            Debug.LogWarning($"Screen '{screen.ScreenId}' is already registered. Overwriting.");
            _screens[screen.ScreenId] = screen;
        }

        public void UnregisterScreen(IScreen screen)
        {
            if (screen == null || string.IsNullOrEmpty(screen.ScreenId))
            {
                return;
            }

            _screens.Remove(screen.ScreenId);
        }

        public bool TryGetScreen<T>(string screenId, out T screen) where T : class, IScreen
        {
            if (_screens.TryGetValue(screenId, out var foundScreen))
            {
                screen = foundScreen as T;
                return screen != null;
            }

            screen = null;
            return false;
        }

        public T GetScreen<T>(string screenId) where T : class, IScreen
        {
            if (TryGetScreen<T>(screenId, out var screen))
            {
                return screen;
            }

            Debug.LogError($"Screen '{screenId}' not found or is not of type {typeof(T).Name}");
            return null;
        }

        public void Clear()
        {
            _screens.Clear();
        }
    }
}

