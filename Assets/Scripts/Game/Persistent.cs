using TowerDefence.Core;
using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Game
{
    /// <summary>
    /// Persistent GameObject that survives scene changes.
    /// Holds TickDispatcher and keeps ServiceLocator alive.
    /// </summary>
    public sealed class Persistent : MonoBehaviour
    {
        private ITickDispatcher _tickDispatcher;

        public void Initialize()
        {
            DontDestroyOnLoad(gameObject);

            _tickDispatcher = gameObject.AddComponent<TickDispatcher>();
            _tickDispatcher.Init();
            Services.Get<IServiceLocator>().Register(_tickDispatcher);

            var stateMachine = Services.Get<IStateMachine>();
            _tickDispatcher.Subscribe(deltaTime => stateMachine?.Tick(deltaTime));
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void OnApplicationQuit()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _tickDispatcher?.Clear();

            if (Services.TryGet<IEventBus>(out var eventBus))
            {
                eventBus.Clear();
            }

            if (Services.TryGet<ISceneLoader>(out var sceneLoader))
            {
                sceneLoader.Dispose();
            }

            if (Services.TryGet<IServiceLocator>(out var locator))
            {
                locator.Clear();
            }
        }
    }
}
