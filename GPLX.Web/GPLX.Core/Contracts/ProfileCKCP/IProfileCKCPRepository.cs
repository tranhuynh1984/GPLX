using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.ProfileCKCP;
using GPLX.Core.DTO.Response.ProfileCKCP;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.ProfileCKCP
{
    public interface IProfileCKCPRepository
    {
        Task<ProfileCKCPSearchResponse> Search(string profileCKMa ="", string Keywords = "");
        Task<ProfileCKCPSearchResponseData> GetById(string profileCKMa, string CPMa);
        Task<ProfileCKCPCreateResponse> Create(ProfileCKCPCreateRequest request);
        Task<ProfileCKCPCreateResponse> Remove(ProfileCKCPCreateRequest request);
    }
}
