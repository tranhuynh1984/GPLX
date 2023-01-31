using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DMPN;
using GPLX.Core.DTO.Request.DMPN;
using GPLX.Core.DTO.Response.DMPN;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.DMPN
{
    public class DMPNRepository : IDMPNRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DMPNRepository> _logger;

        public DMPNRepository(Context context, IMapper mapper, ILogger<DMPNRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DMPNSearchResponse> Search(int skip, int length, DMPNSearchRequest request)
        {
            var response = new DMPNSearchResponse { Draw = request.Draw };
            try
            {
                var query = _context.DMPN.AsQueryable();
                if (!string.IsNullOrEmpty(request.MaPhapNhan))
                    query = query.Where(x => x.PhapNhanId.ToString().ToLower().Contains(request.MaPhapNhan.Trim().ToLower()));
                if (!string.IsNullOrEmpty(request.TenPhapNhan))
                    query = query.Where(x => x.PhapNhanName.ToLower().Contains(request.TenPhapNhan.Trim().ToLower()));
                if (request.Status != -1)
                    query = query.Where(x => x.IsActive == request.Status);


                var data = await query.OrderBy(x => x.PhapNhanName).ToListAsync();

                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<DMPNSearchResponseData>();
                foreach (var d in data.Skip(skip).Take(length))
                {
                    var dMap = _mapper.Map<DMPNSearchResponseData>(d);
                    dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                    dataResponse.Add(dMap);
                }
                response.Data = dataResponse;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
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

        public async Task<DMPNSearchResponse> SearchAll(DMPNSearchRequest request)
        {
            var response = new DMPNSearchResponse { Draw = request.Draw };

            var query = _context.DMPN.AsQueryable();
            if (!string.IsNullOrEmpty(request.MaPhapNhan))
                query = query.Where(x => x.PhapNhanId.ToString().ToLower().Contains(request.MaPhapNhan.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.TenPhapNhan))
                query = query.Where(x => x.PhapNhanName.ToLower().Contains(request.TenPhapNhan.Trim().ToLower()));
            if (request.Status != null && request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);

            var data = await query.OrderBy(x => x.PhapNhanId).ToListAsync();
            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMPNSearchResponseData>();

            for(var i = 0; i < data.Count; i ++)
            {
                var d = data[i];
                var dMap = _mapper.Map<DMPNSearchResponseData>(d);
                dMap.Stt = i + 1;
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
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
