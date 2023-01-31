using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DMDV;
using GPLX.Core.DTO.Request.DMDV;
using GPLX.Core.DTO.Response.DMDV;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.DMDV
{
    public class DMDVRepository : IDMDVRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DMDVRepository> _logger;

        public DMDVRepository(Context context, IMapper mapper, ILogger<DMDVRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<DMDVSearchResponseData>> GetCategory()
        {
            var query = _context.DMDV.AsQueryable();
            var data = await query.OrderBy(x => x.MaDV).Select(x => new DMDVSearchResponseData()
            {
                MaDV = x.MaDV,
                TenDV = x.TenDV
            }).ToListAsync().ConfigureAwait(true);
            return data;
        }

        public async Task<DMDVSearchResponse> Search(int skip, int length, DMDVSearchRequest request)
        {
            var response = new DMDVSearchResponse { Draw = request.Draw };

            var query = _context.DMDV.AsQueryable();
            if (!string.IsNullOrEmpty(request.MaDonVi))
                query = query.Where(x => x.MaDV.ToLower().Contains(request.MaDonVi.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.TenDonVi))
                query = query.Where(x => x.TenDV.ToLower().Contains(request.TenDonVi.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);
            if (request.PhapNhanId != -1)
                query = query.Where(x => x.PhapNhanId == request.PhapNhanId);

            var data = await query.OrderBy(x => x.MaDV).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMDVSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<DMDVSearchResponseData>(d);
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                if (d.PhapNhanId != 0)
                    dMap.PhapNhanName = _context.DMPN.Where(x => x.PhapNhanId == d.PhapNhanId).FirstOrDefault().PhapNhanName;
                else
                    dMap.PhapNhanName = "";

                dataResponse.Add(dMap);
            }

            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DMDVSearchResponse> SearchAll(DMDVSearchRequest request)
        {
            var response = new DMDVSearchResponse { Draw = request.Draw };

            var query = _context.DMDV.AsQueryable();
            if (!string.IsNullOrEmpty(request.MaDonVi))
                query = query.Where(x => x.MaDV.ToLower().Contains(request.MaDonVi.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.TenDonVi))
                query = query.Where(x => x.TenDV.ToLower().Contains(request.TenDonVi.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);
            if (request.PhapNhanId != -1)
                query = query.Where(x => x.PhapNhanId == request.PhapNhanId);
            var data = await query.OrderBy(x => x.TenDV).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMDVSearchResponseData>();

            for(var i = 0; i < data.Count; i ++)
            {
                var d = data[i];
                var dMap = _mapper.Map<DMDVSearchResponseData>(d);
                dMap.Stt = i + 1;
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                if (d.PhapNhanId != 0)
                    dMap.PhapNhanName = _context.DMPN.Where(x => x.PhapNhanId == d.PhapNhanId).FirstOrDefault()?.PhapNhanName;
                else
                    dMap.PhapNhanName = "";

                dMap.CreatedateString = dMap.Createdate.ToString("HH:mm dd/MM/yyyy");
                dMap.UpdatedateString = dMap.Updatedate?.ToString("HH:mm dd/MM/yyyy");

                dataResponse.Add(dMap);
            }

            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }
    }
}