using GPLX.Core.DTO.Request.DMCTV;
using GPLX.Core.DTO.Request.DMCVT;
using GPLX.Core.DTO.Response.DMCTV;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GPLX.Core.Contracts.DMCTV
{
    public interface IDMCTVRepository
    {
        Task<DMCTVSearchResponse> Search(int skip, int length, DMCTVSearchRequest request);

        Task<DMCTVSearchResponse> SearchAll(DMCTVSearchRequest request);

        /// <summary>
        /// Danh sách tỉnh
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        Task<List<ProvinceRespone>> GetProvince(string provinceCode = "");

        /// <summary>
        /// Danh sách huyện
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>

        Task<List<DistrictRespone>> GetDistrict(string provinceCode = "", string districtCode = "");

        /// <summary>
        /// Lấy danh sách chức danh
        /// </summary>
        /// <returns></returns>
        Task<List<JobTitleRespone>> GetJobTitle();

        Task<List<PartnerObjectRespone>> GetPartnerObject();

        Task<List<DiscountRespone>> GetDiscount();

        Task<DMCTVCreateResponse> Create(DMCTVCreateRequest request);
    }
}