using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DMCP;
using GPLX.Core.DTO.Request.DMCP;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.DMCP
{
    public class DMCPRepository : IDMCPRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DMCPRepository> _logger;

        public DMCPRepository(Context context, IMapper mapper, ILogger<DMCPRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DMCPSearchResponse> Search(int skip, int length, DMCPSearchRequest request)
        {
            var response = new DMCPSearchResponse() { Draw = request.Draw };
            try
            {
                IQueryable<DMBangGiaChiNhanh> pricecn;
                if (!string.IsNullOrEmpty(request.BranchCode) && request.BranchCode != "-1")
                {
                    pricecn = from p in _context.DMBangGiaChiNhanh where p.MaBangGiaChiNhanh == request.BranchCode select p;
                    if (pricecn?.Count() > 0)
                    {
                        var query = from cp in _context.DMCP
                                    join dmcn in pricecn
                                    on cp.MaCP equals dmcn.MaCP
                                    into t
                                    from dmcn in t.DefaultIfEmpty()

                                    select new DMCPSearchResponseData
                                    {
                                        MaCP = cp.MaCP,
                                        MaNN = cp.MaNN,
                                        IsActive = cp.IsActive,
                                        TenCP = cp.TenCP,
                                        MaNhCP = cp.MaNhCP,
                                        DG = dmcn.MaCP == null ? cp.DG : dmcn.DG,
                                        MaLoaiKT1 = cp.MaLoaiKT1,
                                        MaLoaiKT2 = cp.MaLoaiKT2,
                                        MaLoaiKT3 = cp.MaLoaiKT3,
                                        DVT = cp.DVT,
                                        TenCPE = cp.TenCPE,
                                        TenRutGon = cp.TenRutGon,
                                        IsActiveName = "",
                                        KhoaGGTrucTiep = cp.KhoaGGTrucTiep,
                                        DGBH = cp.DGBH,
                                        BranchCode = dmcn.MaBangGiaChiNhanh
                                    };

                        if (!string.IsNullOrEmpty(request.MaCP))
                            query = query.Where(x => x.MaCP.Contains(request.MaCP.Trim()));
                        if (!string.IsNullOrEmpty(request.Keywords))
                            query = query.Where(x => x.TenCP.Contains(request.Keywords.Trim()));
                        if (request.Status != -1)
                            query = query.Where(x => x.IsActive == request.Status);
                        if (!string.IsNullOrEmpty(request.MaNhCP) && request.MaNhCP != "-1")
                            query = query.Where(x => x.MaNhCP == request.MaNhCP);
                        //if (!string.IsNullOrEmpty(request.BranchCode) && request.BranchCode != "-1")
                        //    query = query.Where(x => x.BranchCode == request.BranchCode);

                        var data = await query.OrderBy(x => x.MaCP).ToListAsync().ConfigureAwait(true);

                        response.RecordsFiltered = data.Count;
                        response.RecordsTotal = data.Count;

                        var dataResponse = new List<DMCPSearchResponseData>();

                        foreach (var d in data.Skip(skip).Take(length))
                        {
                            var dMap = _mapper.Map<DMCPSearchResponseData>(d);
                            dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                            dataResponse.Add(dMap);
                        }
                        response.Data = dataResponse;
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    }
                    else
                    {
                        var query = _context.DMCP.AsQueryable();
                        if (!string.IsNullOrEmpty(request.MaCP))
                            query = query.Where(x => x.MaCP.Contains(request.MaCP.Trim()));
                        if (!string.IsNullOrEmpty(request.Keywords))
                            query = query.Where(x => x.TenCP.Contains(request.Keywords.Trim()));
                        if (request.Status != -1)
                            query = query.Where(x => x.IsActive == request.Status);
                        if (!string.IsNullOrEmpty(request.MaNhCP) && request.MaNhCP != "-1")
                            query = query.Where(x => x.MaNhCP == request.MaNhCP);
                        //if (!string.IsNullOrEmpty(request.BranchCode) && request.BranchCode != "-1")
                        //    query = query.Where(x => x.MaBangGiaChiNhanh == request.BranchCode);

                        var data = await query.OrderBy(x => x.MaCP).ToListAsync().ConfigureAwait(true);

                        response.RecordsFiltered = data.Count;
                        response.RecordsTotal = data.Count;

                        var dataResponse = new List<DMCPSearchResponseData>();

                        foreach (var d in data.Skip(skip).Take(length))
                        {
                            var dMap = _mapper.Map<DMCPSearchResponseData>(d);
                            dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                            dataResponse.Add(dMap);
                        }
                        response.Data = dataResponse;
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    }
                }
                else
                {
                    var query = _context.DMCP.AsQueryable();
                    if (!string.IsNullOrEmpty(request.MaCP))
                        query = query.Where(x => x.MaCP.Contains(request.MaCP.Trim()));
                    if (!string.IsNullOrEmpty(request.Keywords))
                        query = query.Where(x => x.TenCP.Contains(request.Keywords.Trim()));
                    if (request.Status != -1)
                        query = query.Where(x => x.IsActive == request.Status);
                    if (!string.IsNullOrEmpty(request.MaNhCP) && request.MaNhCP != "-1")
                        query = query.Where(x => x.MaNhCP == request.MaNhCP);

                    var data = await query.OrderBy(x => x.MaCP).ToListAsync().ConfigureAwait(true);

                    response.RecordsFiltered = data.Count;
                    response.RecordsTotal = data.Count;

                    var dataResponse = new List<DMCPSearchResponseData>();

                    foreach (var d in data.Skip(skip).Take(length))
                    {
                        var dMap = _mapper.Map<DMCPSearchResponseData>(d);
                        dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                        dataResponse.Add(dMap);
                    }
                    response.Data = dataResponse;
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }

        public async Task<DMCPSearchResponse> SearchAll(DMCPSearchRequest request)
        {
            var response = new DMCPSearchResponse() { Draw = request.Draw };
            try
            {
                IQueryable<DMBangGiaChiNhanh> pricecn;
                if (!string.IsNullOrEmpty(request.BranchCode) && request.BranchCode != "-1")
                {
                    pricecn = from p in _context.DMBangGiaChiNhanh where p.MaBangGiaChiNhanh == request.BranchCode select p;
                    if (pricecn?.Count() > 0)
                    {
                        var query = from cp in _context.DMCP
                                    join dmcn in pricecn
                                    on cp.MaCP equals dmcn.MaCP
                                    into t
                                    from dmcn in t.DefaultIfEmpty()
                                    select new DMCPSearchResponseData
                                    {
                                        MaCP = cp.MaCP,
                                        MaNN = cp.MaNN,
                                        IsActive = cp.IsActive,
                                        TenCP = cp.TenCP,
                                        MaNhCP = cp.MaNhCP,
                                        DG = dmcn.MaCP == null ? cp.DG : dmcn.DG,
                                        MaLoaiKT1 = cp.MaLoaiKT1,
                                        MaLoaiKT2 = cp.MaLoaiKT2,
                                        MaLoaiKT3 = cp.MaLoaiKT3,
                                        DVT = cp.DVT,
                                        TenCPE = cp.TenCPE,
                                        TenRutGon = cp.TenRutGon,
                                        IsActiveName = "",
                                        KhoaGGTrucTiep = cp.KhoaGGTrucTiep,
                                        DGBH = cp.DGBH
                                    };

                        if (!string.IsNullOrEmpty(request.MaCP))
                            query = query.Where(x => x.MaCP.Contains(request.MaCP.Trim()));
                        if (!string.IsNullOrEmpty(request.Keywords))
                            query = query.Where(x => x.TenCP.Contains(request.Keywords.Trim()));
                        if (request.Status != -1)
                            query = query.Where(x => x.IsActive == request.Status);
                        if (!string.IsNullOrEmpty(request.MaNhCP) && request.MaNhCP != "-1")
                            query = query.Where(x => x.MaNhCP == request.MaNhCP);

                        var data = await query.OrderBy(x => x.MaCP).ToListAsync().ConfigureAwait(true);

                        response.RecordsFiltered = data.Count;
                        response.RecordsTotal = data.Count;

                        var dataResponse = new List<DMCPSearchResponseData>();

                        for (int i = 0; i < data.Count; i++)
                        {
                            var d = data[i];
                            var dMap = _mapper.Map<DMCPSearchResponseData>(d);
                            dMap.Index = i + 1;
                            dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                            dataResponse.Add(dMap);
                        }
                        //foreach (var d in data)
                        //{
                        //    var dMap = _mapper.Map<DMCPSearchResponseData>(d);
                        //    dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                        //    dataResponse.Add(dMap);
                        //}
                        response.Data = dataResponse;
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    }
                    else
                    {
                        var query = _context.DMCP.AsQueryable();
                        if (!string.IsNullOrEmpty(request.MaCP))
                            query = query.Where(x => x.MaCP.Contains(request.MaCP.Trim()));
                        if (!string.IsNullOrEmpty(request.Keywords))
                            query = query.Where(x => x.TenCP.Contains(request.Keywords.Trim()));
                        if (request.Status != -1)
                            query = query.Where(x => x.IsActive == request.Status);
                        if (!string.IsNullOrEmpty(request.MaNhCP) && request.MaNhCP != "-1")
                            query = query.Where(x => x.MaNhCP == request.MaNhCP);

                        var data = await query.OrderBy(x => x.MaCP).ToListAsync().ConfigureAwait(true);

                        response.RecordsFiltered = data.Count;
                        response.RecordsTotal = data.Count;

                        var dataResponse = new List<DMCPSearchResponseData>();

                        for (int i = 0; i < data.Count; i++)
                        {
                            var d = data[i];
                            var dMap = _mapper.Map<DMCPSearchResponseData>(d);
                            dMap.Index = i + 1;
                            dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                            dataResponse.Add(dMap);
                        }
                        response.Data = dataResponse;
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    }
                }
                else
                {
                    var query = _context.DMCP.AsQueryable();
                    if (!string.IsNullOrEmpty(request.MaCP))
                        query = query.Where(x => x.MaCP.Contains(request.MaCP.Trim()));
                    if (!string.IsNullOrEmpty(request.Keywords))
                        query = query.Where(x => x.TenCP.Contains(request.Keywords.Trim()));
                    if (request.Status != -1)
                        query = query.Where(x => x.IsActive == request.Status);
                    if (!string.IsNullOrEmpty(request.MaNhCP) && request.MaNhCP != "-1")
                        query = query.Where(x => x.MaNhCP == request.MaNhCP);

                    var data = await query.OrderBy(x => x.MaCP).ToListAsync().ConfigureAwait(true);

                    response.RecordsFiltered = data.Count;
                    response.RecordsTotal = data.Count;

                    var dataResponse = new List<DMCPSearchResponseData>();

                    for (int i = 0; i < data.Count; i++)
                    {
                        var d = data[i];
                        var dMap = _mapper.Map<DMCPSearchResponseData>(d);
                        dMap.Index = i + 1;
                        dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                        dataResponse.Add(dMap);
                    }
                    response.Data = dataResponse;
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }

        //public async Task<DMPNSearchResponse> SearchAll(DMPNSearchRequest request)
        //{
        //    var response = new DMPNSearchResponse { Draw = request.Draw };
        //    try
        //    {
        //        var query = _context.DMPN.AsQueryable();
        //        if (!string.IsNullOrEmpty(request.MaPhapNhan))
        //            query = query.Where(x => x.PhapNhanId.ToString().ToLower().Contains(request.MaPhapNhan.Trim().ToLower()));
        //        if (!string.IsNullOrEmpty(request.TenPhapNhan))
        //            query = query.Where(x => x.PhapNhanName.ToLower().Contains(request.TenPhapNhan.Trim().ToLower()));
        //        if (request.Status != null)
        //            query = query.Where(x => x.IsActive == request.Status);

        //        var data = await query.OrderByDescending(x => x.PhapNhanName).ToListAsync();
        //        response.RecordsFiltered = data.Count;
        //        response.RecordsTotal = data.Count;
        //        var dataResponse = new List<DMPNSearchResponseData>();

        //        foreach (var d in data)
        //        {
        //            var dMap = _mapper.Map<DMPNSearchResponseData>(d);

        //            if (dMap.IsActive == 0)
        //                dMap.IsActiveName = GlobalEnums.OtherStatusNames[(int)GlobalEnums.StatusDefaultEnum.InActive];
        //            else if (dMap.IsActive == 1)
        //                dMap.IsActiveName = GlobalEnums.OtherStatusNames[(int)GlobalEnums.StatusDefaultEnum.Active];
        //            else
        //                dMap.IsActiveName = "";

        //            dataResponse.Add(dMap);
        //        }
        //        response.Data = dataResponse;
        //        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Log(LogLevel.Error, e, e.Message);
        //        Log.Error(e, e.Message);

        //        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
        //        response.Message = GlobalEnums.NoContentMessage;
        //    }

        //    return response;
        //}
    }
}