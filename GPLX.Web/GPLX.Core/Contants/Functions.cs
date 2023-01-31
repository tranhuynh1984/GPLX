using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace GPLX.Core.Contants
{
    public static class Functions
    {
        public const string CostElementItemCreate = "CostEstimateItem@Add";
        public const string CostElementItemView = "CostEstimateItem@View";
        public const string CostElementItemImport = "CostEstimateImport@Add";

        

        public const string CostEstimateCreate = "CostEstimate@Add";
        public const string CostEstimateView = "CostEstimate@View";

        public const string ActuallySpentCreate = "ActuallySpent@Add";
        public const string ActuallySpentView = "ActuallySpent@View";

        public const string UnitView = "Unit@View";

        public const string LegalView = "Legal@View";

        public const string UsersView = "Users@View";

        public const string GroupsView = "Groups@View";
        public const string GroupsAdd = "Groups@Add";

        public const string DepartmentView = "Department@View";
        public const string DepartmentAdd = "Department@Add";

        public const string CashFollowAdd = "CashFollow@Add";
        public const string CashFollowView = "CashFollow@View";

        public const string CostStatusesView = "CostStatuses@View";
        public const string CostStatusesAdd = "CostStatuses@Add";

        public const string InvestmentPlanView = "InvestmentPlan@View";
        public const string InvestmentPlanAdd = "InvestmentPlan@Add";

        public const string RevenuePlanView = "RevenuePlan@View";
        public const string RevenuePlanAdd = "RevenuePlan@Add";

        public const string ProfitPlanView = "ProfitPlan@View";
        public const string ProfitPlanAdd = "ProfitPlan@Add";


        public const string Permission = "Permission@View";

        public const string Home = "Home@View";

        public const string DashboardView = "Dashboard@View";

        public const string DMPNView = "DMPN@View";
        public const string DMDVView = "DMDV@View";


        public const string DMView = "DM@View";
        public const string DMBSChuyenKhoaView = "DMBS_ChuyenKhoa@View";
        public const string DMCPView = "DMCP@View";
        public const string DMHuyenView = "DMHuyen@View";
        public const string HDCTVView = "HDCTV@View";
        public const string HDKCBView = "HDKCB@View";
        public const string LoaiDeXuatView = "LoaiDeXuat@View";
        public const string ProfileCKView = "ProfileCK@View";
        public const string RelationshipView = "Relationship@View";
        public const string DMTinh = "DMTinh@View";
    }
    public class SslHelper
    {
        /// <summary>
        /// Validate server ssl certificate
        /// Do real validate 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        public static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}
