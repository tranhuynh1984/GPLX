using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Funtions
{
    public interface IFunctionRepository
    {
       Task<IList<Functions>> GetAll();
    }
}
