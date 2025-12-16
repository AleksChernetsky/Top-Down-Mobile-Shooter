using TowerDefence.Core;
using TowerDefence.Data;
using TowerDefence.Systems;
using TowerDefence.UI;

namespace TowerDefence.Game
{
    public static class GameInstaller
    {
        public static void Install(IServiceLocator services)
        {
            services.RegisterLazy<IEventBus, EventBus>();
            services.RegisterLazy<IStateMachine, StateMachine>();
            services.RegisterLazy<ISceneLoader, SceneLoaderService>();
            services.RegisterLazy<IObjectPooler, ObjectPooler>();
            services.RegisterLazy<IInputService, InputService>();
            services.RegisterLazy<IConfigProvider, ConfigProvider>();
            services.RegisterLazy<IScreenRouter, ScreenRouter>();
            services.RegisterLazy<IUIRegistry, UIRegistry>();
            services.RegisterLazy<IGameContext, GameContext>();
        }
    }
}
