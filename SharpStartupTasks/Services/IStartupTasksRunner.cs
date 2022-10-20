using System.Threading;
using System.Threading.Tasks;

namespace SharpStartupTasks.Services
{
    public interface IStartupTasksRunner
    {
        Task RunTasksAsync(CancellationToken cancellationToken);
    }
}