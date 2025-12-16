using System;
using TowerDefence.Core;
using UnityEngine;

namespace TowerDefence.Game
{
    /// <summary>
    /// Bootstraps the game on startup.
    /// Creates services and Persistent object
    /// </summary>
    public sealed class GameEntry : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;

        private void Awake()
        {
            try
            {
                Bootstrap();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to bootstrap: {ex}");
            }
        }

        private void Bootstrap()
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Register(_config);

            GameInstaller.Install(serviceLocator);
            serviceLocator.Register(serviceLocator);

            var persistentGo = new GameObject("Persistent");
            var persistent = persistentGo.AddComponent<Persistent>();
            persistent.Initialize();

            var stateMachine = Services.Get<IStateMachine>();
            stateMachine.SetState(new BootState());
        }
    }
}
