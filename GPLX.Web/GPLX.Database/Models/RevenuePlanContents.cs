namespace GPLX.Database.Models
{
    public class RevenuePlanContents
    {
        public int RevenuePlanContentId { get; set; }
        public string RevenuePlanContentName { get; set; }
        public int Order { get; set; }

        public bool IsRequire { get; set; }
        public string GroupFor { get; set; }
        public string ForSubject { get; set; }
        public string Style { get; set; }
    }
}
