using GPLX.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPLX.Core.Contracts
{
    public interface ISupplierRepository
    {
        Task<Suppliers> GetIdByName(string supplierName);
        Task<IList<Suppliers>> Gets();

        Task<IList<Suppliers>> Searching(string keyword);
    }
}
