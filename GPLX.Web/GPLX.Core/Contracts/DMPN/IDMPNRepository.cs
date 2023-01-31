using System.Threading.Tasks;
using GPLX.Core.DTO.Request.DMPN;
using GPLX.Core.DTO.Response.DMPN;

namespace GPLX.Core.Contracts.DMPN
{
    public interface IDMPNRepository
    {
        Task<DMPNSearchResponse> Search(int skip, int length, DMPNSearchRequest request);
        Task<DMPNSearchResponse> SearchAll(DMPNSearchRequest request);
    }
}
