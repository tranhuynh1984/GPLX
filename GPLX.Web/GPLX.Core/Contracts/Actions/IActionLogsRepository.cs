using System.Threading.Tasks;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Actions
{
    public interface IActionLogsRepository
    {
        Task AddLogAsync(ActionLogs item);
    }
}
