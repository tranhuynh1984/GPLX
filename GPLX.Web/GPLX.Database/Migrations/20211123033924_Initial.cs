using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FunctionUnique = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ActuallySpent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportForWeek = table.Column<int>(type: "int", nullable: false),
                    ReportForWeekName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalEstimateCost = table.Column<long>(type: "bigint", nullable: false),
                    TotalActuallySpent = table.Column<long>(type: "bigint", nullable: false),
                    TotalAmountLeft = table.Column<long>(type: "bigint", nullable: false),
                    TotalActualSpentAtTime = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActuallySpent", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ActuallySpentItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualSpent = table.Column<long>(type: "bigint", nullable: false),
                    AmountLeft = table.Column<long>(type: "bigint", nullable: false),
                    ActualSpentAtTime = table.Column<long>(type: "bigint", nullable: false),
                    ActualPayWeek = table.Column<int>(type: "int", nullable: false),
                    ActualPayWeekName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountantCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualSpentType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestPayWeekName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestPayWeek = table.Column<int>(type: "int", nullable: false),
                    CostEstimateItemTypeId = table.Column<int>(type: "int", nullable: false),
                    CostEstimateItemTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostEstimateGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActuallySpentItem", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ActuallySpentLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActuallySpentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActuallySpentLog", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ActuallySpentMapItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActuallySpentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActuallySpentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActuallySpentMapItem", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CashFollow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathFilePdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFollow", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CashFollowItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFollowTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFollowTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthOfYear = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    SubCashFollowTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubCashFollowTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    PaymentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostEstimateItemTypeId = table.Column<int>(type: "int", nullable: false),
                    CostEstimateItemTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashFollowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Cell = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Col = table.Column<int>(type: "int", nullable: false),
                    IsBold = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFollowItem", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CashFollowLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFollowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFollowLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashFollowType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRoot = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    RefPaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFollowType", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostEstimate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    BeginAvailableBalance = table.Column<long>(type: "bigint", nullable: true),
                    AccountBalance = table.Column<long>(type: "bigint", nullable: true),
                    ExpectRevenue = table.Column<long>(type: "bigint", nullable: true),
                    EstimatedCost = table.Column<long>(type: "bigint", nullable: true),
                    OperatingCost = table.Column<long>(type: "bigint", nullable: true),
                    RoutineCost = table.Column<long>(type: "bigint", nullable: true),
                    NonRoutineCost = table.Column<long>(type: "bigint", nullable: true),
                    InvestmentCost = table.Column<long>(type: "bigint", nullable: true),
                    FinancialCost = table.Column<long>(type: "bigint", nullable: true),
                    TotalExpenditureCapital = table.Column<long>(type: "bigint", nullable: true),
                    TotalSpendingLoan = table.Column<long>(type: "bigint", nullable: true),
                    EndAvailableBalance = table.Column<long>(type: "bigint", nullable: true),
                    Funds = table.Column<long>(type: "bigint", nullable: true),
                    WorkingBalanceCost = table.Column<long>(type: "bigint", nullable: true),
                    EquityCost = table.Column<long>(type: "bigint", nullable: true),
                    PlanCutCost = table.Column<long>(type: "bigint", nullable: true),
                    RotationProposal = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdaterId = table.Column<int>(type: "int", nullable: true),
                    UpdaterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostEstimateBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportForWeek = table.Column<int>(type: "int", nullable: false),
                    ReportForWeekName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    IsSub = table.Column<bool>(type: "bit", nullable: false),
                    CostEstimateType = table.Column<int>(type: "int", nullable: false),
                    CostEstimateTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcelFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PdfFile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEstimate", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostEstimateItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RequestCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestContentNonUnicode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostEstimateItemTypeId = table.Column<int>(type: "int", nullable: false),
                    CostEstimateItemTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostEstimatePaymentType = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayWeek = table.Column<int>(type: "int", nullable: false),
                    PayWeekName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillCost = table.Column<long>(type: "bigint", nullable: false),
                    RequestImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EstimateType = table.Column<int>(type: "int", nullable: false),
                    IsLock = table.Column<int>(type: "int", nullable: false),
                    CostEstimateGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    RequesterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEstimateItem", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostEstimateItemLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CostEstimateItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    FromStatusName = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEstimateItemLogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostEstimateItemMapActuallySpentItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActuallySpentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActuallySpentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostEstimateItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEstimateItemMapActuallySpentItem", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostEstimateItemType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    ComparingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForUnitType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEstimateItemType", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostEstimateLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CostEstimateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEstimateLogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostEstimateMapItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostEstimateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostEstimateItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEstimateMapItems", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusForCostEstimateType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusForSubject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsApprove = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostStatuses", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CostStatusesGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostStatusesGroups", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Functions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unique = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Functions", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "GroupsPermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    FunctionId = table.Column<int>(type: "int", nullable: false),
                    Permission = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsPermission", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SctData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountReciprocalNumber = table.Column<int>(type: "int", nullable: false),
                    IncurredDebt = table.Column<long>(type: "bigint", nullable: true),
                    IncurredHave = table.Column<long>(type: "bigint", nullable: true),
                    SurplusDebt = table.Column<long>(type: "bigint", nullable: true),
                    SurplusHave = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uploader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActuallySpentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SctData", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficesName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OfficesCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficesDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficesType = table.Column<int>(type: "int", nullable: false),
                    OfficesOrder = table.Column<int>(type: "int", nullable: false),
                    OfficesTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficesContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficesAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ParrentID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OfficesGuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "UnitTypeMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTypeMaps", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountGuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_UserId_CreatedDate_Action",
                table: "ActionLogs",
                columns: new[] { "UserId", "CreatedDate", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_UserName_CreatedDate_Action",
                table: "ActionLogs",
                columns: new[] { "UserName", "CreatedDate", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_ActuallySpent_UnitId_ReportForWeek_Id",
                table: "ActuallySpent",
                columns: new[] { "UnitId", "ReportForWeek", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ActuallySpentItem_Id_ActualSpentType_RequestPayWeek",
                table: "ActuallySpentItem",
                columns: new[] { "Id", "ActualSpentType", "RequestPayWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_ActuallySpentLog_ActuallySpentId_ToStatus",
                table: "ActuallySpentLog",
                columns: new[] { "ActuallySpentId", "ToStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ActuallySpentMapItem_Id_ActuallySpentId_ActuallySpentItemId",
                table: "ActuallySpentMapItem",
                columns: new[] { "Id", "ActuallySpentId", "ActuallySpentItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_CashFollow_Id",
                table: "CashFollow",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CashFollow_UnitId_Year_Status",
                table: "CashFollow",
                columns: new[] { "UnitId", "Year", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowItem_CashFollowId",
                table: "CashFollowItem",
                column: "CashFollowId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowItem_Id",
                table: "CashFollowItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowLog_CashFollowId",
                table: "CashFollowLog",
                column: "CashFollowId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowType_Id_IsRoot",
                table: "CashFollowType",
                columns: new[] { "Id", "IsRoot" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimate_UserId_Status_CreatedDate",
                table: "CostEstimate",
                columns: new[] { "UserId", "Status", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimate_UserId_Status_CreatedDate_DepartmentId",
                table: "CostEstimate",
                columns: new[] { "UserId", "Status", "CreatedDate", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimateItem_CreatorId_Status",
                table: "CostEstimateItem",
                columns: new[] { "CreatorId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimateItem_RequestCode",
                table: "CostEstimateItem",
                column: "RequestCode",
                unique: true,
                filter: "[RequestCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimateItemLogs_UserId_CreatedDate",
                table: "CostEstimateItemLogs",
                columns: new[] { "UserId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimateItemMapActuallySpentItem_ActuallySpentItemId_CostEstimateItemId",
                table: "CostEstimateItemMapActuallySpentItem",
                columns: new[] { "ActuallySpentItemId", "CostEstimateItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimateItemMapActuallySpentItem_Status_CostEstimateItemId_ActuallySpentItemId",
                table: "CostEstimateItemMapActuallySpentItem",
                columns: new[] { "Status", "CostEstimateItemId", "ActuallySpentItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimateLogs_UserId_CreatedDate",
                table: "CostEstimateLogs",
                columns: new[] { "UserId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimateMapItems_CostEstimateId_CostEstimateItemId_Status",
                table: "CostEstimateMapItems",
                columns: new[] { "CostEstimateId", "CostEstimateItemId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CostStatuses_Type_Status",
                table: "CostStatuses",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CostStatusesGroups_GroupCode",
                table: "CostStatusesGroups",
                column: "GroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_CostStatusesGroups_GroupCode_StatusesId",
                table: "CostStatusesGroups",
                columns: new[] { "GroupCode", "StatusesId" });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Id",
                table: "Departments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupCode",
                table: "Groups",
                column: "GroupCode",
                unique: true,
                filter: "[GroupCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Id",
                table: "Groups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsPermission_GroupId",
                table: "GroupsPermission",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SctData_Type_Status",
                table: "SctData",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierName",
                table: "Suppliers",
                column: "SupplierName");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Id",
                table: "Units",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Units_OfficesName",
                table: "Units",
                column: "OfficesName");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTypeMaps_Id",
                table: "UnitTypeMaps",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTypeMaps_UnitCode",
                table: "UnitTypeMaps",
                column: "UnitCode",
                unique: true,
                filter: "[UnitCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLogs");

            migrationBuilder.DropTable(
                name: "ActuallySpent");

            migrationBuilder.DropTable(
                name: "ActuallySpentItem");

            migrationBuilder.DropTable(
                name: "ActuallySpentLog");

            migrationBuilder.DropTable(
                name: "ActuallySpentMapItem");

            migrationBuilder.DropTable(
                name: "CashFollow");

            migrationBuilder.DropTable(
                name: "CashFollowItem");

            migrationBuilder.DropTable(
                name: "CashFollowLog");

            migrationBuilder.DropTable(
                name: "CashFollowType");

            migrationBuilder.DropTable(
                name: "CostEstimate");

            migrationBuilder.DropTable(
                name: "CostEstimateItem");

            migrationBuilder.DropTable(
                name: "CostEstimateItemLogs");

            migrationBuilder.DropTable(
                name: "CostEstimateItemMapActuallySpentItem");

            migrationBuilder.DropTable(
                name: "CostEstimateItemType");

            migrationBuilder.DropTable(
                name: "CostEstimateLogs");

            migrationBuilder.DropTable(
                name: "CostEstimateMapItems");

            migrationBuilder.DropTable(
                name: "CostStatuses");

            migrationBuilder.DropTable(
                name: "CostStatusesGroups");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Functions");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "GroupsPermission");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "SctData");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "UnitTypeMaps");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
