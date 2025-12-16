using System.Threading;
using System.Threading.Tasks;
using TowerDefence.Core;

namespace TowerDefence.UI
{
    /// <summary>
    /// UI navigation. Push/Pop for stack-based flow, ShowModal/HideModal for overlays.
    /// </summary>
    public interface IScreenRouter : IService
    {
        Task PushAsync(IScreen screen, CancellationToken cancellationToken = default);
        Task PopAsync(CancellationToken cancellationToken = default);
        Task PopToRootAsync(CancellationToken cancellationToken = default);
        Task ShowModalAsync(IScreen modal, CancellationToken cancellationToken = default);
        Task HideModalAsync(CancellationToken cancellationToken = default);
        IScreen GetCurrentScreen();
        void Clear();
    }
}
