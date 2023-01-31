namespace GPLX.Database.Models
{
    public class ProfitPlanGroups
    {
        public int ProfitPlanGroupId { get; set; }
        public string ProfitPlanGroupName { get; set; }
        /// <summary>
        /// ID của group cha
        /// </summary>
        public int ProfitPlanParentGroupId { get; set; }

        public string ForSubject { get; set; }
        public int Order { get; set; }
        public string GroupFor { get; set; }
        public string RulesCellOnRow { get; set; }
        public bool IsRequire { get; set; }
        public string Style { get; set; }
    }
}
