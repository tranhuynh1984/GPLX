using AutoMapper;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.CostEstimate;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Core.DTO.Response.CashFollow;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.DTO.Response.Department;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.InvestmentPlan;
using GPLX.Core.DTO.Response.ProfitPlan;
using GPLX.Core.DTO.Response.RevenuePlan;
using GPLX.Core.DTO.Response.Unit;
using GPLX.Core.DTO.Response.Users;
using GPLX.Database.Models;
using GPLX.Core.DTO.Response.DMPN;
using GPLX.Core.DTO.Response.DMDV;
using GPLX.Core.DTO.Response.DMHuyen;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.HDKCB;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.DTO.Response.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Response.LoaiDeXuat;
using GPLX.Core.DTO.Response.DMCTV;
using GPLX.Core.DTO.Response.TBL_CTVGROUP;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB2_DETAIL;
using GPLX.Core.DTO.Response.ProfileCK;
using GPLX.Core.DTO.Response.Relationship;
using GPLX.Core.DTO.Response.ProfileCKCP;
using GPLX.Core.DTO.Response.DeXuat;
using GPLX.Core.DTO.Response.DeXuatChiTiet;
using GPLX.Database.Models.Phase2;
using GPLX.Core.DTO.Response.ProcessStep;
using GPLX.Core.DTO.Response.Process;
using GPLX.Core.DTO.Response.DeXuatGhiChu;
using GPLX.Core.DTO.Response.DeXuatKhoaMaCTV;
using GPLX.Core.DTO.Response.DeXuatLuanChuyenMa;

namespace GPLX.Infrastructure.Configs
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<CostEstimateItemCreateRequest, CostEstimateItem>();
            CreateMap<CostEstimateItem, CostEstimateItemCreateRequest>();
            CreateMap<ActuallySpentItem, ActuallySpentItemResponse>();
            CreateMap<CostEstimateUpdate, Database.Models.CostEstimate>();
            CreateMap<ActuallySpentItemResponse, ActuallySpentItem>();
            CreateMap<ActuallySpent, SearchActuallySpentData>();
            CreateMap<CostEstimateItemFromExcel, CostEstimateItem>();
            CreateMap<Database.Models.CostEstimate, SearchCostEstimateResponseData>();
            CreateMap<SCTView, SctData>();
            CreateMap<ExportCostEstimateModel, CostEstimateUpdate>();

            CreateMap<Departments, DepartmentSearchResponseData>();
            CreateMap<Groups, GroupsSearchResponseData>();
            CreateMap<CostStatuses, CostStatusSearchResponseData>();
            CreateMap<Units, UnitSearchResponseData>();
            CreateMap<Users, UsersSearchResponseData>();
            CreateMap<UserSync, Users>();
            CreateMap<InvestmentPlan, SearchInvestmentPlanResponseData>();
            CreateMap<RevenuePlan, SearchRevenuePlanResponseData>();

            CreateMap<ProfitPlan, SearchProfitPlanResponseData>();
            CreateMap<ProfitPlanAggregatesExcel, ProfitPlanAggregates>();
            CreateMap<ProfitPlanDetailExcel, ProfitPlanDetails>();

            CreateMap<CashFollowAggregateExcel, CashFollowAggregates>();
            CreateMap<CashFollowAggregates, CashFollowAggregateExcel>();

            CreateMap<CashFollowItemExcel, CashFollowItem>();
            CreateMap<CashFollowItem, CashFollowItemExcel>();
            CreateMap<CostEstimateItem, CostEstimateItemSearchResponseData>();

            CreateMap<DMPN, DMPNSearchResponseData>();
            CreateMap<DMDV, DMDVSearchResponseData>();
            CreateMap<DMHuyen, DMHuyenSearchResponseData>();
            CreateMap<DM, DMSearchResponseData>();
            CreateMap<HDKCB, HDKCBSearchResponseData>();
            CreateMap<DMCP, DMCPSearchResponseData>();
            CreateMap<DMCPSearchResponseData, DMCPSearchResponseData>();
            CreateMap<DMBS_ChuyenKhoa, DMBS_ChuyenKhoaSearchResponseData>();
            CreateMap<LoaiDeXuat, LoaiDeXuatSearchResponseData>();
            //CreateMap<HDCTV, DMCTVSearchResponseData>();

            CreateMap<TBL_CTVGROUP, TBL_CTVGROUPSearchResponseData>();
            CreateMap<TBL_CTVGROUPSUB, TBL_CTVGROUPSUBSearchResponseData>();
            CreateMap<TBL_CTVGROUPSUB1_DETAIL, TBL_CTVGROUPSUB1_DETAILSearchResponseData>();
            CreateMap<TBL_CTVGROUPSUB2_DETAIL, TBL_CTVGROUPSUB2_DETAILSearchResponseData>();

            CreateMap<ProfileCK, ProfileCKSearchResponseData>();
            CreateMap<ProfileCKCP, ProfileCKCPSearchResponseData>();
            CreateMap<Relationship, RelationshipSearchResponseData>();
            CreateMap<DeXuat, DeXuatSearchResponseData>();
            CreateMap<DeXuatChiTiet, DeXuatChiTietSearchResponseData>();
            CreateMap<DeXuatKhoaMaCTV, DeXuatKhoaMaCTVSearchResponseData>();
            CreateMap<DeXuatLuanChuyenMa, DeXuatLuanChuyenMaSearchResponseData>();
            CreateMap<DeXuatGhiChu, DeXuatGhiChuSearchResponseData>();
            CreateMap<Process, ProcessSearchResponseData>();
            CreateMap<ProcessStep, ProcessStepSearchResponseData>();
            CreateMap<DMCTV, DMCTVSearchResponseData>();
        }
    }
}