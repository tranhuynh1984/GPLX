using System.Threading.Tasks;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.PdfLogs
{
    public interface IPdfLogsRepository
    {
        Task<bool> CreateAsync(FilePdfCreateLogs create);
        Task<int> CounterDay(int unit, string type);
    }
}
