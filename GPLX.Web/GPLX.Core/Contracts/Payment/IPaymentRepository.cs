using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Payment
{
    public interface IPaymentRepository
    {
        Task<IList<Database.Models.Payment>> AllPayments();
        Task<IList<CostEstimateItemType>> AllTypes(string unitType);
    }
}
