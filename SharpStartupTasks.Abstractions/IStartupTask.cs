using System.Threading;
using System.Threading.Tasks;

namespace SharpStartupTasks
{
    public interface IStartupTask : IBaseStartupTask
    {
        public Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
