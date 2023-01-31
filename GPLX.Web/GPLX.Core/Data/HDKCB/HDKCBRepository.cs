using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.HDKCB;
using GPLX.Core.DTO.Request.HDKCB;
using GPLX.Core.DTO.Response.HDKCB;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.HDKCB
{
    public class HDKCBRepository : IHDKCBRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HDKCBRepository> _logger;

        public HDKCBRepository(Context context, IMapper mapper, ILogger<HDKCBRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<HDKCBSearchResponse> Search(int skip, int length, HDKCBSearchRequest request)
        {
            var response = new HDKCBSearchResponse() { Draw = request.Draw };

            try
            {
                var query = _context.HDKCB.AsQueryable();
                if (!string.IsNullOrEmpty(request.MaHD))
                    query = query.Where(x => x.MaHD.ToLower().Contains(request.MaHD.Trim().ToLower()));
                if (!string.IsNullOrEmpty(request.TenHD))
                    query = query.Where(x => x.TenHD.ToLower().Contains(request.TenHD.Trim().ToLower()));
                if (!string.IsNullOrEmpty(request.TenHDTimNhanh))
                    query = query.Where(x => x.TenHD.ToLower().Contains(request.TenHDTimNhanh.Trim().ToLower()));
                if (request.IDHD.HasValue)
                    query = query.Where(x => x.IDHD == request.IDHD);
                if (request.ND.HasValue)
                    query = query.Where(x => x.ND >= request.ND);
                if (request.NS.HasValue)
                    query = query.Where(x => x.NS < request.NS.Value.AddDays(1));
                if (request.Status.HasValue && request.Status != -1)
                    query = query.Where(x => x.IsActive == request.Status);

                var data = await query.OrderBy(x => x.MaHD).ToListAsync();

                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<HDKCBSearchResponseData>();

                foreach (var d in data.Skip(skip).Take(length))
                {
                    var dMap = _mapper.Map<HDKCBSearchResponseData>(d);
                    dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                    dataResponse.Add(dMap);
                }
                response.Data = dataResponse;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }

        public async Task<HDKCBSearchResponse> SearchAll(HDKCBSearchRequest request)
        {
            var response = new HDKCBSearchResponse { Draw = request.Draw };
            try
            {
                var query = _context.HDKCB.AsQueryable();
                if (!string.IsNullOrEmpty(request.MaHD))
                    query = query.Where(x => x.MaHD.ToLower().Contains(request.MaHD.Trim().ToLower()));
                if (!string.IsNullOrEmpty(request.TenHD))
                    query = query.Where(x => x.TenHD.ToLower().Contains(request.TenHD.Trim().ToLower()));
                if (request.IDHD.HasValue)
                    query = query.Where(x => x.IDHD == request.IDHD);
                if (request.ND.HasValue)
                    query = query.Where(x => x.ND >= request.ND);
                if (request.NS.HasValue)
                    query = query.Where(x => x.NS < request.NS.Value.AddDays(1));
                if (request.Status.HasValue && request.Status != -1)
                    query = query.Where(x => x.IsActive == request.Status);
                
                var data = await query.OrderByDescending(x => x.TenHD).ToListAsync();
                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<HDKCBSearchResponseData>();

                for(var i = 0; i < data.Count; i ++)
                {
                    var d = data[i];
                    var dMap = _mapper.Map<HDKCBSearchResponseData>(d);
                    dMap.Stt = i + 1;
                    dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                    dMap.NDString = dMap.ND.ToString("HH:mm dd/MM/yyyy");
                    dMap.NSString = dMap.NS.ToString("HH:mm dd/MM/yyyy");

                    dMap.CreatedateString = dMap.Createdate.ToString("HH:mm dd/MM/yyyy");
                    dMap.UpdatedateString = dMap.Updatedate?.ToString("HH:mm dd/MM/yyyy");
                    dataResponse.Add(dMap);
                }
                response.Data = dataResponse;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }
        
        public async Task<HDKCBDetailResponse> GetByIdAsync(int idhd)
        {
            try
            {
                var hdkcb = await _context.HDKCB.FirstOrDefaultAsync(a => a.IDHD == idhd);
                var detailResponse = new HDKCBDetailResponse
                {
                    IDHD = hdkcb.IDHD,
                    MaLoai = hdkcb.MaLoai,
                    MaHD = hdkcb.MaHD,
                    TenHD = hdkcb.TenHD,
                    Ngay = hdkcb.Ngay?.ToString("HH:mm dd/MM/yyyy"),
                    Del = hdkcb.Del,
                    IsActive = hdkcb.IsActive,
                    ND = hdkcb.ND?.ToString("HH:mm dd/MM/yyyy"),
                    NS = hdkcb.NS?.ToString("HH:mm dd/MM/yyyy")
                };
                
                return detailResponse;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }
        
        public async Task<DetailDVSearchResponse> GetByHDKCKService(int skip, int length, string keyword, int? draw,
            int idhd)
        {
            var response = new DetailDVSearchResponse { Draw = draw ?? 0 };
            try
            {
                var servicesQueryable = from giaKCB in _context.GiaKCB.Where(x => x.IDHD == idhd && x.Del == 0)
                    join dmcp in _context.DMCP on giaKCB.MaCP equals dmcp.MaCP 
                        into dp from dmcp in dp.DefaultIfEmpty()
                    join branchPrice in _context.DMBangGiaChiNhanh on giaKCB.MaCP equals branchPrice.MaCP 
                        into dmcn from branchPrice in dmcn.DefaultIfEmpty()
                    select new 
                    {
                        ServiceCode = giaKCB.MaCP,
                        ServiceName = dmcp.TenCP,
                        Amount = giaKCB.SL,
                        Unit = dmcp.DVT,
                        ServicePrice = branchPrice.DG,
                        DiscountPrice = giaKCB.DG,
                        TotalPrice = giaKCB.DG,
                        Del = giaKCB.Del
                    };
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    servicesQueryable = servicesQueryable.Where(x => x.ServiceCode.Contains(keyword) || x.ServiceName.Contains(keyword));
                }
                
                var services = await servicesQueryable.AsNoTracking().ToListAsync();
                var serviceList = services.Skip(skip).Take(length).Select((g) => new DetailDVResponse
                {
                    ServiceCode = g.ServiceCode,
                    ServiceName = g.ServiceName,
                    Amount = g.Amount,
                    Unit = g.Unit,
                    ServicePrice = g.ServicePrice,
                    DiscountPrice = g.DiscountPrice,
                    TotalPrice = g.TotalPrice,
                    Active = g.Del == 0
                }).ToList();
                response.TotalPrice = services.Sum(x => x.TotalPrice);
                response.RecordsFiltered = services.Count;
                response.RecordsTotal = services.Count;
                response.Data = serviceList;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
                return response;
            }
        }
    }
}
