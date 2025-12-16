using System;
using System.Threading;
using System.Threading.Tasks;
using TowerDefence.Core;
using UnityEngine.SceneManagement;

namespace TowerDefence.Systems
{
    /// <summary>
    /// Async scene loading with cancellation support.
    /// </summary>
    public interface ISceneLoader : IService, IDisposable
    {
        Task LoadSceneAsync(string sceneName, LoadSceneMode mode, CancellationToken cancellationToken = default);
        Task UnloadSceneAsync(string sceneName, CancellationToken cancellationToken = default);
        bool IsSceneLoaded(string sceneName);
    }
}
