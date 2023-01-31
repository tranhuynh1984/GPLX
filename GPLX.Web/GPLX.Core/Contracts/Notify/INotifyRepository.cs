using System.Threading.Tasks;

namespace GPLX.Core.Contracts.Notify
{
    public interface INotifyRepository
    {
        Task<bool> AddAsync(Database.Models.Notify add);
    }
}
