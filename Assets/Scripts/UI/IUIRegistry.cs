using TowerDefence.Core;

namespace TowerDefence.UI
{
    public interface IUIRegistry : IService
    {
        void RegisterScreen(IScreen screen);
        void UnregisterScreen(IScreen screen);
        bool TryGetScreen<T>(string screenId, out T screen) where T : class, IScreen;
        T GetScreen<T>(string screenId) where T : class, IScreen;
        void Clear();
    }
}

