using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.ProfileCK;
using GPLX.Core.DTO.Response.ProfileCK;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.ProfileCK
{
    public interface IProfileCKRepository
    {
        Task<ProfileCKSearchResponse> Search(int skip, int length, ProfileCKSearchRequest request);
        Task<ProfileCKSearchResponse> SearchAll(ProfileCKSearchRequest request);
        Task<ProfileCKSearchResponseData> GetById(string profileCKMa);
        Task<ProfileCKCreateResponse> Create(ProfileCKCreateRequest request);
        Task<ProfileCKCreateResponse> Remove(ProfileCKCreateRequest request);
    }
}
