using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.DeXuatGhiChu;
using GPLX.Core.DTO.Response.DeXuatGhiChu;

namespace GPLX.Core.Contracts.DeXuatGhiChu
{
    public interface IDeXuatGhiChuRepository
    {
        Task<DeXuatGhiChuSearchResponse> Search(DeXuatGhiChuSearchRequest request);
        Task<DeXuatGhiChuCreateResponse> Create(DeXuatGhiChuCreateRequest request);
        Task<List<Database.Models.DeXuatGhiChu>> FindAllByCode(string code);
    }
}
