using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Cells;
using AutoMapper;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.CostStatus;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.Statuses
{
    public class CostStatusesRepository : ICostStatusesRepository
    {

        private readonly Context _ctx;
        private readonly ILogger<CostStatusesRepository> _log;
        private readonly IMapper _mapper;
        private readonly IActionLogsRepository _actionLogsRepository;

        public CostStatusesRepository(Context ctx,
            ILogger<CostStatusesRepository> log, IMapper mapper, IActionLogsRepository actionLogsRepository)
        {
            _ctx = ctx;
            _log = log;
            _mapper = mapper;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<CostStatusCreateResponse> AddAsync(CostStatusCreateRequest request)
        {
            var response = new CostStatusCreateResponse();
            try
            {
                // update
                if (request.RawId != Guid.Empty)
                {
                    var record = await _ctx.CostStatuses.FirstOrDefaultAsync(x => x.Id == request.RawId);
                    if (record == null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                    {
                        record.StatusForSubject = request.StatusForSubject;
                        record.Type = request.Type;
                        record.StatusForCostEstimateType = request.StatusForCostEstimateType;
                        record.IsApprove = request.IsApprove;
                        record.Order = request.Order;
                        record.Value = request.Value;
                        record.Name = request.Name;
                        record.Singed = request.Signed;

                        _ctx.Update(record);
                        await _ctx.SaveChangesAsync();

                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            UserId = request.Creator,
                            Action = "Edit",
                            FunctionUnique = "Statuses",
                            Content = JsonConvert.SerializeObject(request)
                        });
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật trạng thái thành công!";
                    }
                }
                else
                {
                    var checkDuplicate = await _ctx.CostStatuses.FirstOrDefaultAsync(x =>
                        (x.Name == request.Name || x.Value == request.Value) && x.StatusForSubject == request.StatusForSubject &&
                        x.StatusForCostEstimateType == request.StatusForCostEstimateType && x.Type == request.Type);
                    if (checkDuplicate != null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Trạng thái đã tồn tại!";
                    }
                    else
                    {
                        var insertRecord = new CostStatuses
                        {
                            Id = Guid.NewGuid(),
                            Value = request.Value,
                            Status = (int)GlobalEnums.StatusDefaultEnum.Active,
                            Name = request.Name,
                            Type = request.Type,
                            StatusForSubject = request.StatusForSubject,
                            StatusForCostEstimateType = request.StatusForCostEstimateType,
                            IsApprove = request.IsApprove,
                            Order = request.Order,
                            Singed = request.Signed
                        };
                        await _ctx.CostStatuses.AddAsync(insertRecord);
                        _ctx.SaveChanges();

                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            UserId = request.Creator,
                            Action = "Add",
                            FunctionUnique = "Statuses",
                            Content = JsonConvert.SerializeObject(request)
                        });
                        var specifyUnit = await GetSpecialUnitFollowConfigs();

                        var data = _mapper.Map<CostStatusSearchResponseData>(insertRecord);
                        data.StatusName = GlobalEnums.OtherStatusNames[insertRecord.Status];
                        data.Record = insertRecord.Id.ToString().StringAesEncryption(request.RequestPage);
                        data.StatusForCostEstimateType = GlobalEnums.CostEstimateTypeNames[data.StatusForCostEstimateType];
                        data.StatusForSubject = GlobalEnums.ObjectNames.TryGetValue(data.StatusForSubject, out var g) ? g : specifyUnit.FirstOrDefault(x => x.UnitCode.Equals(data.StatusForSubject))?.UnitShortName;
                        data.Type = GlobalEnums.TypeNames[data.Type];
                        response.Data = data;

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới trạng thái thành công!";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<IList<StatusesGranted>> ListStatusesForSubject(DataSeenRequest request)
        {
            try
            {

                var cQuery = from ct in _ctx.CostStatuses
                    join ctg in _ctx.CostStatusesGroups on ct.Id equals ctg.StatusesId
                    join g in _ctx.Groups on ctg.GroupCode equals g.GroupCode
                    where ct.Status ==
                        (int) GlobalEnums.StatusDefaultEnum.Active && ct.StatusForCostEstimateType.Equals(
                                                                       request.CostEstimateType)
                                                                   && ct.Type.Equals(request.Type)
                    select new
                    {
                        CostStatuses = ct,
                        CostStatusesGroups = ctg,
                        PositionName = g.Name
                    };



                     var query = await cQuery.ToListAsync();

                query =
                    query.Where(c => request.GroupCodes.Any(m =>
                        m.Equals(c.CostStatusesGroups.GroupCode, StringComparison.CurrentCultureIgnoreCase))).ToList();

                var response = query.Select(p => new StatusesGranted
                {
                    Id = p.CostStatuses.Id,
                    Value = p.CostStatuses.Value,
                    Status = p.CostStatuses.Status,
                    Name = p.CostStatuses.Name,
                    StatusForSubject = p.CostStatuses.StatusForSubject,
                    Type = p.CostStatuses.Type,
                    StatusForCostEstimateType = p.CostStatuses.StatusForCostEstimateType,
                    IsApprove = p.CostStatuses.IsApprove,
                    Order = p.CostStatuses.Order,
                    Used = !string.IsNullOrEmpty(p.CostStatusesGroups.Type) &&
                           request.GroupCodes.Contains(p.CostStatusesGroups.GroupCode),
                    Sign = !string.IsNullOrEmpty(p.CostStatusesGroups.Type) && p.CostStatuses.Singed,
                    PositionCode = p.CostStatusesGroups.GroupCode,
                    PositionName = p.PositionName
                }).ToList();
                return response;
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message, request);
                Log.Error(e, e.Message, request);
                return new List<StatusesGranted>();
            }
        }

        public async Task<CostStatusSearchResponse> Search(int skip, int length, CostStatusSearchRequest request)
        {
            var response = new CostStatusSearchResponse();
            try
            {
                var specifyUnit = await GetSpecialUnitFollowConfigs();
                var query = _ctx.CostStatuses.Where(x =>
                        x.Status == request.Status || request.Status == (int)GlobalEnums.StatusDefaultEnum.All)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.Name.ToLower().Contains(request.Keywords.ToLower()));
                if (!string.IsNullOrEmpty(request.StatusForSubject))
                    query = query.Where(x => x.StatusForSubject.Equals(request.StatusForSubject));
                if (!string.IsNullOrEmpty(request.StatusForCostEstimateType))
                    query = query.Where(x => x.StatusForCostEstimateType.Equals(request.StatusForCostEstimateType));
                if (!string.IsNullOrEmpty(request.Type))
                    query = query.Where(x => x.Type.Equals(request.Type));
                var dataResponse = new List<CostStatusSearchResponseData>();
                var data = await query.OrderBy(x => x.Order).ToListAsync();
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Draw = request.Draw;
                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                foreach (var d in data.Skip(skip).Take(length))
                {
                    var item = _mapper.Map<CostStatusSearchResponseData>(d);
                    item.StatusName = GlobalEnums.OtherStatusNames[d.Status];
                    item.Record = d.Id.ToString().StringAesEncryption(request.RequestPage);
                    item.StatusForCostEstimateType = GlobalEnums.CostEstimateTypeNames[item.StatusForCostEstimateType];
                    item.StatusForSubject = GlobalEnums.ObjectNames.TryGetValue(item.StatusForSubject, out var v) ? v
                        : specifyUnit.FirstOrDefault(x => x.UnitCode.Equals(item.StatusForSubject, StringComparison.CurrentCultureIgnoreCase))?.UnitShortName;

                    item.Type = GlobalEnums.TypeNames[item.Type];
                    dataResponse.Add(item);
                }

                response.Data = dataResponse;
                return response;
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message, request);
                Log.Error(e, e.Message, request);
                return new CostStatusSearchResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.NoContentMessage
                };
            }
        }

        public async Task<CostStatusCreateResponse> ChangeStatus(CostStatusCreateRequest request)
        {
            var response = new CostStatusCreateResponse();
            try
            {
                // update
                if (request.RawId != Guid.Empty)
                {
                    var record = await _ctx.CostStatuses.FirstOrDefaultAsync(x => x.Id == request.RawId);
                    if (record == null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                    {
                        record.Status = request.Status;
                        _ctx.Update(record);
                        await _ctx.SaveChangesAsync();

                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            UserId = request.Creator,
                            Action = "Edit",
                            FunctionUnique = "ChangeStatus",
                            Content = JsonConvert.SerializeObject(request)
                        });
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật trạng thái thành công!";
                    }
                }
                else
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = GlobalEnums.NoContentMessage;
                }
                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<CostStatuses> GetById(Guid id)
        {
            try
            {
                var record = await _ctx.CostStatuses.FirstOrDefaultAsync(x => x.Id == id);
                return record;
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<IList<Database.Models.Groups>> GetGrantStatuses(Guid id)
        {
            try
            {
                var join = from g in _ctx.Groups
                           join map in _ctx.CostStatusesGroups on g.GroupCode equals map.GroupCode
                           join s in _ctx.CostStatuses on map.StatusesId equals s.Id
                           where s.Id == id
                           select g;
                var data = await join.ToListAsync();
                return data;
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<Database.Models.Groups> GetUsedByGroup(Guid id)
        {
            try
            {
                var join = from g in _ctx.Groups
                           join map in _ctx.CostStatusesGroups on g.GroupCode equals map.GroupCode
                           join s in _ctx.CostStatuses on map.StatusesId equals s.Id
                           where s.Id == id && !string.IsNullOrEmpty(map.Type)
                           select g;
                var data = await join.FirstOrDefaultAsync();
                return data;
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<CostStatusesGrantResponse> SetGrant(CostStatusesGrantRequest request)
        {
            var response = new CostStatusesGrantResponse();
            try
            {
                if (request.RawId == Guid.Empty)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = GlobalEnums.NoContentMessage;
                }
                else
                {
                    var transaction = await _ctx.Database.BeginTransactionAsync();
                    await transaction.CreateSavepointAsync("Before");
                    try
                    {
                        var join = from g in _ctx.Groups
                                   join map in _ctx.CostStatusesGroups on g.GroupCode equals map.GroupCode
                                   join s in _ctx.CostStatuses on map.StatusesId equals s.Id
                                   where s.Id == request.RawId
                                   select new { G = g, map };
                        var dataJoin = await join.ToListAsync();
                        // danh sách các group được bỏ gán
                        var listLefts = dataJoin.Where(x =>
                            request.Grants.All(m => !m.Equals(x.G.GroupCode, StringComparison.OrdinalIgnoreCase))
                            && !(x.G.GroupCode.Equals(request.Used) && !string.IsNullOrEmpty(x.map.Type))).ToList();

                        if (listLefts.Any())
                        {
                            _ctx.CostStatusesGroups.RemoveRange(listLefts.Select(x => x.map));
                            await _ctx.SaveChangesAsync();
                        }

                        var listCreate = request.Grants.Where(x =>
                            dataJoin.All(m => !m.G.GroupCode.Equals(x, StringComparison.OrdinalIgnoreCase))).ToList();
                        if (listCreate.Any())
                        {
                            var creates = listCreate.Select(x => new CostStatusesGroups
                            {
                                Id = Guid.NewGuid(),
                                StatusesId = request.RawId,
                                GroupCode = x
                            });
                            await _ctx.AddRangeAsync(creates);
                            await _ctx.SaveChangesAsync();
                        }

                        var listUsedRemove = dataJoin.Where(x => !string.IsNullOrEmpty(x.map.Type)).ToList();
                        if (listUsedRemove.Any())
                            _ctx.CostStatusesGroups.RemoveRange(listUsedRemove.Select(x => x.map));

                        if (!string.IsNullOrEmpty(request.Used))
                        {
                            await _ctx.AddAsync(new CostStatusesGroups
                            {
                                Id = Guid.NewGuid(),
                                StatusesId = request.RawId,
                                GroupCode = request.Used,
                                Type = "Used"
                            });
                        }
                        await _ctx.SaveChangesAsync();
                        await transaction.CommitAsync();

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật gán quyền thành công!";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackToSavepointAsync("Before");
                        _log.Log(LogLevel.Error, e, e.Message);
                        Log.Error(e, e.Message);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = GlobalEnums.ErrorMessage;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }

            return response;
        }

        public async Task<IList<CostStatuses>> GetAll()
        {
            try
            {
                var q = _ctx.CostStatuses.Where(x => x.Status == 1);
                return await q.ToListAsync();
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<IList<CostStatusFollowResponse>> GetFollow(GetFollowRequest request)
        {
            try
            {
                var response = new List<CostStatusFollowResponse>();
                var q = _ctx.CostStatuses.Where(x => x.Status == 1 && x.StatusForSubject.Equals(request.StatusForSubject) && x.Type.Equals(request.Type) && x.StatusForCostEstimateType.Equals(request.StatusForCostEstimateType));

                q = q.OrderBy(x => x.Order).ThenByDescending(x => x.IsApprove);

                var primaries = await q.ToListAsync();
                var allGroupStatus = await _ctx.CostStatusesGroups.ToListAsync();
                var allGroups = await _ctx.Groups.Where(x => x.Status == 1).ToListAsync();

                foreach (var p in primaries)
                {
                    var use = allGroupStatus.FirstOrDefault(x => x.StatusesId == p.Id && !string.IsNullOrEmpty(x.Type));
                    var listGrants = allGroupStatus.Where(x => x.StatusesId == p.Id && string.IsNullOrEmpty(x.Type)).ToList();
                    var cf = new CostStatusFollowResponse
                    {
                        Groups = listGrants.Count > 0 ?
                            allGroups.Where(x => listGrants.Any(c => c.GroupCode.Equals(x.GroupCode))).OrderBy(x => x.Order).ToList() : new List<Database.Models.Groups>(),
                        CostStatuses = p,
                        Use = use != null ? allGroups.FirstOrDefault(x => x.GroupCode == use.GroupCode) : null
                    };
                    response.Add(cf);
                }

                return response;
            }
            catch (Exception e)
            {
                _log.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<IList<SpecialUnitFollowConfigs>> GetSpecialUnitFollowConfigs()
        {
            try
            {
                var q = _ctx.SpecialUnitFollowConfigs.ToListAsync();
                return await q;
            }
            catch (Exception e)
            {
                Log.Error(e, "error");
                return new List<SpecialUnitFollowConfigs>();
            }
        }

        /// <summary>
        /// Kiểm tra file có đủ vị trí ký số + ký điện tử hay không
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="type"></param>
        /// <param name="time"></param>
        /// <param name="subject"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public async Task<string> ValidateSignerPositionInExcel(Worksheet ws, string type, string time, string subject, int unitId)
        {
            try
            {
                var specifiesUnit = await GetSpecialUnitFollowConfigs();
                var spec = specifiesUnit?.FirstOrDefault(c => c.UnitId == unitId);
                if (spec != null)
                    subject = spec.UnitCode;
                var allSingerPositionOfTypeAndSubject = await _ctx.CostStatuses.Join(_ctx.CostStatusesGroups, x => x.Id, y => y.StatusesId,
                        (x, y) => new
                        {
                            CostStatuses = x,
                            CostStatusesGroups = y
                        }).Where(p => p.CostStatuses.Status == (int)GlobalEnums.StatusDefaultEnum.Active
                                && p.CostStatuses.StatusForCostEstimateType == time
                                && p.CostStatuses.Type == type
                                && p.CostStatuses.StatusForSubject.ToLower().Equals(subject.ToLower())
                                && p.CostStatuses.IsApprove == 1
                                && p.CostStatuses.Status == 1
                                && p.CostStatusesGroups.Type.Equals("Used")
                    ).Select(c => c.CostStatusesGroups.GroupCode)
                    .ToListAsync();
                allSingerPositionOfTypeAndSubject = allSingerPositionOfTypeAndSubject.Distinct().ToList();
                if (allSingerPositionOfTypeAndSubject.Count == 0)
                    return string.Empty;

                var allGroupSinger = await _ctx.Groups.Where(c => allSingerPositionOfTypeAndSubject.Contains(c.GroupCode))
                    .ToListAsync();
                if (allGroupSinger.Count == 0)
                    return string.Empty;

                Dictionary<string, bool> dictionariesFoundPosition = new Dictionary<string, bool>();
                foreach (var ag in allGroupSinger)
                    dictionariesFoundPosition.Add(ag.GroupCode, false);

                var cells = ws.Cells;
                foreach (Cell cell in cells)
                {
                    foreach (var ag in allGroupSinger)
                    {
                        if (cell.StringValue.Trim().StartsWith(ag.Name.Trim(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            dictionariesFoundPosition[ag.GroupCode] = true;
                            break;
                        }

                        if (ag.GroupCode.StartsWith("KTT", StringComparison.CurrentCultureIgnoreCase) 
                            && cell.StringValue.Trim().StartsWith("Kế toán trưởng", StringComparison.CurrentCultureIgnoreCase))
                        {
                            dictionariesFoundPosition[ag.GroupCode] = true;
                            break;
                        }
                    }
                }

                foreach (var dp in dictionariesFoundPosition)
                {
                    if (!dp.Value)
                        return $"Không tìm thấy vị trí ký của <b>{allGroupSinger.First(c => c.GroupCode == dp.Key).Name}</b> trong biểu mẫu!";
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                Log.Error(e, "Message");
                return string.Empty;
            }
        }
    }
}
