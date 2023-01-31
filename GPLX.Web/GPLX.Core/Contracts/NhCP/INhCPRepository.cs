using GPLX.Core.DTO.Response.NhCP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GPLX.Core.Contracts.NhCP
{
    public interface INhCPRepository
    {
        Task<NhCPSearchResponse> GetAll();

        Task<List<NhCPSearchResponseData>> GetCategory();
    }
}