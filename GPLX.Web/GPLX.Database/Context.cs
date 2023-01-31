using Microsoft.EntityFrameworkCore;
using model = GPLX.Database.Models;

namespace GPLX.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<model.ActionLogs> ActionLogs { get; set; }
        public DbSet<model.CostEstimate> CostEstimates { get; set; }
        public DbSet<model.CostEstimateItem> CostEstimateItems { get; set; }
        public DbSet<model.CostEstimateItemLogs> CostEstimateItemLogs { get; set; }
        public DbSet<model.CostEstimateLogs> CostEstimateLogs { get; set; }
        public DbSet<model.Functions> Functions { get; set; }
        public DbSet<model.CostStatuses> CostStatuses { get; set; }
        public DbSet<model.CostStatusesGroups> CostStatusesGroups { get; set; }
        public DbSet<model.Suppliers> Suppliers { get; set; }
        public DbSet<model.CostEstimateMapItems> CostEstimateMapItems { get; set; }
        public DbSet<model.ActuallySpent> ActuallySpent { get; set; }
        public DbSet<model.ActuallySpentItem> ActuallySpentItem { get; set; }
        public DbSet<model.ActuallySpentMapItem> ActuallySpentMapItem { get; set; }
        public DbSet<model.CostEstimateItemMapActuallySpentItem> CostEstimateItemMapActuallySpentItem { get; set; }
        public DbSet<model.ActuallySpentLog> ActuallySpentLog { get; set; }
        public DbSet<model.CostEstimateItemType> CostEstimateItemTypes { get; set; }
        public DbSet<model.Payment> Payments { get; set; }
        public DbSet<model.SctData> SctData { get; set; }
        public DbSet<model.CashFollowGroup> CashFollowGroups { get; set; }
        public DbSet<model.CashFollow> CashFollow { get; set; }
        public DbSet<model.CashFollowItem> CashFollowItem { get; set; }
        public DbSet<model.CashFollowAggregates> CashFollowAggregates { get; set; }
        public DbSet<model.CashFollowLog> CashFollowLog { get; set; }

        public DbSet<model.Users> Users { get; set; }
        public DbSet<model.Units> Units { get; set; }
        public DbSet<model.Groups> Groups { get; set; }
        public DbSet<model.GroupsPermission> GroupsPermission { get; set; }
        public DbSet<model.UnitTypeMap> UnitTypeMaps { get; set; }
        public DbSet<model.Departments> Departments { get; set; }
        public DbSet<model.InvestmentPlanContents> InvestmentPlanContents { get; set; }
        public DbSet<model.InvestmentPlan> InvestmentPlan { get; set; }
        public DbSet<model.InvestmentPlanDetails> InvestmentPlanDetails { get; set; }
        public DbSet<model.InvestmentPlanAggregate> InvestmentPlanAggregate { get; set; }
        public DbSet<model.InvestmentPlanLogs> InvestmentPlanLogs { get; set; }

        public DbSet<model.RevenuePlanContents> RevenuePlanContents { get; set; }
        public DbSet<model.RevenuePlanAggregate> RevenuePlanAggregate { get; set; }
        public DbSet<model.RevenuePlanCashDetails> RevenuePlanCashDetails { get; set; }
        public DbSet<model.RevenuePlan> RevenuePlan { get; set; }
        public DbSet<model.RevenuePlanLogs> RevenuePlanLogs { get; set; }
        public DbSet<model.RevenuePlanCustomerDetails> RevenuePlanCustomerDetails { get; set; }
        public DbSet<model.ProfitPlan> ProfitPlan { get; set; }
        public DbSet<model.ProfitPlanGroups> ProfitPlanGroups { get; set; }
        public DbSet<model.ProfitPlanAggregates> ProfitPlanAggregates { get; set; }
        public DbSet<model.ProfitPlanDetails> ProfitPlanDetails { get; set; }
        public DbSet<model.ProfitPlanLogs> ProfitPlanLogs { get; set; }
        public DbSet<model.UserUnitsManages> UserUnitsManages { get; set; }
        public DbSet<model.UserGroups> UserGroups { get; set; }
        public DbSet<model.UserConcurrently> UserConcurrently { get; set; }
        public DbSet<model.FilePdfCreateLogs> FilePdfCreateLogs { get; set; }
        public DbSet<model.SpecialUnitFollowConfigs> SpecialUnitFollowConfigs { get; set; }
        public DbSet<model.Notify> Notify { get; set; }

        public DbSet<model.DM> DM { get; set; }
        public DbSet<model.DMBS_ChuyenKhoa> DMBS_ChuyenKhoa { get; set; }
        public DbSet<model.DMCP> DMCP { get; set; }
        public DbSet<model.DMDV> DMDV { get; set; }
        public DbSet<model.DMHuyen> DMHuyen { get; set; }
        public DbSet<model.DMPN> DMPN { get; set; }

        //public DbSet<model.HDCTV> HDCTV { get; set; }
        public DbSet<model.HDCTVExt> HDCTVExt { get; set; }

        public DbSet<model.HDKCB> HDKCB { get; set; }
        public DbSet<model.LoaiDeXuat> LoaiDeXuat { get; set; }
        public DbSet<model.ProfileCK> ProfileCK { get; set; }
        public DbSet<model.Relationship> Relationship { get; set; }
        public DbSet<model.CLS> CLS { get; set; }
        public DbSet<model.CLSCT> CLSCT { get; set; }
        public DbSet<model.DKK> DKK { get; set; }
        public DbSet<model.NhCP> NhCP { get; set; }
        public DbSet<model.DMBangGiaChiNhanh> DMBangGiaChiNhanh { get; set; }

        public DbSet<model.DMCTV> DMCTV { get; set; }
        public DbSet<model.DMCTVExt> DMCTVExt { get; set; }
        public DbSet<model.TBL_CTVGROUP> TBL_CTVGROUP { get; set; }
        public DbSet<model.TBL_CTVGROUPSUB> TBL_CTVGROUPSUB { get; set; }
        public DbSet<model.TBL_CTVGROUPSUB1_DETAIL> TBL_CTVGROUPSUB1_DETAIL { get; set; }
        public DbSet<model.TBL_CTVGROUPSUB2_DETAIL> TBL_CTVGROUPSUB2_DETAIL { get; set; }
        public DbSet<model.ProfileCKCP> ProfileCKCP { get; set; }

        public DbSet<model.ProcessMapRole> ProcessMapRole { get; set; }
        public DbSet<model.ProcessRole> ProcessRole { get; set; }

        public DbSet<model.DeXuatChiTiet> DeXuatChiTiet { get; set; }
        public DbSet<model.DeXuat> DeXuat { get; set; }
        public DbSet<model.DeXuatGhiChu> DeXuatGhiChu { get; set; }
        public DbSet<model.DeXuatLuanChuyenMa> DeXuatLuanChuyenMa { get; set; }
        public DbSet<model.DeXuatKhoaMaCTV> DeXuatKhoaMaCTV { get; set; }
        public DbSet<model.DMGG> DMGG { get; set; }
        public DbSet<model.GiaKCB> GiaKCB { get; set; }

        public DbSet<GPLX.Database.Models.Phase2.Process> Process { get; set; }
        public DbSet<GPLX.Database.Models.Phase2.ProcessStep> ProcessStep { get; set; }
        public DbSet<GPLX.Database.Models.Phase2.ApprovedFlow> ApprovedFlow { get; set; }
        public DbSet<GPLX.Database.Models.Phase2.ApprovedFlowDetail> ApprovedFlowDetail { get; set; }
        public DbSet<GPLX.Database.Models.Phase2.TBL_LOGCTVGROUP> TBL_LOGCTVGROUP { get; set; }

        public DbSet<model.NextStep.Teacher> Teacher { get; set; }
        public DbSet<model.NextStep.Student> Student { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Phase1

            modelBuilder.Entity<model.ActionLogs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).HasDefaultValueSql("NEWID()");
                b.HasIndex(m => new { m.UserId, m.CreatedDate, m.Action });
                b.HasIndex(m => new { m.UserName, m.CreatedDate, m.Action });
                b.ToTable("ActionLogs");
            });

            modelBuilder.Entity<model.CostEstimate>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).HasDefaultValueSql("NEWID()");
                b.HasIndex(m => new { m.UserId, m.Status, m.CreatedDate });
                b.ToTable("CostEstimate");
            });

            modelBuilder.Entity<model.CostEstimateItem>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).HasDefaultValueSql("NEWID()");
                b.HasIndex(m => new { m.RequestCode }).IsUnique();
                b.HasIndex(m => new { m.CreatorId, m.Status });
                b.ToTable("CostEstimateItem");
            });

            modelBuilder.Entity<model.CostEstimateItemLogs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).HasDefaultValueSql("NEWID()");
                b.HasIndex(m => new { m.UserId, m.CreatedDate });
                b.ToTable("CostEstimateItemLogs");
            });

            modelBuilder.Entity<model.CostEstimateLogs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).HasDefaultValueSql("NEWID()");
                b.HasIndex(m => new { m.UserId, m.CreatedDate });
                b.ToTable("CostEstimateLogs");
            });

            modelBuilder.Entity<model.Functions>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.ToTable("Functions");
            });

            modelBuilder.Entity<model.CostStatuses>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Type, m.Status });
                b.ToTable("CostStatuses");
            });

            modelBuilder.Entity<model.CostStatusesGroups>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.GroupCode, m.StatusesId });
                b.HasIndex(m => new { m.GroupCode });
                b.ToTable("CostStatusesGroups");
            });

            modelBuilder.Entity<model.Suppliers>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.SupplierName });
                b.ToTable("Suppliers");
            });

            modelBuilder.Entity<model.CostEstimateMapItems>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.CostEstimateId, m.CostEstimateItemId, m.Status });
                b.ToTable("CostEstimateMapItems");
            });

            modelBuilder.Entity<model.ActuallySpent>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.UnitId, m.ReportForWeek, m.Id });
                b.ToTable("ActuallySpent");
            });

            modelBuilder.Entity<model.ActuallySpentItem>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id, m.ActualSpentType, m.RequestPayWeek });
                b.ToTable("ActuallySpentItem");
            });

            modelBuilder.Entity<model.ActuallySpentMapItem>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id, m.ActuallySpentId, m.ActuallySpentItemId });
                b.ToTable("ActuallySpentMapItem");
            });

            modelBuilder.Entity<model.CostEstimateItemMapActuallySpentItem>(b =>
            {
                b.HasKey(m => m.Id).IsClustered();
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.ActuallySpentItemId, m.CostEstimateItemId });
                b.HasIndex(m => new { m.Status, m.CostEstimateItemId, m.ActuallySpentItemId });
                b.ToTable("CostEstimateItemMapActuallySpentItem");
            });

            modelBuilder.Entity<model.CostEstimateItemType>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.ToTable("CostEstimateItemType");
            });
            modelBuilder.Entity<model.CostEstimateItemType>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.ToTable("CostEstimateItemType");
            });

            modelBuilder.Entity<model.ActuallySpentLog>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.ActuallySpentId, m.ToStatus });
                b.ToTable("ActuallySpentLog");
            });

            modelBuilder.Entity<model.SctData>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Type, m.Status });
                b.ToTable("SctData");
            });

            modelBuilder.Entity<model.CashFollowGroup>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id });
                b.HasIndex(m => new { m.ForSubject });
                b.ToTable("CashFollowGroups");
            });

            modelBuilder.Entity<model.CashFollow>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id });
                b.HasIndex(m => new { m.UnitId, m.Year, m.Status });
                b.ToTable("CashFollow");
            });

            modelBuilder.Entity<model.CashFollowAggregates>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id });
                b.HasIndex(m => new { m.CashFollowId });
                b.ToTable("CashFollowAggregates");
            });

            modelBuilder.Entity<model.Users>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.HasIndex(m => new { m.UserName });
                b.HasIndex(m => new { m.Id });
                b.ToTable("Users");
            });

            modelBuilder.Entity<model.Units>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.HasIndex(m => new { m.OfficesName });
                b.HasIndex(m => new { m.Id });
                b.ToTable("Units");
            });

            modelBuilder.Entity<model.Groups>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.Property(m => m.GroupCode);
                b.HasIndex(m => new { m.GroupCode }).IsUnique();
                b.HasIndex(m => new { m.Id });
                b.ToTable("Groups");
            });

            modelBuilder.Entity<model.CashFollowItem>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id });
                b.HasIndex(m => new { m.CashFollowId });
                b.ToTable("CashFollowItem");
            });

            modelBuilder.Entity<model.CashFollowLog>(b =>
            {
                b.HasIndex(m => new { m.CashFollowId });
                b.ToTable("CashFollowLog");
            });

            modelBuilder.Entity<model.GroupsPermission>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.GroupId });
                b.ToTable("GroupsPermission");
            });

            modelBuilder.Entity<model.Departments>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id });
                b.ToTable("Departments");
            });

            modelBuilder.Entity<model.UnitTypeMap>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.UnitCode }).IsUnique(true);
                b.HasIndex(m => new { m.Id });
                b.ToTable("UnitTypeMaps");
            });

            modelBuilder.Entity<model.UnitTypeMap>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.UnitCode }).IsUnique(true);
                b.HasIndex(m => new { m.Id });
                b.ToTable("UnitTypeMaps");
            });

            modelBuilder.Entity<model.UserUnitsManages>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.OfficeCode, m.UserId });
                b.HasIndex(m => new { m.UserId });
                b.HasIndex(m => new { m.Id });
                b.ToTable("UserUnitsManages");
            });

            modelBuilder.Entity<model.UserGroups>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.UserId, m.GroupId });
                b.HasIndex(m => new { m.UserId });
                b.ToTable("UserGroups");
            });

            modelBuilder.Entity<model.UserConcurrently>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.UserId, m.GroupCode });
                b.HasIndex(m => new { m.UserId });
                b.ToTable("UserConcurrently");
            });

            modelBuilder.Entity<model.FilePdfCreateLogs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.UnitId, m.Type });
                b.HasIndex(m => new { m.Id });
                b.ToTable("FilePdfCreateLogs");
            });

            modelBuilder.Entity<model.SpecialUnitFollowConfigs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id });
                b.HasIndex(m => new { m.UnitCode });
                b.ToTable("SpecialUnitFollowConfigs");
            });

            modelBuilder.Entity<model.Notify>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id });
                b.ToTable("Notify");
            });

            #endregion Phase1

            #region Phase2

            modelBuilder.Entity<model.DM>(b =>
            {
                b.HasKey(m => new { m.MaDM, m.IDTree }).IsClustered(true);
                b.HasIndex(m => new { m.IDTree });
                b.HasIndex(m => new { m.MaDM });
                b.ToTable("DM");
            });
            modelBuilder.Entity<model.DMGG>(b =>
            {
                b.HasKey(m => m.MaGG).IsClustered(true);
                b.ToTable("DMGG");
            });
            modelBuilder.Entity<model.GiaKCB>(b =>
            {
                b.HasKey(m => m.IDGiaCT).IsClustered(true);
                b.ToTable("GiaKCB");
            });
            modelBuilder.Entity<model.DMBS_ChuyenKhoa>(b =>
            {
                b.HasKey(m => m.Ma).IsClustered(true);
                b.HasIndex(m => new { m.Ten });
                b.HasIndex(m => new { m.Ma });
                b.ToTable("DMBS_ChuyenKhoa");
            });

            modelBuilder.Entity<model.DMCP>(b =>
            {
                b.HasKey(m => m.MaCP).IsClustered(true);
                b.HasIndex(m => new { m.TenCP });
                b.HasIndex(m => new { m.MaCP });
                b.ToTable("DMCP");
            });

            modelBuilder.Entity<model.DMDV>(b =>
            {
                b.HasKey(m => m.MaDV).IsClustered(true);
                b.HasIndex(m => new { m.TenDV });
                b.HasIndex(m => new { m.MaDV });
                b.ToTable("DMDV");
            });

            modelBuilder.Entity<model.DMHuyen>(b =>
            {
                b.HasKey(m => m.MaHuyen).IsClustered(true);
                b.HasIndex(m => new { m.TenHuyen });
                b.HasIndex(m => new { m.MaHuyen });
                b.ToTable("DMHuyen");
            });

            modelBuilder.Entity<model.DMPN>(b =>
            {
                b.HasKey(m => m.PhapNhanId).IsClustered(true);
                b.HasIndex(m => new { m.PhapNhanName });
                b.HasIndex(m => new { m.PhapNhanId });
                b.ToTable("DMPN");
            });

            //modelBuilder.Entity<model.HDCTV>(b =>
            //{
            //    b.HasKey(m => m.MaBS).IsClustered(true);
            //    b.HasIndex(m => new { m.TenBS });
            //    b.HasIndex(m => new { m.MaBS });
            //    b.ToTable("HDCTV");
            //});

            modelBuilder.Entity<model.HDCTVExt>(b =>
            {
                b.HasKey(m => m.MaBS).IsClustered(true);
                b.HasIndex(m => new { m.MaBS });
                b.ToTable("HDCTVExt");
            });

            modelBuilder.Entity<model.HDKCB>(b =>
            {
                b.HasKey(m => m.IDHD).IsClustered(true);
                b.HasIndex(m => new { m.MaHD });
                b.HasIndex(m => new { m.IDHD });
                b.ToTable("HDKCB");
            });

            modelBuilder.Entity<model.LoaiDeXuat>(b =>
            {
                b.HasKey(m => m.LoaiDeXuatCode).IsClustered(true);
                b.HasIndex(m => new { m.LoaiDeXuatName });
                b.HasIndex(m => new { m.LoaiDeXuatCode });
                b.ToTable("LoaiDeXuat");
            });

            modelBuilder.Entity<model.ProfileCK>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.HasIndex(m => new { m.ProfileCKMa });
                b.HasIndex(m => new { m.Id });
                b.ToTable("ProfileCK");
            });

            modelBuilder.Entity<model.ProfileCKCP>(b =>
            {
                b.HasKey(m => new { m.ProfileCKMa, m.CPMa }).IsClustered(true);
                b.ToTable("ProfileCKCP");
            });

            modelBuilder.Entity<model.Relationship>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.HasIndex(m => new { m.RelationshipCode });
                b.HasIndex(m => new { m.Id });
                b.ToTable("Relationship");
            });

            modelBuilder.Entity<model.CLS>(b =>
            {
                b.HasKey(m => m.IDCLS).IsClustered(true);
                b.HasIndex(m => new { m.IDCLS });
                b.HasIndex(m => new { m.S_ID });
                b.ToTable("CLS");
            });

            modelBuilder.Entity<model.CLSCT>(b =>
            {
                b.HasKey(m => m.IDCLSCT).IsClustered(true);
                b.HasIndex(m => new { m.IDCLSCT });
                b.ToTable("CLSCT");
            });

            modelBuilder.Entity<model.DKK>(b =>
            {
                b.HasKey(m => m.MaBN).IsClustered(true);
                b.HasIndex(m => new { m.P_ID });
                b.HasIndex(m => new { m.MaBN });
                b.ToTable("DKK");
            });

            modelBuilder.Entity<model.NhCP>(b =>
            {
                b.HasKey(m => m.MaNhCP).IsClustered(true);
                b.HasIndex(m => new { m.MaNhCP });
                b.ToTable("NhCP");
            });

            modelBuilder.Entity<model.DMBangGiaChiNhanh>(b =>
            {
                b.HasKey(m => m.MaBangGiaChiNhanh).IsClustered(true);
                b.HasIndex(m => new { m.MaBangGiaChiNhanh });
                b.HasIndex(m => new { m.MaCP });
                b.ToTable("DMBangGiaChiNhanh");
            });

            modelBuilder.Entity<model.DMCTV>(b =>
            {
                b.HasKey(m => m.MaBS).IsClustered(true);
                b.HasIndex(m => new { m.TenBS });
                b.HasIndex(m => new { m.MaBS });
                b.ToTable("DMCTV");
            });

            modelBuilder.Entity<model.DMCTVExt>(b =>
            {
                b.HasKey(m => m.MaBS).IsClustered(true);
                b.HasIndex(m => new { m.MaBS });
                b.ToTable("DMCTVExt");
            });

            modelBuilder.Entity<model.TBL_CTVGROUP>(b =>
            {
                b.HasKey(m => m.CTVGroupID).IsClustered(true);
                b.HasIndex(m => new { m.CTVGroupName });
                b.HasIndex(m => new { m.CTVGroupID });
                b.ToTable("TBL_CTVGROUP");
            });

            modelBuilder.Entity<model.TBL_CTVGROUPSUB>(b =>
            {
                b.HasKey(m => m.SubId).IsClustered(true);
                b.HasIndex(m => new { m.SubName });
                b.HasIndex(m => new { m.SubId });
                b.ToTable("TBL_CTVGROUPSUB");
            });

            modelBuilder.Entity<model.TBL_CTVGROUPSUB1_DETAIL>(b =>
            {
                b.HasKey(m => new { m.SubId, m.MaCP }).IsClustered(true);
                b.HasIndex(m => new { m.SubId });
                b.HasIndex(m => new { m.MaCP });
                b.ToTable("TBL_CTVGROUPSUB1_DETAIL");
            });

            modelBuilder.Entity<model.TBL_CTVGROUPSUB2_DETAIL>(b =>
            {
                b.HasKey(m => new { m.SubId, m.MaCP }).IsClustered(true);
                b.HasIndex(m => new { m.SubId });
                b.HasIndex(m => new { m.MaCP });
                b.ToTable("TBL_CTVGROUPSUB2_DETAIL");
            });

            modelBuilder.Entity<model.ProcessMapRole>(b =>
            {
                b.HasKey(m => new { m.IDRole, m.GroupCode }).IsClustered(true);
                b.HasIndex(m => new { m.IDRole });
                b.HasIndex(m => new { m.GroupCode });
                b.Property(p => p.GroupCode).HasMaxLength(450);
                b.ToTable("ProcessMapRole");
            });

            modelBuilder.Entity<model.ProcessRole>(b =>
            {
                b.HasKey(m => new { m.IDRole }).IsClustered(true);
                b.ToTable("ProcessRole");
                b.Property(p => p.Name).HasMaxLength(250);
            });

            modelBuilder.Entity<Models.Phase2.Process>(b =>
            {
                b.HasKey(m => new { m.ProcessId }).IsClustered(true);
                b.Property(m => m.ProcessId).ValueGeneratedOnAdd();
                b.ToTable("Process");
                b.Property(p => p.ProcessName).HasMaxLength(250);
            });

            modelBuilder.Entity<model.DeXuat>(b =>
            {
                b.HasKey(m => new { m.DeXuatCode }).IsClustered(true);
                b.HasIndex(m => new { m.DeXuatCode });
                b.ToTable("DeXuat");
                b.Property(p => p.DeXuatName).HasMaxLength(100);
            });
            modelBuilder.Entity<model.DeXuatChiTiet>(b =>
            {
                b.HasKey(m => new { m.DeXuatCode, m.FieldName }).IsClustered(true);
                b.Property(m => m.DeXuatCode).ValueGeneratedOnAdd();
                b.ToTable("DeXuatChiTiet");
                b.Property(p => p.FieldName).HasMaxLength(100);
                b.Property(p => p.ValueOld).HasMaxLength(100);
                b.Property(p => p.ValueNew).HasMaxLength(100);
                b.Property(p => p.Note).HasMaxLength(500);
            });

            modelBuilder.Entity<model.DeXuatLuanChuyenMa>(b =>
            {
                b.HasKey(m => new { m.DeXuatCode, m.MaCTV }).IsClustered(true);
                b.Property(m => m.DeXuatCode).ValueGeneratedOnAdd();
                b.ToTable("DeXuatLuanChuyenMa");
                b.Property(p => p.MaCTV).HasMaxLength(10);
                b.Property(p => p.Note).HasMaxLength(500);
            });

            modelBuilder.Entity<model.DeXuatKhoaMaCTV>(b =>
            {
                b.HasKey(m => new { m.DeXuatCode, m.MaCTV }).IsClustered(true);
                b.Property(m => m.DeXuatCode).ValueGeneratedOnAdd();
                b.ToTable("DeXuatKhoaMaCTV");
                b.Property(p => p.MaCTV).HasMaxLength(10);
                b.Property(p => p.Note).HasMaxLength(500);
            });

            modelBuilder.Entity<model.DeXuatGhiChu>(b =>
            {
                b.HasKey(m => new { m.DeXuatCode, m.ProcessStepId }).IsClustered(true);
                b.ToTable("DeXuatGhiChu");
                b.Property(p => p.Note).HasMaxLength(500);
            });

            modelBuilder.Entity<Models.Phase2.ProcessStep>(b =>
             {
                 b.HasKey(m => new { m.StepId }).IsClustered(true);
                 b.Property(m => m.StepId).ValueGeneratedOnAdd();
                 b.ToTable("ProcessStep");
                 b.Property(p => p.IsLastStep).HasDefaultValue(false);
                 b.Property(p => p.StepName).HasMaxLength(250);
             });

            modelBuilder.Entity<Models.Phase2.ApprovedFlow>(b =>
             {
                 b.HasKey(m => new { m.FlowId }).IsClustered(true);
                 b.Property(m => m.FlowId).ValueGeneratedOnAdd();
                 b.HasIndex(m => new { m.DoctorId });
                 b.ToTable("ApprovedFlow");
                 b.Property(p => p.FlowName).HasMaxLength(300);
                 b.Property(p => p.StatusFlow).HasDefaultValue(0);
             });

            modelBuilder.Entity<Models.Phase2.ApprovedFlowDetail>(b =>
            {
                b.HasKey(m => new { m.FlowDetailId }).IsClustered(true);
                b.Property(m => m.FlowDetailId).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.FlowId });
                b.ToTable("ApprovedFlowDetail");
                b.Property(p => p.Status).HasDefaultValue(0);
                b.Property(p => p.Reason).HasMaxLength(300);
                b.Property(p => p.ApprovedUser).HasMaxLength(500);
                b.Property(p => p.ListUserAccept).HasMaxLength(500);
            });
            modelBuilder.Entity<Models.Phase2.TBL_LOGCTVGROUP>(b =>
            {
                b.HasKey(m => new { m.LogCtvId }).IsClustered(true);
                b.Property(m => m.LogCtvId).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.DoctorId });
                b.ToTable("TBL_LOGCTVGROUP");
                b.Property(p => p.LogCtvStatus).HasDefaultValue(0);
                b.Property(p => p.IsActive).HasDefaultValue(1);
                b.Property(p => p.ThuSau).HasDefaultValue(0);
                //b.Property(p => p.IsHetHan).HasComputedColumnSql($"CASE WHEN GETDATE() > {nameof(Models.Phase2.TBL_LOGCTVGROUP.ToDate)} THEN 1 ELSE 0 END PERSISTED NOT NULL", false);
                //(case when [ToDate]<getdate() then CONVERT([bit],(1),0) else CONVERT([bit],(0),0) end)
            });

            #endregion Phase2

            #region NextStep

            modelBuilder.Entity<model.NextStep.Teacher>(b =>
            {
                b.HasKey(m => new { m.TeacherId}).IsClustered(true);
                b.HasIndex(m => new { m.TeacherId });
                b.ToTable("Teacher");
            });
            modelBuilder.Entity<model.NextStep.Student>(b =>
            {
                b.HasKey(m => new { m.StudentId }).IsClustered(true);
                b.HasIndex(m => new { m.StudentId });
                b.ToTable("Student");
            });

            #endregion NextStep

            #region Kế hoạch đầu tư

            modelBuilder.Entity<model.InvestmentPlanContents>(b =>
            {
                b.HasKey(m => m.InvestmentPlanContentId).IsClustered(true);
                b.Property(m => m.InvestmentPlanContentId).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.InvestmentPlanContentId }).IsUnique(true);
                b.ToTable("InvestmentPlanContents");
            });

            modelBuilder.Entity<model.InvestmentPlan>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.Id, m.Status, m.UnitId });
                b.ToTable("InvestmentPlan");
            });

            modelBuilder.Entity<model.InvestmentPlanDetails>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.Id, m.InvestmentPlanId });
                b.ToTable("InvestmentPlanDetails");
            });
            modelBuilder.Entity<model.InvestmentPlanAggregate>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.Id, m.InvestmentPlanId });
                b.ToTable("InvestmentPlanAggregate");
            });
            modelBuilder.Entity<model.InvestmentPlanLogs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.Id, m.InvestmentPlanId });
                b.ToTable("InvestmentPlanLogs");
            });

            modelBuilder.Entity<model.UserConcurrently>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique();
                b.HasIndex(m => new { m.UnitId, m.UnitCode });
                b.ToTable("UserConcurrently");
            });

            #endregion Kế hoạch đầu tư

            #region Kế hoạch doanh thu

            modelBuilder.Entity<model.RevenuePlan>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.Id, m.UnitId, m.Status });
                b.ToTable("RevenuePlan");
            });

            modelBuilder.Entity<model.RevenuePlanAggregate>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.RevenuePlanContentId });
                b.ToTable("RevenuePlanAggregate");
            });

            modelBuilder.Entity<model.RevenuePlanContents>(b =>
            {
                b.HasKey(m => m.RevenuePlanContentId).IsClustered(true);
                b.Property(m => m.RevenuePlanContentId).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.RevenuePlanContentId }).IsUnique(true);
                b.ToTable("RevenuePlanContents");
            });

            modelBuilder.Entity<model.RevenuePlanCashDetails>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.RevenuePlanId });
                b.ToTable("RevenuePlanCashDetails");
            });

            modelBuilder.Entity<model.RevenuePlanCustomerDetails>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.RevenuePlanId });
                b.ToTable("RevenuePlanCustomerDetails");
            });

            modelBuilder.Entity<model.RevenuePlanLogs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.RevenuePlanId });
                b.HasIndex(m => new { m.RevenuePlanId, m.FromStatus });
                b.ToTable("RevenuePlanLogs");
            });

            #endregion Kế hoạch doanh thu

            #region Kế hoạch lợi nhuận

            modelBuilder.Entity<model.ProfitPlan>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.UnitId, m.Status, m.IsSub });
                b.ToTable("ProfitPlan");
            });

            modelBuilder.Entity<model.ProfitPlanAggregates>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.Id }).IsUnique(true);
                b.HasIndex(m => new { m.ProfitPlanId });
                b.ToTable("ProfitPlanAggregates");
            });

            modelBuilder.Entity<model.ProfitPlanGroups>(b =>
            {
                b.HasKey(m => m.ProfitPlanGroupId).IsClustered(true);
                b.Property(m => m.ProfitPlanGroupId).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.ProfitPlanGroupId }).IsUnique(true);
                b.HasIndex(m => new { m.ForSubject });
                b.ToTable("ProfitPlanGroups");
            });

            modelBuilder.Entity<model.ProfitPlanDetails>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.ProfitPlanId });
                b.ToTable("ProfitPlanDetails");
            });

            modelBuilder.Entity<model.ProfitPlanLogs>(b =>
            {
                b.HasKey(m => m.Id).IsClustered(true);
                b.Property(m => m.Id).ValueGeneratedOnAdd();
                b.HasIndex(m => new { m.ProfitPlanId });
                b.ToTable("ProfitPlanLogs");
            });

            #endregion Kế hoạch lợi nhuận
        }
    }
}