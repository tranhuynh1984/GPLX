using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.Unit;
using GPLX.Core.DTO.Response.Unit;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.Unit
{
    public class UnitRepository : IUnitRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UnitRepository> _logger;

        public UnitRepository(Context context, IMapper mapper, ILogger<UnitRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddRangeAsync(IList<Units> lst)
        {
            try
            {
                var transaction = _context.Database.BeginTransaction();
                await transaction.CreateSavepointAsync("Before");
                try
                {
                    var listUnits = await _context.Units.ToListAsync();
                    var lstInsert = lst.Where(a => listUnits.All(x => !x.OfficesCode.Equals(a.OfficesCode, StringComparison.OrdinalIgnoreCase))).ToList();
                    lstInsert.ForEach(x => { x.Id = 0; });
                    await _context.AddRangeAsync(lstInsert);
                    _context.SaveChanges();

                    var listOlder = lst.Where(a => listUnits.Any(x => x.OfficesCode.Equals(a.OfficesCode, StringComparison.OrdinalIgnoreCase))).ToList();

                    var listUpdates = new List<Units>();
                    var listDelete = listUnits.Where(x => lst.All(c => !c.OfficesCode.Equals(x.OfficesCode))).ToList();
                    foreach (var old in listOlder)
                    {
                        var oDbUnit = listUnits.First(x => x.OfficesCode.Equals(old.OfficesCode));
                        oDbUnit.OfficesName = old.OfficesName;
                        oDbUnit.OfficesAddress = old.OfficesAddress;
                        oDbUnit.OfficesSub = old.OfficesSub;
                        oDbUnit.OfficesContact = old.OfficesContact;
                        oDbUnit.OfficesGuid = old.OfficesGuid;
                        oDbUnit.TenantID = old.TenantID;
                        oDbUnit.Status = old.Status;
                        oDbUnit.TenantName = old.TenantName;
                        //oDbUnit.OfficesShortName = old.OfficesShortName;

                        listUpdates.Add(oDbUnit);
                    }

                    if (listDelete.Count > 0)
                        _context.RemoveRange(listDelete);

                    if (listUpdates.Count > 0)
                        _context.UpdateRange(listUpdates);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, e, e.Message);
                    Log.Error(e, e.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return false;
            }

        }

        public async Task<UnitSearchResponse> Search(int skip, int length, UnitSearchRequest request)
        {
            var response = new UnitSearchResponse { Draw = request.Draw };
            try
            {
                var query = _context.Units.AsQueryable();
                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.OfficesDesc.ToLower().Contains(request.Keywords.Trim().ToLower())
                                             || x.OfficesCode.ToLower().Contains(request.Keywords.Trim().ToLower())
                                             || x.OfficesShortName.ToLower().Contains(request.Keywords.Trim().ToLower()));
                var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<UnitSearchResponseData>();

                foreach (var d in data.Skip(skip).Take(length))
                {
                    string gUnitType = (d.OfficesSub?? string.Empty).Equals("sub", StringComparison.CurrentCultureIgnoreCase) ? GlobalEnums.ObjectSub : (d.OfficesSub??string.Empty).Equals("YT", StringComparison.CurrentCultureIgnoreCase) ? GlobalEnums.UnitIn : GlobalEnums.UnitOut;
                    var dMap = _mapper.Map<UnitSearchResponseData>(d);
                    dMap.TypeName = !string.IsNullOrEmpty(gUnitType) ? GlobalEnums.ObjectNames[gUnitType] : string.Empty;
                    dMap.OfficesShortName ??= d.OfficesName;
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

        public async Task<UnitSetTypeResponse> SetInOut(UnitSetTypeRequest request)
        {
            var transaction = _context.Database.BeginTransaction();
            var response = new UnitSetTypeResponse();
            try
            {
                transaction.CreateSavepoint("Before");
                var join = await _context.UnitTypeMaps.FirstOrDefaultAsync(x => x.UnitCode.ToLower().Equals(request.UnitCode.ToLower()));
                if (join == null)
                {
                    await _context.UnitTypeMaps.AddAsync(new UnitTypeMap
                    {
                        Id = Guid.NewGuid(),
                        Type = request.Type,
                        UnitCode = request.UnitCode,
                        TypeName = GlobalEnums.UnitTypeNames.FirstOrDefault(x => string.Equals(x.Key, request.Type, StringComparison.OrdinalIgnoreCase)).Value
                    });
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Gán loại đơn vị thành công!";
                }
                else
                {
                    _context.UnitTypeMaps.Remove(join);
                    await _context.SaveChangesAsync();
                    await _context.UnitTypeMaps.AddAsync(new UnitTypeMap
                    {
                        Id = Guid.NewGuid(),
                        Type = request.Type,
                        UnitCode = request.UnitCode,
                        TypeName = GlobalEnums.UnitTypeNames.FirstOrDefault(x => string.Equals(x.Key, request.Type, StringComparison.OrdinalIgnoreCase)).Value
                    });
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Gán loại đơn vị thành công!";
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackToSavepointAsync("Before");
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
                _logger.Log(LogLevel.Error, e.Message);
                Log.Error(e, e.Message);

            }

            return response;
        }

        public async Task<string> GetUnitType(string code)
        {
            try
            {
                var join = await _context.UnitTypeMaps.FirstOrDefaultAsync(x => x.UnitCode.ToLower().Equals(code.ToLower()));
                return join?.Type;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                Log.Error(e, e.Message);

                return string.Empty;
            }
        }

        public async Task<Units> GetByCodeAsync(string offCodes)
        {
            try
            {
                return await _context.Units.FirstOrDefaultAsync(a => a.OfficesCode == offCodes);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<IList<Units>> GetAllAsync(string name, int offset, int limit)
        {
            try
            {
                var result = _context.Units.AsQueryable();
                if (!string.IsNullOrEmpty(name))
                    result = result.Where(a => a.OfficesName.Contains(name));

                return (await result.ToListAsync()).Skip(offset).Take(limit).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return new List<Units>();
            }
        }

        public async Task<Units> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Units.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }
    }
}
