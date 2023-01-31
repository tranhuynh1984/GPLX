using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.DMHuyen;
using GPLX.Core.DTO.Request.DMHuyen;
using GPLX.Core.DTO.Response.DMHuyen;
using GPLX.Core.Enum;
using GPLX.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.DMHuyen
{
    public class DMHuyenRepository : IDMHuyenRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DMHuyenRepository> _logger;

        public DMHuyenRepository(Context context, IMapper mapper, ILogger<DMHuyenRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DMHuyenSearchResponse> Search(int skip, int length, DMHuyenSearchRequest request)
        {
            var response = new DMHuyenSearchResponse { Draw = request.Draw };
            var query = _context.DMHuyen.AsQueryable()
                .LeftJoin(_context.DM, huyen => huyen.MaTinh, tinh => tinh.MaDM, (huyen, tinh) => new
                {
                    MaHuyen = huyen.MaHuyen,
                    TenHuyen = huyen.TenHuyen,
                    MaTinh = tinh.MaDM,
                    TenTinh = tinh.TenDM,
                    IdTree = tinh.IDTree,
                    IsActive = huyen.IsActive
                });
            query = query.Where(x => x.IdTree == 59);
            if (!string.IsNullOrEmpty(request.MaHuyen))
                query = query.Where(x => x.MaHuyen.ToLower().Contains(request.MaHuyen.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.TenHuyen))
                query = query.Where(x => x.TenHuyen.ToLower().Contains(request.TenHuyen.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);
            if (request.MaTinh != "-1")
                query = query.Where(x => x.MaTinh == request.MaTinh);

            var data = await query.OrderByDescending(x => x.TenHuyen).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMHuyenSearchResponseData>();
            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = new DMHuyenSearchResponseData
                {
                    MaHuyen = d.MaHuyen,
                    TenHuyen = d.TenHuyen,
                    MaTinh = d.MaTinh,
                    TenTinh = d.TenTinh,
                    IsActive = d.IsActive
                };
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dMap.CreatedateString = dMap.Createdate.ToString("HH:mm dd/MM/yyyy");
                dMap.UpdatedateString = dMap.Updatedate?.ToString("HH:mm dd/MM/yyyy");
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DMHuyenSearchResponse> SearchAll(DMHuyenSearchRequest request)
        {
            var response = new DMHuyenSearchResponse { Draw = request.Draw };

            var query = _context.DMHuyen.AsQueryable()
            .LeftJoin(_context.DM, huyen => huyen.MaTinh, tinh => tinh.MaDM, (huyen, tinh) => new
            {
                MaHuyen = huyen.MaHuyen,
                TenHuyen = huyen.TenHuyen,
                MaTinh = tinh.MaDM,
                TenTinh = tinh.TenDM,
                IdTree = tinh.IDTree,
                IsActive = huyen.IsActive
            });
            query = query.Where(x => x.IdTree == 59);
            if (!string.IsNullOrEmpty(request.MaHuyen))
                query = query.Where(x => x.MaHuyen.ToLower().Contains(request.MaHuyen.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.TenHuyen))
                query = query.Where(x => x.TenHuyen.ToLower().Contains(request.TenHuyen.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);
            if (request.MaTinh != "-1")
                query = query.Where(x => x.MaTinh == request.MaTinh);

            var data = await query.OrderByDescending(x => x.TenHuyen).ToListAsync();
            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMHuyenSearchResponseData>();

            for (var i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var dMap = new DMHuyenSearchResponseData
                {
                    Stt = i + 1,
                    MaHuyen = d.MaHuyen,
                    TenHuyen = d.TenHuyen,
                    MaTinh = d.MaTinh,
                    TenTinh = d.TenTinh,
                    IsActive = d.IsActive
                };
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }
    }
}
