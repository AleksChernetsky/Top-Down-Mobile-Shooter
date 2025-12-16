using System.Threading;
using System.Threading.Tasks;

namespace TowerDefence.UI
{
    public interface IScreen
    {
        string ScreenId { get; }
        Task ShowAsync(CancellationToken cancellationToken = default);
        Task HideAsync(CancellationToken cancellationToken = default);
    }
}

